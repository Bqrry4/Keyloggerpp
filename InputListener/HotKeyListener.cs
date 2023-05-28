/**************************************************************************
*                                                                        *
*  File:        HotKeyListener.cs                                        *
*  Copyright:   (c)Paniș Alexandru                                       *
*               @Kakerou_CLUB                                            *
*  Description: Listener for hotkeys that implemets observer pattern,    *
*               so it notify all the subscribers of a hotkey that was    *
*               registered                                               *
*                                                                        *
**************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Interop;
using static InputListener.LLInput;


namespace InputListener
{
    /// <summary>
    /// 
    /// </summary>
    public class HotKeyListener : IObservable<string>
    {
        //The consumers for hKeys
        private List<IObserver<string>> _observers; 

        private Dictionary<(ModifierKeys modifier, VirtualKeys vKey), string> _registeredHKeys;
        private int _registeredHotkeyCount = 0;

        public HotKeyListener()
        {
            _observers = new List<IObserver<string>>();
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


        //Add a hotkey to the dictionary
        public void Register(string hotKey)
        {
            ModifierKeys modifiers = ModifierKeys.None;
            VirtualKeys vKey;
            string[] keys = hotKey.Split(' ');
            foreach (string key in keys)
            {

                //Check for modifiers
                switch (key)
                {
                    case "Ctrl":
                    case "LeftCtrl":
                    case "RightCtrl":
                        modifiers |= ModifierKeys.Control;
                        break;
                    case "Win":
                    case "LeftWin":
                    case "RightWin":
                        modifiers |= ModifierKeys.Windows;
                        break;
                    case "Shift":
                    case "LeftShift":
                    case "RightShift":
                        modifiers |= ModifierKeys.Shift;
                        break;
                    case "Alt":
                    case "LeftAlt":
                    case "RightAlt":
                        modifiers |= ModifierKeys.Alt;
                        break;
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

        public void Clear()
        {
            _registeredHKeys.Clear();
            _registeredHotkeyCount = 0;
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
                    foreach(var observer in _observers)
                    {
                        observer.OnNext(hotKey);
                    }
                }
            }
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            // return new Unsubscriber(_observers, observer);
            return null;
        }
    }
}
