using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Input;
using static InputListener.LLInput;

namespace InputListener
{
    public class InputListener
    {
        private IntPtr hookedKeyboard;
        private IntPtr hookedMouse;
        public InputListener()
        {
            hookedKeyboard = SetHook(LowLevelKeyboardProc, HookType.WH_KEYBOARD_LL);
            hookedMouse = SetHook(LowLevelMouseProc, HookType.WH_MOUSE_LL);
        }

        ~InputListener()
        {
            UnhookWindowsHookEx(hookedKeyboard);
            UnhookWindowsHookEx(hookedMouse);
        }

        /// <summary>
        /// A ProcessHookMessage callback that process llKeyboard messages
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LowLevelKeyboardProc(MessageType msT, IntPtr data)
        {
            KBDLLHOOKSTRUCT dataStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(data, typeof(KBDLLHOOKSTRUCT));

            switch (msT)
            {
                case MessageType.WM_KEYDOWN:

                    break;
                case MessageType.WM_KEYUP:

                    break;
                case MessageType.WM_SYSKEYDOWN:

                    break;
                case MessageType.WM_SYSKEYUP:

                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// A ProcessHookMessage callback that process llMouse messages
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LowLevelMouseProc(MessageType msT, IntPtr data)
        {
            MSLLHOOKSTRUCT dataStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(data, typeof(MSLLHOOKSTRUCT));

            switch (msT)
            {
                case MessageType.WM_RBUTTONDOWN:

                    break;
                case MessageType.WM_RBUTTONUP:

                    break;
                case MessageType.WM_LBUTTONDOWN:

                    break;
                case MessageType.WM_LBUTTONUP:

                    break;
            }
        }
    }

    public class LLEventData
    {
        public int type;
    }

    public class KeyEventData : LLEventData
    {
        public Key key;
        //How many times it was multiplied when the key was pressed
        public int count;
    }

    public class MouseEventData : LLEventData
    {
        public Point choords;

    }
}
