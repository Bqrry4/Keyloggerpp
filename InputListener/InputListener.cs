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
                            _currentKeyEvent.Add(new KeyEventData
                            {
                                vkCode = dataStruct.vkCode,
                                uChar = GetCharsFromVKCode(dataStruct.vkCode, dataStruct.scanCode),
                                count = 1
                            });

                            goto ProcessEvent;
                        }
                    }

                    _currentKeyEvent.Add(new KeyEventData
                    {
                        vkCode = dataStruct.vkCode,
                        uChar = GetCharsFromVKCode(dataStruct.vkCode, dataStruct.scanCode),
                        count = 1
                    });



                    break;
                case MessageType.WM_SYSKEYUP:
                case MessageType.WM_KEYUP:
                ProcessEvent:

                    //UpPress event pair for an already processed event
                    if (_currentKeyEvent.Count == 0)
                    {
                        //throw it away
                        break;
                    }


                    //PROCESS event
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

            //PostEvent 3
        }
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct LLEventData
    {
        /// <summary>
        /// 0 = KeyEventData
        /// 1 = MouseEventData
        /// </summary>
        [FieldOffset(0)]
        public int eType;

        [FieldOffset(sizeof(int))]
        public KeyEventData kEvent;

        [FieldOffset(sizeof(int))]
        public MouseEventData mEvent;

    }

    public class KeyEventData
    {
        //the virtual key code
        public uint vkCode;
        //The unicode character
        public string uChar;
        //How many times it was multiplied when the key was pressed
        public byte count;
    }

    public class MouseEventData
    {
        //Screen coordinates where the mouse was pointing
        public Point coords;
        /// <summary>
        /// 0 = LeftClick 
        /// 1 = RightClick 
        /// </summary>
        public int buttonID;
        /// <summary>
        /// 0 = Press | 1 = Release
        /// </summary>
        public bool status;
    }
}
