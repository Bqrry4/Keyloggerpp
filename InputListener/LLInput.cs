using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InputListener
{
    /// <summary>
    /// Incapsulate the winApi part for low level input
    /// </summary>
    public static class LLInput
    {
        /// <summary>
        /// A callback procedure that must process the messages catched by the hook
        /// </summary>
        /// <param name="msT"></param>
        /// <param name="data">A structure of type MSLLHOOKSTRUCT or KBDLLHOOKSTRUCT depends what type of hook was registered</param>
        public delegate void ProcessHookMessage(MessageType msT, IntPtr data);

        //Callback function prototype for SetWindowsHookEx
        private delegate IntPtr HookCallback(int nCode, UIntPtr wParam, IntPtr lParam);

        public enum MessageType : int
        {
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,

            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        public enum HookType : int
        {
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }


        /// <summary>
        /// Set the hook to capture low level input from mouse and keyboard
        /// </summary>
        /// <param name="proc">A function to process the message</param>
        /// <param name="hookType"></param>
        /// <returns>A pointer to registered hook, it must be freed with Unhook function</returns>
        public static IntPtr SetHook(ProcessHookMessage proc, HookType hookType)
        {

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx((int)hookType,
                (int nCode, UIntPtr wParam, IntPtr lParam) =>
                {
                    //If nCode >=0, the wParam and lParam parameters contain information about a keyboard message.
                    if (nCode >= 0)
                    {
                        MessageType msT = (MessageType)wParam.ToUInt32();
                        proc(msT, lParam);

                    }

                    return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
                },
                GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookCallback lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, UIntPtr wParam, IntPtr lParam);

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        int X;
        int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public Point pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }
}
