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

        /// <summary>
        /// Win = 0x1
        /// Alt = 0x2
        /// Shift = 0x4
        /// Ctrl = 0x8
        /// </summary>
        private ushort _activeModifiers = 0x0;

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

            switch (msT)
            {
                case MessageType.WM_SYSKEYDOWN:
                case MessageType.WM_KEYDOWN:


                    //Looking for modifiers
                    switch ((VirtualKeys)dataStruct.vkCode)
                    {
                        case VirtualKeys.LeftWindows:
                        case VirtualKeys.RightWindows:
                            _activeModifiers |= 0x1;
                            break;

                        case VirtualKeys.LeftMenu:
                        case VirtualKeys.RightMenu:
                            _activeModifiers |= 0x2;
                            break;

                        case VirtualKeys.LeftShift:
                        case VirtualKeys.RightShift:
                            _activeModifiers |= 0x4;
                            break;

                        case VirtualKeys.LeftControl:
                        case VirtualKeys.RightControl:
                            _activeModifiers |= 0x8;
                            break;
                            //If it will be needed, i will extend it for separate right and left combination in further releases
                    }


                    //Press interrupted by another press event
                    if (_currentKeyEvent.Count > 0)
                    {
                        //Its a duplicate for the same event
                        if (_currentKeyEvent.First().vkCode == dataStruct.vkCode)
                        {
                            if (_currentKeyEvent.First().count < 255)
                            {
                                //Increase the count for duplicates when long down press
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

                            //Process the old event which was interrupted by the new one (forcing the event)
                            goto ProcessEvent;
                        }
                    }
                    //Append the event to list
                    _currentKeyEvent.Add(new KeyEventData
                    {
                        vkCode = dataStruct.vkCode,
                        uChar = GetCharsFromVKCode(dataStruct.vkCode, dataStruct.scanCode),
                        count = 1
                    });

                    break;


                case MessageType.WM_SYSKEYUP:
                case MessageType.WM_KEYUP:

                    //Looking if it is a modifier
                    switch ((VirtualKeys)dataStruct.vkCode)
                    {

                        case VirtualKeys.LeftWindows:
                        case VirtualKeys.RightWindows:
                            ushort mask;
                            mask = 0x1;
                            goto ClearModifierMask;
                        case VirtualKeys.LeftMenu:
                        case VirtualKeys.RightMenu:
                            mask = 0x2;
                            goto ClearModifierMask;
                        case VirtualKeys.LeftShift:
                        case VirtualKeys.RightShift:
                            mask = 0x4;
                            goto ClearModifierMask;
                        case VirtualKeys.LeftControl:
                        case VirtualKeys.RightControl:
                            mask = 0x8;

                        ClearModifierMask:
                            //Clear the mask if it was released (keyUp event)
                            _activeModifiers &= (ushort)~mask;

                            break;
                    }


                    //UpPress event pair for an already processed event
                    if (_currentKeyEvent.Count == 0)
                    {
                        //If its not a modifier then its a pair for a normal key
                        break;

                    }

                //Label for a forced event
                ProcessEvent:

                    //Add to an eventQ
                    KeyEventData kEvent = _currentKeyEvent.First();
                    _currentKeyEvent.RemoveAt(0);

                    //If it has a modifier, it must be a combination
                    List<uint> modifers = new List<uint>();
                    if (_activeModifiers > 0)
                    {
                        //WinKey is on?
                        if ((_activeModifiers & 0x1) != 0)
                        {
                            modifers.Add((uint)VirtualKeys.LeftWindows);
                        }

                        //Ctrl is on?
                        if ((_activeModifiers & 0x8) != 0)
                        {
                            modifers.Add((uint)VirtualKeys.LeftControl);

                        }
                        //Alt is on?
                        if ((_activeModifiers & 0x2) != 0)
                        {
                            modifers.Add((uint)VirtualKeys.LeftMenu);

                        }

                        //Shift is on?
                        if ((_activeModifiers & 0x4) != 0)
                        {
                            //Shift alone is not a valid combination
                            if (modifers.Count > 0)
                            {
                                modifers.Add((uint)VirtualKeys.LeftShift);
                            }
                        }
                    }

                    if (modifers.Count > 0)
                    {

                        //If the last key is a modifier throw it away (Invalid combination)
                        switch ((VirtualKeys)kEvent.vkCode)
                        {
                            case VirtualKeys.LeftWindows:
                            case VirtualKeys.RightWindows:
                            case VirtualKeys.LeftMenu:
                            case VirtualKeys.RightMenu:
                            case VirtualKeys.LeftShift:
                            case VirtualKeys.RightShift:
                            case VirtualKeys.LeftControl:
                            case VirtualKeys.RightControl:

                                //throw 
                                return;
                        }
                        foreach (uint c in modifers)
                        {
                            Console.WriteLine((VirtualKeys)(c));
                        }
                        Console.WriteLine((VirtualKeys)kEvent.vkCode);

                    }
                    else
                    {
                        Console.WriteLine((VirtualKeys)kEvent.vkCode);

                    }

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
        /// 2 = KeyCombinationEvent
        /// </summary>
        [FieldOffset(0)]
        public int eType;

        [FieldOffset(sizeof(int))]
        public KeyEventData kEvent;

        [FieldOffset(sizeof(int))]
        public MouseEventData mEvent;

        [FieldOffset(sizeof(int))]
        public KeyCombination cEvent;

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

    public class KeyCombination
    {
        List<uint> modifers;
        KeyEventData key;
    }
}
