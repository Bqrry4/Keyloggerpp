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
        /// <returns></returns>
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

                switch (commandName)
                {
                    case "Click":
                        args = argumentString.Split(',');
                        if (args.Length != 2)
                        {
                            throw new ArgumentException("'Click' command accepts 2 integer arguments!");
                        }
                        args[0] = args[0].Replace(" ", "").Replace("\"", "");
                        args[1] = args[1].Replace(" ", "").Replace("\"", "");
                        int posX, posY;
                        try
                        {
                            posX = Int32.Parse(args[0]);
                            posY = Int32.Parse(args[1]);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        //return new ClickCommand(posX, posY);
                        break;
                    case "Send":
                        if(textPattern.Matches(argumentString).Count > 1)
                        {
                            throw new ArgumentException("'Send' command accepts only one string argument!");
                        }
                        string inputs = textPattern.Match(argumentString).Groups[0].Value;
                        //return new SendCommand(inputs);
                        break;
                    case "MessageBox":
                        string message, title;
                        switch (textPattern.Matches(argumentString).Count)
                        {
                            case 1:
                                message = textPattern.Match(argumentString).Groups[0].Value;
                                //return new MessageBoxCommand(message);
                                break;
                            case 2:
                                message = textPattern.Match(argumentString).Groups[0].Value;
                                title = textPattern.Match(argumentString).Groups[1].Value;
                                //return new MessageBoxCommand(message, title);
                                break;
                            default:
                                throw new ArgumentException("'MessageBox' command accepts one or two string arguments!");
                        }
                        break;
                    default:
                        throw new NotImplementedException("Command not implemented!");
                }
            }
            else
            {
                //throw new MalformedLineException();
            }
        }
    }
}
