/**************************************************************************
 *                                                                        *
 *  File:        ScriptInterpreter.cs                                     *
 *  Copyright:   (c) Olăreț Radu                                          *
 *               @Kakerou_CLUB                                            *
 *  Description: Klpp script interpreter           .                      *
 *                                                                        *
 **************************************************************************/

using Interpreter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Interpreter
{
    /// <summary>
    /// Class that encapsulates klpp script interpreting and running.
    /// <para>Reads the given script line by line and interprets them into commands</para>
    /// </summary>
    public class ScriptInterpreter : IObserver<string>
    {
        private Dictionary<string, List<IKlppCommand>> _hotkeyScripts = new Dictionary<string, List<IKlppCommand>>();

        /// <summary>
        /// Interpret the given script and return the hotkeys to be listened for as triggers.
        /// </summary>
        /// <param name="script">String containing the script to be interpreted</param>
        /// <param name="hotkeys">List of strings representing the triggers to be listened for</param>
        /// <exception cref="ArgumentException">If the script dorsn't follow klpp script syntax</exception>
        /// <exception cref="AggregateException">If there is an error parsing a line of script</exception>
        public void Parse(in string script, out List<string> hotkeys)
        {
            CommandFactory.ResetMouseState();

            hotkeys = new List<string>();
            //Step 1: Read the hotkey and send it to the Intermediary.
            //Step 2: Read the script line by line and construct Command objects.
            //Step 3: Repeat 1-2 until end of string.

            Regex hotkeyPattern = new Regex("(.+)::\\n{(\\n(?:\\s*.+\\n)+)}");

            Match hotkey = hotkeyPattern.Match(script);

            if (!hotkey.Success)
            {
                throw new ArgumentException("Invalid script syntax!");
            }

            ushort lineIndex = 0;

            while (hotkey.Success)
            {
                hotkeys.Append(hotkey.Groups[1].Value);
                string[] lines = hotkey.Groups[2].Value.Split('\n');
                List<IKlppCommand> hotkeyCommandList = new List<IKlppCommand>();

                foreach(string line in lines)
                {
                    lineIndex++;
                    if (line != string.Empty)
                    {
                        try
                        {
                            IKlppCommand command = CommandFactory.Parse(line);
                            hotkeyCommandList.Add(command);
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("Error at line " + lineIndex + ": " + ex.Message);
                            //break;
                            throw new AggregateException("Error parsing klpp script at line " + lineIndex + ": " + ex.Message + "\n\t Line: " + line, ex);
                        }
                    }
                }

                _hotkeyScripts.Add(hotkey.Groups[1].Value, hotkeyCommandList);
                //_hotkeyScripts[hotkey.Groups[1].Value] = hotkeyCommandList;
                hotkey = hotkey.NextMatch();
            }
        }
        /// <summary>
        /// Runs the list of commands registered to the given trigger
        /// </summary>
        /// <remarks>This method will be called by the Intermediary when it detects that the given hotkey was pressed.</remarks>
        /// <param name="hotkey">String representing the trigger</param>
        /// <exception cref="HotkeyNotFoundException">If the given hotkey was not registered within the interpreter </exception>
        /// <exception cref="AggregateException">If an error occurs while executing a command </exception>
        private void Run(in string hotkey)
        {
            if (!_hotkeyScripts.TryGetValue(hotkey, out List<IKlppCommand> commandList))
            {
                throw new HotkeyNotFoundException("Hotkey " + hotkey + " is not registered in the interpreter!");
            }
            foreach (IKlppCommand command in commandList)
            {
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    throw new AggregateException("Error executing hotkey " + hotkey, ex);
                }
            }
        }

        public void Clear()
        {
            _hotkeyScripts.Clear();
        }

        public void OnNext(string value)
        {
            try
            {
                Run(value);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void OnError(Exception error)
        {
            Clear();
        }

        public void OnCompleted()
        {
            Clear();
        }
    }
}
