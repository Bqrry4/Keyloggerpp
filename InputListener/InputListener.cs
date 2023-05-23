using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Input;
using static InputListener.LLInput;

namespace InputListener
{
    public class InputListener
    {
        private IntPtr _hookedKeyboard;
        private IntPtr _hookedMouse;

        private List<KeyEventData> _currentKeyEvent = new List<KeyEventData>(2);


        public InputListener()
        {
            _hookedKeyboard = SetHook(LowLevelKeyboardProc, HookType.WH_KEYBOARD_LL);
            _hookedMouse = SetHook(LowLevelMouseProc, HookType.WH_MOUSE_LL);
        }

        ~InputListener()
        {
            UnhookWindowsHookEx(_hookedKeyboard);
            UnhookWindowsHookEx(_hookedMouse);
        }

        /// <summary>
        /// A ProcessHookMessage callback that process llKeyboard messages
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LowLevelKeyboardProc(MessageType msT, IntPtr data)
        {
            KBDLLHOOKSTRUCT dataStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(data, typeof(KBDLLHOOKSTRUCT));
            //Key key = KeyInterop.KeyFromVirtualKey((int)dataStruct.vkCode);

            //LLInput.VKCodeToString(dataStruct.vkCode, msT == MessageType.WM_KEYDOWN);

            switch (msT)
            {
                case MessageType.WM_SYSKEYDOWN:
                case MessageType.WM_KEYDOWN:


                    //Press interrupted by another press event
                    if (_currentKeyEvent.Count > 0)
                    {
                        if (_currentKeyEvent.First().vkCode == dataStruct.vkCode)
                        {
                            if (_currentKeyEvent.First().count < 255)
                            {
                                _currentKeyEvent.First().count++;
                                break;
                            }
                            else
                            {
                                //Batch 255 duplicate presses
                                goto ProcessEvent;

                            }
                        }
                        else
                        {
                            //Adding the new event
                            _currentKeyEvent.Add(new KeyEventData(dataStruct.vkCode, GetCharsFromVKCode(dataStruct.vkCode, dataStruct.scanCode), 1));

                            goto ProcessEvent;
                        }
                    }

                    _currentKeyEvent.Add(new KeyEventData(dataStruct.vkCode, GetCharsFromVKCode(dataStruct.vkCode, dataStruct.scanCode), 1));


                    break;
                case MessageType.WM_SYSKEYUP:
                case MessageType.WM_KEYUP:
                ProcessEvent:

                    //UpPress event pair for an already processed event
                    if (_currentKeyEvent.Count == 0)
                    {
                        break;
                    }


                    //sadfpojasfj PROCESS
                    KeyEventData pp = _currentKeyEvent.First();
                    // Console.WriteLine(_currentKeyEvent.First().count);
                    _currentKeyEvent.RemoveAt(0);

                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// A ProcessHookMessage callback that process llMouse messages
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LowLevelMouseProc(MessageType msT, IntPtr data)
        {
            MSLLHOOKSTRUCT dataStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(data, typeof(MSLLHOOKSTRUCT));

            MouseEventData msE = new MouseEventData();
            msE.coords = dataStruct.pt;

            switch (msT)
            {

                case MessageType.WM_LBUTTONDOWN:
                    msE.buttonID = 0;
                    msE.status = false;

                    break;
                case MessageType.WM_LBUTTONUP:
                    msE.buttonID = 0;
                    msE.status = true;

                    break;

                case MessageType.WM_RBUTTONDOWN:
                    msE.buttonID = 1;
                    msE.status = false;

                    break;
                case MessageType.WM_RBUTTONUP:
                    msE.status = true;
                    msE.buttonID = 1;

                    break;
            }

            //PostEvent
        }
    }

    public abstract class LLEventData
    {
        /// <summary>
        /// 0 = KeyEventData
        /// 1 = MouseEventData
        /// </summary>
        public int eType;
    }

    public class KeyEventData : LLEventData
    {
        public uint vkCode;
        //The unicode character
        public string uChar;
        //How many times it was multiplied when the key was pressed
        public byte count;

        public KeyEventData()
        {
            eType = 0;

        }
        public KeyEventData(uint vkCode, string uChar, byte count) : this()
        {
            this.vkCode = vkCode;
            this.uChar = uChar;
            this.count = count;
        }
    }

    public class MouseEventData : LLEventData
    {
        public Point coords;

        public int buttonID;
        /// <summary>
        /// 0 = Press | 1 = Release
        /// </summary>
        public bool status;

        public MouseEventData()
        {
            eType = 1;
        }

        public MouseEventData(Point coords, int buttonID, bool status) : this()
        {

            this.coords = coords;
            this.buttonID = buttonID;
            this.status = status;

        }

    }
}
