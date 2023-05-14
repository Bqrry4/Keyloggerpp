using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InputListener
{
    /// <summary>
    /// Incapsulate the winApi part for low level input
    /// </summary>
    internal static class LLInput
    {
        public delegate IntPtr HookCallback(int nCode, UIntPtr wParam, IntPtr lParam);

        public enum KeyEvent : int
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


        public static IntPtr SetHook(HookCallback proc, HookType idHook)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx((int)idHook, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookCallback lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, UIntPtr wParam, IntPtr lParam);

    }
}
