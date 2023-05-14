using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InputListener
{
    internal class InputListener
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IntPtr LowLevelInputProc(int nCode, UIntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0)
            {
                if ((LLInput.KeyEvent)wParam.ToUInt32() == LLInput.KeyEvent.WM_SYSKEYDOWN)
                {
                    //Console.WriteLine((Keys)Marshal.ReadInt32(lParam));
                }
            }

            return LLInput.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
    }
}
