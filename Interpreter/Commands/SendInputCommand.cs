using InputListener;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Interpreter
{
    /// <summary>
    /// Command that sends keypresses to the eventQueue of the OS
    /// </summary>
    internal class SendInputCommand : IKlppCommand
    {
        private string _text;

        public SendInputCommand(string text)
        {
            _text = text;
        }

        /// <summary>
        /// Sends the keys given as keystrokes to the eventQueue of the OS
        /// </summary>
        /// <exception cref="ArgumentException">When given key does not exist</exception>
        public void Execute()
        {
            INPUT[] inputArray = new INPUT[_text.Length * 2]; //Worst case scenario is one modifier + many text keys, so plenty of space
            int index = 0;
            Stack<INPUT> releaseStack = new Stack<INPUT>(_text.Length / 3); // The only key releases that need to be loaded into the release stack are modifier keys Ctrl, Alt, Win and Shift
            string[] keys = _text.Split(' ');
            foreach (string key in keys)
            {
                //First check for modifiers as they need to be added to the releaseStack
                if (key.Equals("Ctrl") || key.Equals("LeftCtrl") || key.Equals("RightCtrl"))
                {
                    inputArray[index++] = new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0xA2, //LCTRL
                            wScan = 0,
                            dwFlags = 0
                        }
                    };
                    releaseStack.Push(new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0xA2, //LCTRL
                            wScan = 0,
                            dwFlags = KeyEvent.KETEVENTF_KEYUP
                        }
                    });
                }
                else if (key.Equals("Win") || key.Equals("LeftWin") || key.Equals("RightWin"))
                {
                    inputArray[index++] = new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0x5B, //LWIN
                            wScan = 0,
                            dwFlags = 0
                        }
                    };
                    releaseStack.Push(new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0x5B, //LWIN
                            wScan = 0,
                            dwFlags = KeyEvent.KETEVENTF_KEYUP
                        }
                    });
                }
                else if (key.Equals("Shift") || key.Equals("LeftShift") || key.Equals("RightShift"))
                {
                    inputArray[index++] = new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0xA0, //LSHIFT
                            wScan = 0,
                            dwFlags = 0
                        }
                    };
                    releaseStack.Push(new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0xA0, //LSHIFT
                            wScan = 0,
                            dwFlags = KeyEvent.KETEVENTF_KEYUP
                        }
                    });
                }
                else if (key.Equals("Alt") || key.Equals("LeftAlt") || key.Equals("RightAlt")) 
                {
                    inputArray[index++] = new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0xA4, //LALT
                            wScan = 0,
                            dwFlags = 0
                        }
                    };
                    releaseStack.Push(new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0xA4, //LALT
                            wScan = 0,
                            dwFlags = KeyEvent.KETEVENTF_KEYUP
                        }
                    });
                }
                //Second, check for digits as their VirtualKeys equivalent is not user-friendly
                else if (key.Length == 1 && '0' <= key[0] && '1' >= key[0])
                {
                    inputArray[index++] = new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = (short)key[0],
                            dwFlags = KeyEvent.KEYEVENTF_UNICODE
                        }
                    };
                    inputArray[index++] = new INPUT
                    {
                        type = InputType.INPUT_KEYBOARD,
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = (short)key[0],
                            dwFlags = KeyEvent.KEYEVENTF_UNICODE | KeyEvent.KETEVENTF_KEYUP
                        }
                    };
                }
                //Last, check for other keys
                else try
                    {
                        VirtualKeys vKey = (VirtualKeys)Enum.Parse(typeof(VirtualKeys), key);
                        inputArray[index++] = new INPUT
                        {
                            type = InputType.INPUT_KEYBOARD,
                            ki = new KEYBDINPUT
                            {
                                wVk = (short)vKey,
                                wScan = 0,
                                dwFlags = 0
                            }
                        };
                        inputArray[index++] = new INPUT
                        {
                            type = InputType.INPUT_KEYBOARD,
                            ki = new KEYBDINPUT
                            {
                                wVk = (short)vKey,
                                wScan = 0,
                                dwFlags = KeyEvent.KETEVENTF_KEYUP
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Error executing SendInputCommand: Key not recognized: " + key, ex);
                    }
            }
            while(releaseStack.Count > 0)
            {
                inputArray[index++] = releaseStack.Pop();
            }
            LLInput.SendInput((uint)inputArray.Length, inputArray, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
