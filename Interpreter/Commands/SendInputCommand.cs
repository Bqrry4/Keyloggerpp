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
            for (int i = 0; i < _text.Length; i++)
            {
                if (_text.Substring(i).StartsWith("Ctrl"))
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
                else if(_text.Substring(i).StartsWith("Win"))
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
                else if(_text.Substring(i).StartsWith("Shift"))
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
                else if (_text.Substring(i).StartsWith("Alt"))
                {

                }

                inputArray[2 * i] = new INPUT
                {
                    type = InputType.INPUT_KEYBOARD,
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = (short)_text[i],
                        dwFlags = KeyEvent.KEYEVENTF_UNICODE
                    }
                };
                inputArray[2 * i + i] = new INPUT
                {
                    type = InputType.INPUT_KEYBOARD,
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = (short)_text[i],
                        dwFlags = KeyEvent.KEYEVENTF_UNICODE | KeyEvent.KETEVENTF_KEYUP
                    }
                };
            }
            LLInput.SendInput((uint)inputArray.Length, inputArray, Marshal.SizeOf(typeof(INPUT)));
            //throw new NotImplementedException();
        }
    }
}
