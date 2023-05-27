/**************************************************************************
 *                                                                        *
 *  File:        CommandFactory.cs                                        *
 *  Copyright:   (c) Olăreț Radu                                          *
 *               @Kakerou_CLUB                                            *
 *  Description: Class that builds Commands from a lines of text          *
 *                                                                        *
 **************************************************************************/

using InputListener;
using Interpreter.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace Interpreter
{
    internal static class CommandFactory
    {
        /// <summary>
        /// Takes a line of script representing a Keylogger++ command and creates a corresponding KlppCommand object 
        /// </summary>
        /// <param name="command">String containing a line of Klpp script</param>
        /// <returns>Command parsed out of given text</returns>
        /// <exception cref="MalformedLineException"> When a line does not conform to klpp script</exception>
        /// <exception cref="NotImplementedException"> When klpp format is respected, but the command given is not recognized </exception>
        /// <exception cref="ArgumentException"> When a command was recognized, but its' usage was erroneous</exception>
        public static IKlppCommand Parse(string command)
        {
            Regex commandPattern = new Regex("(\\w+)\\(((?:(?: ?\\d+,?)|(?: ?\"(?:[^\"])*\",?))*)\\)+");

            Match match = commandPattern.Match(command);

            //0 for mouse1, 1 for mouse2, true for pressed, false for released
            bool[] mousePressed = new bool[2]; 

            if (match.Success)
            {
                string commandName = match.Groups[1].Value;
                string argumentString = match.Groups[2].Value;
                string[] args;
                Regex textPattern = new Regex("\"([^\"]+)\"");

                ushort posX, posY;
                byte mouseButton;

                switch (commandName)
                {
                    case "MousePress":
                        args = argumentString.Split(',');
                        if (args.Length != 3)
                        {
                            throw new ArgumentException("Syntax error: 'MousePress' command usage: MousePress(uInt16 posX, uInt16 posY, byte button)");
                        }
                        args[0] = args[0].Replace(" ", "").Replace("\"", "");
                        args[1] = args[1].Replace(" ", "").Replace("\"", "");
                        args[2] = args[2].Replace(" ", "").Replace("\"", "");
                        try
                        {
                            posX = UInt16.Parse(args[0]);
                            posY = UInt16.Parse(args[1]);
                            mouseButton = Byte.Parse(args[2]);
                            if (mouseButton != 1 && mouseButton != 2)
                            {
                                throw new ArgumentException("Error parsing MousePress: Invalid mouse button: " + mouseButton);
                            }
                            if (mousePressed[mouseButton - 1])
                            {
                                throw new ArgumentException("Error parsing MousePress: Specified button not released yet!");
                            }
                            mousePressed[mouseButton - 1] = !mousePressed[mouseButton - 1];
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException("Syntax error: 'MousePress' command usage: MousePress(uInt16 posX, uInt16 posY, byte button)", ex);
                        }
                        return new MousePressCommand(posX, posY, mouseButton);
                    case "MouseRelease":
                        args = argumentString.Split(',');
                        if (args.Length != 3)
                        {
                            throw new ArgumentException("Syntax error: 'MouseRelease' command usage: MouseRelease(uInt16 posX, uInt16 posY, byte button)");
                        }
                        args[0] = args[0].Replace(" ", "").Replace("\"", "");
                        args[1] = args[1].Replace(" ", "").Replace("\"", "");
                        args[2] = args[2].Replace(" ", "").Replace("\"", "");
                        
                        try
                        {
                            posX = UInt16.Parse(args[0]);
                            posY = UInt16.Parse(args[1]);
                            mouseButton = Byte.Parse(args[2]);
                            if (mouseButton != 1 && mouseButton != 2)
                            {
                                throw new ArgumentException("Error parsing MouseRelease: Invalid mouse button: " + mouseButton);
                            }
                            if (!mousePressed[mouseButton - 1])
                            {
                                throw new ArgumentException("Error parsing MousePress: Specified button not pressed yet!");
                            }
                            mousePressed[mouseButton - 1] = !mousePressed[mouseButton - 1];
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException("Syntax error: 'MouseRelease' command usage: MouseRelease(uInt16 posX, uInt16 posY, byte button)", ex);
                        }
                        return new MouseReleaseCommand(posX, posY, mouseButton);
                    case "Send":
                        if(textPattern.Matches(argumentString).Count > 1)
                        {
                            throw new ArgumentException("Syntax error: 'Send' command usage: Send(string text)");
                        }
                        string text = textPattern.Match(argumentString).Groups[0].Value;
                        return new SendCommand(text);
                    case "SendInput":
                        if (textPattern.Matches(argumentString).Count > 1)
                        {
                            throw new ArgumentException("Syntax error: 'SendInput' command usage: SendInput(string keyCombo)");
                        }

                        string inputs = textPattern.Match(argumentString).Groups[0].Value;
                        foreach (string key in inputs.Split(' ')) //sanity check
                        {
                            try
                            {
                                VirtualKeys vKey = (VirtualKeys)Enum.Parse(typeof(VirtualKeys), key);
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentException("Error parsing SendInputCommand: Key not recognized: " + key, ex);
                            }
                        }
                        return new SendInputCommand(inputs);
                    case "MessageBox":
                        string message, title;
                        switch (textPattern.Matches(argumentString).Count)
                        {
                            case 1:
                                message = textPattern.Match(argumentString).Groups[0].Value;
                                return new MessageBoxCommand(message);
                            case 2:
                                message = textPattern.Match(argumentString).Groups[0].Value;
                                title = textPattern.Match(argumentString).Groups[1].Value;
                                return new MessageBoxCommand(message, title);
                            default:
                                throw new ArgumentException("Syntax error: 'MessageBox' command usage: MessageBox(string message[, string title])");
                        }
                    default:
                        throw new NotImplementedException("Syntax error: Command " + commandName + " not recognized!");
                }
            }
            else
            {
                throw new MalformedLineException("Syntax error: Command " +  command + " does not match klpp scripting standards!");
            }
        }
    }
}
