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
using System.Threading;
using System.Windows.Input;
using System.Windows.Interop;
using static InputListener.LLInput;


namespace InputListener
{
    /// <summary>
    /// A class that register hotkeys and notify a subscriber when it was produced
    /// </summary>
    public class HotKeyListener : IObservable<string>
    {
        //The thread where the listener will run (It must run in a separate thread cuz of the message queue)
        private Thread _hkThread;
        private uint _threadNativeID;

        //The consumers for hKeys
        private List<IObserver<string>> _observers;

        private Dictionary<(ModifierKeys modifier, VirtualKeys vKey), string> _registeredHKeys;
        private int _registeredHotkeyCount = 0;

        public HotKeyListener()
        {
            _observers = new List<IObserver<string>>();
            _registeredHKeys = new Dictionary<(ModifierKeys, VirtualKeys), string>();
        }

/*        ~HotKeyListener()
        {
            StopListening();
        }*/

        public void StartListening()
        {
            //Create the new thread
            _hkThread = new Thread(new ThreadStart(delegate
            {
                /*It must register and loop in the same thread to work more information go to
                 https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey
                 */
                RegHotKeys();
                Listen();

            }));

            //Start the thread
            _hkThread.Start();
        }


        public void StopListening()
        {
            //Post the WM_QUIT message on the message loop
            PostThreadMessage(_threadNativeID, 0x0012, IntPtr.Zero, IntPtr.Zero);
            //Wait for the thread to end
            _hkThread.Join();
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
                    case "ctrl":
                    case "Ctrl":
                    case "LeftCtrl":
                    case "RightCtrl":
                        modifiers |= ModifierKeys.Control;
                        continue;

                    case "win":
                    case "Win":
                    case "LeftWin":
                    case "RightWin":
                        modifiers |= ModifierKeys.Windows;
                        continue;
                    case "shift":
                    case "Shift":
                    case "LeftShift":
                    case "RightShift":
                        modifiers |= ModifierKeys.Shift;
                        continue;
                    case "alt":
                    case "Alt":
                    case "LeftAlt":
                    case "RightAlt":
                        modifiers |= ModifierKeys.Alt;
                        continue;
                }

                //Get the vk of the ordinary key
                try
                {
                    //Check if its a digit or a char
                    if (key.Length == 1)
                    {
                        //Check if its a digit on key
                        if ('0' <= key[0] && '1' >= key[0])
                        {
                            //Ascii for a digit correspond to its virtual Key
                            vKey = (VirtualKeys)key[0];
                        }
                        //Then its a character
                        else
                        {
                            vKey = (VirtualKeys)Enum.Parse(typeof(VirtualKeys), key.ToUpper());
                        }
                    }
                    //Something else
                    else
                    {
                        vKey = (VirtualKeys)Enum.Parse(typeof(VirtualKeys), key);
                    }

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

        /// <summary>
        /// Clear the registered hotkeys from the system
        /// !!MUST BE INVOKED FROM THE SAME THREAD THE RegHotKeys WAS!!
        /// </summary>
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
            _threadNativeID = GetCurrentThreadId();

            MSG msg = new MSG();
            while (GetMessage(ref msg, IntPtr.Zero, 0, 0))
            {
                if (msg.message == (int)MessageType.WM_HOTKEY)
                {
                    ModifierKeys mod = (ModifierKeys)((int)msg.lParam & 0xFFFF);
                    VirtualKeys vk = (VirtualKeys)(((int)msg.lParam >> 16) & 0xFFFF);

                    string hotKey;
                    if (_registeredHKeys.TryGetValue((mod, vk), out hotKey))
                    {
                        //Notify those who signed for hk
                        foreach (var observer in _observers)
                        {
                            observer.OnNext(hotKey);
                        }
                    }
                }
            }
            //WM_QUIT recieved
            UnRegHotKeys();
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
