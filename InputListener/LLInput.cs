using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;

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

        #region privateImports

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookCallback lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, UIntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll")]
        private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int vkey);


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetKeyboardLayout(uint dwLayout);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();
        #endregion

        /// <summary>
        /// Returns the Unicode character represented by the current keyboard state
        /// </summary>
        /// <param name="vkCode"></param>
        /// <returns></returns>
        public static string GetCharsFromVKCode(uint vkCode, uint scanCode)
        {

            var buf = new StringBuilder(8);
            var keyboardState = new byte[256];

            // Gets the current windows window handle, threadID, processID
            IntPtr currentHWnd = GetForegroundWindow();
            uint currentWindowThreadID = GetWindowThreadProcessId(currentHWnd, out _);

            // This programs Thread ID
            uint thisProgramThreadId = GetCurrentThreadId();


            bool isAttachedThread = false;
            // Attach to active thread so we can get the most late state of the keyboard
            if (AttachThreadInput(thisProgramThreadId, currentWindowThreadID, true))
            {
                isAttachedThread = true;

            }

            //Seems there is a bug, so this is needed so GetKeyboardState can work correctly
            GetKeyState(0);

            //Retrieve the keyboardState and return an empty string on fail
            if (!GetKeyboardState(keyboardState))
            {
                //Also we must detach that thread
                if (isAttachedThread)
                {
                    // Detach
                    AttachThreadInput(thisProgramThreadId, currentWindowThreadID, false);
                }

                return "";
            }

            //If a thread was attached now we can detach
            if (isAttachedThread)
            {
                // Detach
                AttachThreadInput(thisProgramThreadId, currentWindowThreadID, false);

            }

            // Gets the layout of keyboard
            IntPtr hkl = GetKeyboardLayout(currentWindowThreadID);

            ToUnicodeEx(vkCode, scanCode, keyboardState, buf, buf.Capacity, 0, hkl);
            return buf.ToString();
        }


        /// <summary>
        /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput
        /// </summary>
        [DllImport("user32.dll")]
        public static extern uint SendInput(uint cInputs, INPUT[] pInputs, int cbSize);

    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msllhookstruct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public Point pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-kbdllhookstruct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-input
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
        /// <summary>
        /// 0 = INPUT_MOUSE
        /// 1 = INPUT_KEYBOARD
        /// 2 = INPUT_HARDWARE
        /// </summary>
        [FieldOffset(0)]
        public InputType type;

        [FieldOffset(sizeof(int))]
        public MOUSEINPUT mi;

        [FieldOffset(sizeof(int))]
        public KEYBDINPUT ki;

        [FieldOffset(sizeof(int))]
        public HARDWAREINPUT hi;
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-mouseinput
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public long dx;
        public long dy;
        public int mouseData;
        public int dwFlags;
        public int time;
        public UIntPtr dwExtraInfo;
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-keybdinput
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public short wVk;
        public short wScan;
        public KeyEvent dwFlags;
        public int time;
        public UIntPtr dwExtraInfo;
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-hardwareinput
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }

    public enum InputType : int
    {
        INPUT_MOUSE = 0,
        INPUT_KEYBOARD = 1,
        INPUT_HARDWARE = 2
    }
    
    public enum KeyEvent : int
    {
        KEYEVENTF_EXTENDEDKEY = 1,
        KETEVENTF_KEYUP = 2,
        KEYEVENTF_SCANCODE = 3,
        KEYEVENTF_UNICODE = 4
    }
}
