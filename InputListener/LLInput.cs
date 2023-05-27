/**************************************************************************
*                                                                        *
*  File:        LLInput.cs                                               *
*  Copyright:   (c)Paniș Alexandru                                       *
*               @Kakerou_CLUB                                            *
*  Description: Contains definitions and a little logic for using        *
*               winApi for low level input.                              *
*                                                                        *
**************************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

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

        [DllImport("user32.dll")] 
        private static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")] 
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);

        [DllImport("user32.dll")]
        internal static extern bool GetMessage(ref MSG lpMsg, IntPtr hWnd, uint mMsgFilterInMain, uint mMsgFilterMax);

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

    public enum MessageType : int
    {
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,

        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,

        WM_HOTKEY = 0x0312
    }

    public enum HookType : int
    {
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
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
    [StructLayout(LayoutKind.Explicit)]
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

    /// <summary>
    /// Enumeration for virtual keys.
    /// </summary>
    public enum VirtualKeys
        : ushort
    {
        LeftButton = 0x01,
        RightButton = 0x02,
        Cancel = 0x03,
        MiddleButton = 0x04,
        ExtraButton1 = 0x05,
        ExtraButton2 = 0x06,
        Backspace = 0x08,
        Tab = 0x09,
        Clear = 0x0C,
        Enter = 0x0D,
        Shift = 0x10,
        Ctrl = 0x11,
        /// <summary></summary>
        Alt = 0x12,
        /// <summary></summary>
        Pause = 0x13,
        /// <summary></summary>
        CapsLock = 0x14,
        /// <summary></summary>
        Esc = 0x1B,
        /// <summary></summary>
        Convert = 0x1C,
        /// <summary></summary>
        NonConvert = 0x1D,
        /// <summary></summary>
        Accept = 0x1E,
        /// <summary></summary>
        ModeChange = 0x1F,
        /// <summary></summary>
        Space = 0x20,
        /// <summary></summary>
        Prior = 0x21,
        /// <summary></summary>
        Next = 0x22,
        /// <summary></summary>
        End = 0x23,
        /// <summary></summary>
        Home = 0x24,
        /// <summary></summary>
        Left = 0x25,
        /// <summary></summary>
        Up = 0x26,
        /// <summary></summary>
        Right = 0x27,
        /// <summary></summary>
        Down = 0x28,
        /// <summary></summary>
        Select = 0x29,
        /// <summary></summary>
        Print = 0x2A,
        /// <summary></summary>
        Execute = 0x2B,
        /// <summary></summary>
        Snapshot = 0x2C,
        /// <summary></summary>
        Insert = 0x2D,
        /// <summary></summary>
        Delete = 0x2E,
        /// <summary></summary>
        Help = 0x2F,
        /// <summary></summary>
        N0 = 0x30,
        /// <summary></summary>
        N1 = 0x31,
        /// <summary></summary>
        N2 = 0x32,
        /// <summary></summary>
        N3 = 0x33,
        /// <summary></summary>
        N4 = 0x34,
        /// <summary></summary>
        N5 = 0x35,
        /// <summary></summary>
        N6 = 0x36,
        /// <summary></summary>
        N7 = 0x37,
        /// <summary></summary>
        N8 = 0x38,
        /// <summary></summary>
        N9 = 0x39,
        /// <summary></summary>
        A = 0x41,
        /// <summary></summary>
        B = 0x42,
        /// <summary></summary>
        C = 0x43,
        /// <summary></summary>
        D = 0x44,
        /// <summary></summary>
        E = 0x45,
        /// <summary></summary>
        F = 0x46,
        /// <summary></summary>
        G = 0x47,
        /// <summary></summary>
        H = 0x48,
        /// <summary></summary>
        I = 0x49,
        /// <summary></summary>
        J = 0x4A,
        /// <summary></summary>
        K = 0x4B,
        /// <summary></summary>
        L = 0x4C,
        /// <summary></summary>
        M = 0x4D,
        /// <summary></summary>
        N = 0x4E,
        /// <summary></summary>
        O = 0x4F,
        /// <summary></summary>
        P = 0x50,
        /// <summary></summary>
        Q = 0x51,
        /// <summary></summary>
        R = 0x52,
        /// <summary></summary>
        S = 0x53,
        /// <summary></summary>
        T = 0x54,
        /// <summary></summary>
        U = 0x55,
        /// <summary></summary>
        V = 0x56,
        /// <summary></summary>
        W = 0x57,
        /// <summary></summary>
        X = 0x58,
        /// <summary></summary>
        Y = 0x59,
        /// <summary></summary>
        Z = 0x5A,
        /// <summary></summary>
        LeftWin = 0x5B,
        /// <summary></summary>
        RightWin = 0x5C,
        /// <summary></summary>
        Application = 0x5D,
        /// <summary></summary>
        Sleep = 0x5F,
        /// <summary></summary>
        Numpad0 = 0x60,
        /// <summary></summary>
        Numpad1 = 0x61,
        /// <summary></summary>
        Numpad2 = 0x62,
        /// <summary></summary>
        Numpad3 = 0x63,
        /// <summary></summary>
        Numpad4 = 0x64,
        /// <summary></summary>
        Numpad5 = 0x65,
        /// <summary></summary>
        Numpad6 = 0x66,
        /// <summary></summary>
        Numpad7 = 0x67,
        /// <summary></summary>
        Numpad8 = 0x68,
        /// <summary></summary>
        Numpad9 = 0x69,
        /// <summary></summary>
        Multiply = 0x6A,
        /// <summary></summary>
        Add = 0x6B,
        /// <summary></summary>
        Separator = 0x6C,
        /// <summary></summary>
        Subtract = 0x6D,
        /// <summary></summary>
        Decimal = 0x6E,
        /// <summary></summary>
        Divide = 0x6F,
        /// <summary></summary>
        F1 = 0x70,
        /// <summary></summary>
        F2 = 0x71,
        /// <summary></summary>
        F3 = 0x72,
        /// <summary></summary>
        F4 = 0x73,
        /// <summary></summary>
        F5 = 0x74,
        /// <summary></summary>
        F6 = 0x75,
        /// <summary></summary>
        F7 = 0x76,
        /// <summary></summary>
        F8 = 0x77,
        /// <summary></summary>
        F9 = 0x78,
        /// <summary></summary>
        F10 = 0x79,
        /// <summary></summary>
        F11 = 0x7A,
        /// <summary></summary>
        F12 = 0x7B,
        /// <summary></summary>
        F13 = 0x7C,
        /// <summary></summary>
        F14 = 0x7D,
        /// <summary></summary>
        F15 = 0x7E,
        /// <summary></summary>
        F16 = 0x7F,
        /// <summary></summary>
        F17 = 0x80,
        /// <summary></summary>
        F18 = 0x81,
        /// <summary></summary>
        F19 = 0x82,
        /// <summary></summary>
        F20 = 0x83,
        /// <summary></summary>
        F21 = 0x84,
        /// <summary></summary>
        F22 = 0x85,
        /// <summary></summary>
        F23 = 0x86,
        /// <summary></summary>
        F24 = 0x87,
        /// <summary></summary>
        NumLock = 0x90,
        /// <summary></summary>
        ScrollLock = 0x91,
        /// <summary></summary>
        NEC_Equal = 0x92,
        /// <summary></summary>
        LeftShift = 0xA0,
        /// <summary></summary>
        RightShift = 0xA1,
        /// <summary></summary>
        LeftCtrl = 0xA2,
        /// <summary></summary>
        RightCtrl = 0xA3,
        /// <summary></summary>
        LeftAlt = 0xA4,
        /// <summary></summary>
        RightAlt = 0xA5,
        /// <summary></summary>
        BrowserBack = 0xA6,
        /// <summary></summary>
        BrowserForward = 0xA7,
        /// <summary></summary>
        BrowserRefresh = 0xA8,
        /// <summary></summary>
        BrowserStop = 0xA9,
        /// <summary></summary>
        BrowserSearch = 0xAA,
        /// <summary></summary>
        BrowserFavorites = 0xAB,
        /// <summary></summary>
        BrowserHome = 0xAC,
        /// <summary></summary>
        VolumeMute = 0xAD,
        /// <summary></summary>
        VolumeDown = 0xAE,
        /// <summary></summary>
        VolumeUp = 0xAF,
        /// <summary></summary>
        MediaNextTrack = 0xB0,
        /// <summary></summary>
        MediaPrevTrack = 0xB1,
        /// <summary></summary>
        MediaStop = 0xB2,
        /// <summary></summary>
        MediaPlayPause = 0xB3,
        /// <summary></summary>
        LaunchMail = 0xB4,
        /// <summary></summary>
        LaunchMediaSelect = 0xB5,
        /// <summary></summary>
        LaunchApplication1 = 0xB6,
        /// <summary></summary>
        LaunchApplication2 = 0xB7,
        /// <summary></summary>
        OEM1 = 0xBA,
        /// <summary></summary>
        OEMPlus = 0xBB,
        /// <summary></summary>
        OEMComma = 0xBC,
        /// <summary></summary>
        OEMMinus = 0xBD,
        /// <summary></summary>
        OEMPeriod = 0xBE,
        /// <summary></summary>
        OEM2 = 0xBF,
        /// <summary></summary>
        OEM3 = 0xC0,
        /// <summary></summary>
        OEM4 = 0xDB,
        /// <summary></summary>
        OEM5 = 0xDC,
        /// <summary></summary>
        OEM6 = 0xDD,
        /// <summary></summary>
        OEM7 = 0xDE,
        /// <summary></summary>
        OEM8 = 0xDF,
        /// <summary></summary>
        OEMAX = 0xE1,
        /// <summary></summary>
        OEM102 = 0xE2,
        /// <summary></summary>
        ICOHelp = 0xE3,
        /// <summary></summary>
        ICO00 = 0xE4,
        /// <summary></summary>
        ProcessKey = 0xE5,
        /// <summary></summary>
        ICOClear = 0xE6,
        /// <summary></summary>
        Packet = 0xE7,
        /// <summary></summary>
        OEMReset = 0xE9,
        /// <summary></summary>
        OEMJump = 0xEA,
        /// <summary></summary>
        OEMPA1 = 0xEB,
        /// <summary></summary>
        OEMPA2 = 0xEC,
        /// <summary></summary>
        OEMPA3 = 0xED,
        /// <summary></summary>
        OEMWSCtrl = 0xEE,
        /// <summary></summary>
        OEMCUSel = 0xEF,
        /// <summary></summary>
        OEMATTN = 0xF0,
        /// <summary></summary>
        OEMFinish = 0xF1,
        /// <summary></summary>
        OEMCopy = 0xF2,
        /// <summary></summary>
        OEMAuto = 0xF3,
        /// <summary></summary>
        OEMENLW = 0xF4,
        /// <summary></summary>
        OEMBackTab = 0xF5,
        /// <summary></summary>
        ATTN = 0xF6,
        /// <summary></summary>
        CRSel = 0xF7,
        /// <summary></summary>
        EXSel = 0xF8,
        /// <summary></summary>
        EREOF = 0xF9,
        /// <summary></summary>
        Play = 0xFA,
        /// <summary></summary>
        Zoom = 0xFB,
        /// <summary></summary>
        Noname = 0xFC,
        /// <summary></summary>
        PA1 = 0xFD,
        /// <summary></summary>
        OEMClear = 0xFE
    }
}
