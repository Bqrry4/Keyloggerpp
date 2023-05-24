using InputListener;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Interpreter
{
    /// <summary>
    /// Command that sends modified keypresses
    /// </summary>
    internal class SendInputCommand : IKlppCommand
    {
        private string _text;

        public SendInputCommand(string text)
        {
            _text = text;
        }

        public void Execute()
        {
            //TODO: SEND MODIFIED KEY
            INPUT[] inputArray = new INPUT[_text.Length * 2];
            int index = 0;
            Stack<INPUT> releaseStack = new Stack<INPUT>(_text.Length);
            string[] keys = _text.Split(' ');
            foreach (string key in keys)
            {
                //First check for modifiers
                if (key.Contains("Ctrl"))
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
                else if (key.Contains("Win"))
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
                else if (key.Contains("Shift"))
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
                else if (key.Contains("Alt"))
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
                //Second, check for regular, non-OEM keys 
                else if (('a' <= key[0] && 'z' >= key[0]) || ('0' <= key[0] && '1' >= key[0]))
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
                //Last, check for other special keys, like Tab
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
                    catch (Exception exc)
                    {
                        throw exc;
                        //do something lmao
                    }

            }
            while(releaseStack.Count > 0)
            {
                inputArray[index++] = releaseStack.Pop();
            }
            LLInput.SendInput((uint)inputArray.Length, inputArray, Marshal.SizeOf(typeof(INPUT)));
            //throw new NotImplementedException();
        }
    }
}
