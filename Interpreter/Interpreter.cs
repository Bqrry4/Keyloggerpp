using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Interpreter
{
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
            //Step 2: Read the script line by line and construct Command objects (maybe a CommandFactory?).
            //Step 3: Repeat until end of string.

            Regex hotkeyPattern = new Regex("(.+)::\\n({\\n(?:\\s{4}.+\\n)+})");

            Match hotkey = hotkeyPattern.Match(script);

            while (hotkey.Success)
            {
                hotkeys.Append(hotkey.Groups[1].Value);
                string[] lines = hotkey.Groups[2].Value.Split('\n');
                List<IKlppCommand> hotkeyCommandList = new List<IKlppCommand>();

                foreach(string line in lines)
                {
                    //TODO: CommandFactory?
                    //IKlppCommand  command = _commandFactory.parse(line);
                    //hotkeyCommandList.Add(command);
                }

                _hotkeyScripts.Add(hotkey.Groups[1].Value, hotkeyCommandList);
            }
        }
        /// <summary>
        /// This method will be called by the Intermediary when it detects that the given hotkey was pressed.
        /// </summary>
        /// <param name="hotkey">String representing the trigger</param>
        public void Run(in string hotkey)
        {
            List<IKlppCommand> commandList;
            if(!_hotkeyScripts.TryGetValue(hotkey, out commandList))
            {
                //throw new HotkeyNotFoundException;
            }
            foreach(IKlppCommand command in commandList)
            {
                command.Execute();
            }
        }
    }
}
