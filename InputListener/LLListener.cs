/**************************************************************************
*                                                                        *
*  File:        LLListener.cs                                            *
*  Copyright:   (c)Paniș Alexandru                                       *
*               @Kakerou_CLUB                                            *
*  Description: Class that encapsulates the capturing of low level       *
*               input and sending events to a queue given in the         *
*               constructor.                                             *
*                                                                        *
**************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static InputListener.LLInput;

namespace InputListener
{
    /// <summary>
    /// Class that encapsulates the capturing of low level input and sending events to a queue given in the constructor
    /// </summary>
    public class LLListener
    {
        //The eventQueue where the valid events are pushed
        private ConcurrentQueue<LLEventData> _eventQueue = new ConcurrentQueue<LLEventData>();


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

        public LLListener(ConcurrentQueue<LLEventData> eventQueue)
        {
            _eventQueue = eventQueue;
        }
        /// <summary>
        /// Set hooks over ll input
        /// </summary>
        public void StartListening()
        {
            _hookedKeyboard = SetHook(LowLevelKeyboardProc, HookType.WH_KEYBOARD_LL);
            _hookedMouse = SetHook(LowLevelMouseProc, HookType.WH_MOUSE_LL);
        }
        /// <summary>
        /// Unhook to stop recieving callbacks
        /// </summary>
        public void StopListening()
        {
            UnhookWindowsHookEx(_hookedKeyboard);
            UnhookWindowsHookEx(_hookedMouse);
        }

        ~LLListener()
        {
            StopListening();
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
                        case VirtualKeys.LeftWin:
                        case VirtualKeys.RightWin:
                            _activeModifiers |= 0x1;
                            break;

                        case VirtualKeys.LeftAlt:
                        case VirtualKeys.RightAlt:
                            _activeModifiers |= 0x2;
                            break;

                        case VirtualKeys.LeftShift:
                        case VirtualKeys.RightShift:
                            _activeModifiers |= 0x4;
                            break;

                        case VirtualKeys.LeftCtrl:
                        case VirtualKeys.RightCtrl:
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

                        case VirtualKeys.LeftWin:
                        case VirtualKeys.RightWin:
                            ushort mask;
                            mask = 0x1;
                            goto ClearModifierMask;
                        case VirtualKeys.LeftAlt:
                        case VirtualKeys.RightAlt:
                            mask = 0x2;
                            goto ClearModifierMask;
                        case VirtualKeys.LeftShift:
                        case VirtualKeys.RightShift:
                            mask = 0x4;
                            goto ClearModifierMask;
                        case VirtualKeys.LeftCtrl:
                        case VirtualKeys.RightCtrl:
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
                    KeyEventData kbE = _currentKeyEvent.First();
                    _currentKeyEvent.RemoveAt(0);

                    //If it has a modifier, it must be a combination
                    List<uint> modifersList = new List<uint>();
                    if (_activeModifiers > 0)
                    {
                        //WinKey is on?
                        if ((_activeModifiers & 0x1) != 0)
                        {
                            modifersList.Add((uint)VirtualKeys.LeftWin);
                        }

                        //Ctrl is on?
                        if ((_activeModifiers & 0x8) != 0)
                        {
                            modifersList.Add((uint)VirtualKeys.LeftCtrl);

                        }
                        //Alt is on?
                        if ((_activeModifiers & 0x2) != 0)
                        {
                            modifersList.Add((uint)VirtualKeys.LeftAlt);

                        }

                        //Shift is on?
                        if ((_activeModifiers & 0x4) != 0)
                        {
                            //Shift alone is not a valid combination
                            if (modifersList.Count > 0)
                            {
                                modifersList.Add((uint)VirtualKeys.LeftShift);
                            }
                        }
                    }

                    if (modifersList.Count > 0)
                    {

                        //If the last key is a modifier throw it away (Invalid combination)
                        switch ((VirtualKeys)kbE.vkCode)
                        {
                            case VirtualKeys.LeftWin:
                            case VirtualKeys.RightWin:
                            case VirtualKeys.LeftAlt:
                            case VirtualKeys.RightAlt:
                            case VirtualKeys.LeftShift:
                            case VirtualKeys.RightShift:
                            case VirtualKeys.LeftCtrl:
                            case VirtualKeys.RightCtrl:

                                //throw 
                                return;
                        }
                        //Push Event to the queue
                        _eventQueue.Enqueue(new LLEventData
                        {
                            eType = 2,
                            cEvent = new KeyCombination
                            {
                                modifers = modifersList,
                                key = kbE
                            }
                        });
                    }
                    else
                    {
                        //Push Event to the queue
                        _eventQueue.Enqueue(new LLEventData
                        {
                            eType = 0,
                            kEvent = kbE
                        });
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

            //Push Event to the queue
            _eventQueue.Enqueue(new LLEventData
            {
                eType = 1,
                mEvent= msE
            });
        }
    }

    [StructLayout(LayoutKind.Explicit)]
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
        public List<uint> modifers;
        public KeyEventData key;
    }
}
