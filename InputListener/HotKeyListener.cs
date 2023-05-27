using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using static InputListener.LLInput;


namespace InputListener
{
    public class HotKeyListener
    {

        private Dictionary<(ModifierKeys modifier, VirtualKeys vKey), string> _registeredHKeys;
        private int _registeredHotkeyCount = 0;

        public HotKeyListener()
        {
            _registeredHKeys = new Dictionary<(ModifierKeys, VirtualKeys), string>();
        }

        ~HotKeyListener()
        {
            StopListening();
        }

        public void StartListening()
        {
            /*It must register and loop in the same thread to work more information go to
             https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey
             */
            RegHotKeys();
            Listen();
        }


        public void StopListening()
        {
            UnRegHotKeys();
        }

        public void Register(List<string> hotKeys)
        {
            foreach (string hotKey in hotKeys)
            {
                ModifierKeys modifiers = ModifierKeys.None;
                VirtualKeys vKey;
                string[] keys = hotKey.Split(' ');
                foreach (string key in keys)
                {
                    //Check for modifiers
                    if (key.Equals("Ctrl") || key.Equals("LeftCtrl") || key.Equals("RightCtrl"))
                    {
                        modifiers |= ModifierKeys.Control;
                        continue;
                    }
                    if (key.Equals("Win") || key.Equals("LeftWin") || key.Equals("RightWin"))
                    {
                        modifiers |= ModifierKeys.Windows;
                        continue;
                    }
                    if (key.Equals("Shift") || key.Equals("LeftShift") || key.Equals("RightShift"))
                    {
                        modifiers |= ModifierKeys.Shift;
                        continue;
                    }
                    if (key.Equals("Alt") || key.Equals("LeftAlt") || key.Equals("RightAlt"))
                    {
                        modifiers |= ModifierKeys.Alt;
                        continue;
                    }

                    //Get the vk of the ordinary key
                    try
                    {
                        vKey = (VirtualKeys)Enum.Parse(typeof(VirtualKeys), key);

                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Error at registering a Hotkey: Key not recognized: " + key, ex);
                    }

                    //Seems like a valid one, adding to the collection (￣ヘ￣)
                    _registeredHKeys.Add((modifiers, vKey), hotKey);
                }
            }
        }

        //Register the hotkeys to the system
        private void RegHotKeys()
        {
            foreach (var entry in _registeredHKeys)
            {
                RegisterHotKey(IntPtr.Zero, _registeredHotkeyCount, (int)entry.Key.modifier, (int)entry.Key.vKey);
                _registeredHotkeyCount++;
            }

        }

        //Clear the registered hotkeys from the system
        private void UnRegHotKeys()
        {
            for (int i = 0; i < _registeredHotkeyCount; i++)
            {
                UnregisterHotKey(IntPtr.Zero, i);
            }
        }


        //The message loop, waiting for hotKeys
        private void Listen()
        {
            MSG msg = new MSG();
            while (GetMessage(ref msg, IntPtr.Zero, 0, 0))
            {
                if (msg.message == (int)MessageType.WM_HOTKEY)
                {
                    ModifierKeys mod = (ModifierKeys)((int)msg.lParam & 0xFFFF);
                    VirtualKeys vk = (VirtualKeys)(((int)msg.lParam >> 16) & 0xFFFF);

                    string hotKey = string.Empty;
                    _registeredHKeys.TryGetValue((mod, vk), out hotKey);

                    //Notify those who signed for hk
                }
            }
        }

    }
}
