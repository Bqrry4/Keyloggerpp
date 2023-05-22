using Interpreter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Interpreter
{
    /// <summary>
    /// Class that encapsulates klpp script interpreting and running.
    /// <para>Reads the given script line by line and interprets them into commands</para>
    /// </summary>
    public class Interpreter
    {
        private Dictionary<string, List<IKlppCommand>> _hotkeyScripts;
        //private CommandFactory _commandFactory;

        /// <summary>
        /// Interpret the given script and return the hotkeys to be listened for as triggers.
        /// </summary>
        /// <param name="script">String containing the script to be interpreted</param>
        /// <param name="hotkeys">List of strings representing the triggers to be listened for</param>
        public Interpreter(in string script, out List<string> hotkeys)
        {
            hotkeys = new List<string>();
            //Step 1: Read the hotkey and send it to the Intermediary.
            //Step 2: Read the script line by line and construct Command objects.
            //Step 3: Repeat 1-2 until end of string.

            Regex hotkeyPattern = new Regex("(.+)::\\n({\\n(?:\\s{4}.+\\n)+})");

            Match hotkey = hotkeyPattern.Match(script);

            ushort lineIndex = 0;

            while (hotkey.Success)
            {
                hotkeys.Append(hotkey.Groups[1].Value);
                string[] lines = hotkey.Groups[2].Value.Split('\n');
                List<IKlppCommand> hotkeyCommandList = new List<IKlppCommand>();

                foreach(string line in lines)
                {
                    lineIndex++;
                    try
                    {
                        IKlppCommand command = CommandFactory.Parse(line);
                        hotkeyCommandList.Add(command);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error at line " + lineIndex + ": " + ex.Message);
                    }
                }

                _hotkeyScripts.Add(hotkey.Groups[1].Value, hotkeyCommandList);
            }
        }
        /// <summary>
        /// Runs the list of commands registered to the given trigger
        /// </summary>
        /// <remarks>This method will be called by the Intermediary when it detects that the given hotkey was pressed.</remarks>
        /// <param name="hotkey">String representing the trigger</param>
        /// <exception cref="HotkeyNotFoundException">If the given hotkey was not registered within the interpreter </exception>
        public void Run(in string hotkey)
        {
            List<IKlppCommand> commandList;
            if(!_hotkeyScripts.TryGetValue(hotkey, out commandList))
            {
                throw new HotkeyNotFoundException("Hotkey " + hotkey + " is not registered in the interpreter!");
            }
            foreach(IKlppCommand command in commandList)
            {
                command.Execute();
            }
        }
    }
}
