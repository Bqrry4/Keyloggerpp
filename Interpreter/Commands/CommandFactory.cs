using Interpreter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter
{
    internal class CommandFactory
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
                            throw new ArgumentException("Syntax error: 'MousePress' command accepts 2 integer and 1 byte arguments!");
                        }
                        args[0] = args[0].Replace(" ", "").Replace("\"", "");
                        args[1] = args[1].Replace(" ", "").Replace("\"", "");
                        args[2] = args[2].Replace(" ", "").Replace("\"", "");
                        try
                        {
                            posX = UInt16.Parse(args[0]);
                            posY = UInt16.Parse(args[1]);
                            mouseButton = Byte.Parse(args[2]);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        return new MousePressCommand(posX, posY, mouseButton);
                        //break;
                    case "ClickRelease":
                        args = argumentString.Split(',');
                        if (args.Length != 3)
                        {
                            throw new ArgumentException("Syntax error: 'Click' command accepts 3 integer arguments!");
                        }
                        args[0] = args[0].Replace(" ", "").Replace("\"", "");
                        args[1] = args[1].Replace(" ", "").Replace("\"", "");
                        args[2] = args[2].Replace(" ", "").Replace("\"", "");
                        
                        try
                        {
                            posX = UInt16.Parse(args[0]);
                            posY = UInt16.Parse(args[1]);
                            mouseButton = Byte.Parse(args[2]);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        return new MouseReleaseCommand(posX, posY, mouseButton);
                    //break;
                    case "Send":
                        if(textPattern.Matches(argumentString).Count > 1)
                        {
                            throw new ArgumentException("Syntax error: 'Send' command accepts only one string argument!");
                        }
                        string text = textPattern.Match(argumentString).Groups[0].Value;
                        return new SendCommand(text);
                        //break;
                    case "SendInput":
                        if (textPattern.Matches(argumentString).Count > 1)
                        {
                            throw new ArgumentException("Syntax error: 'SendInput' command accepts only one string argument!");
                        }
                        string inputs = textPattern.Match(argumentString).Groups[0].Value;
                        return new SendInputCommand(inputs);
                        //break;
                    case "MessageBox":
                        string message, title;
                        switch (textPattern.Matches(argumentString).Count)
                        {
                            case 1:
                                message = textPattern.Match(argumentString).Groups[0].Value;
                                return new MessageBoxCommand(message);
                                //break;
                            case 2:
                                message = textPattern.Match(argumentString).Groups[0].Value;
                                title = textPattern.Match(argumentString).Groups[1].Value;
                                return new MessageBoxCommand(message, title);
                                //break;
                            default:
                                throw new ArgumentException("Syntax error: 'MessageBox' command accepts one or two string arguments!");
                        }
                        //break;
                    default:
                        throw new NotImplementedException("Command " + commandName + " not implemented!");
                }
            }
            else
            {
                throw new MalformedLineException("Syntax error: " +  command);
            }
        }
    }
}
