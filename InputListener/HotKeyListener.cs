using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InputListener
{
    public class HotKeyListener
    {
        private void Register(List<string> hotKeys)
        {
            foreach (string hotKey in hotKeys)
            {
                ModifierKeys modifiers = ModifierKeys.None;

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
                        VirtualKeys vKey = (VirtualKeys)Enum.Parse(typeof(VirtualKeys), key);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Error at registering a Hotkey: Key not recognized: " + key, ex);
                    }


                }
            }
        }
    }
}
