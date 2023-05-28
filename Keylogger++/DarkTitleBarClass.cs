/**************************************************************************
 *                                                                        *
 *  File:        DarkTitleBarClass.cs                                     *
 *  Copyright:   (c) Păduraru George                                      *
 *               @Kakerou_CLUB                                            *
 *  Description: Class to make the forms title bar dark.                  *
 *                                                                        *
 **************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace Keylogger__
{
    public class DarkTitleBarClass
    {

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        /// <summary>
        /// Function for changing the title bar of forms to dark theme based on windows version.
        /// </summary>
        /// <param name="handle"></param> Required interface handle that the change applies to
        /// <param name="enabled"></param>
        /// <returns></returns>
        internal static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }
    }
}
