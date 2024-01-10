/**************************************************************************
 *                                                                        *
 *  File:        MessageBoxCommand.cs                                     *
 *  Copyright:   (c) Olăreț Radu                                          *
 *               @Kakerou_CLUB                                            *
 *  Description: Command that opens a messageBox with the given message   *
 *               and titles.                                              *
 **************************************************************************/

//using System.Windows.Forms;

using InputListener;
using System.Runtime.InteropServices;

namespace Interpreter
{
    /// <summary>
    /// Command that opens a messageBox with the given message and titles
    /// </summary>
    internal class MessageBoxCommand : IKlppCommand
    {
        private readonly string _message;
        private readonly string _title;


        public MessageBoxCommand(string message, string title = "Alert!")
        {
            _message = message;
            _title = title;
        }

        /// <summary>
        /// ....Shows a message box...
        /// </summary>
        public void Execute()
        {
            LLInput.MessageBox(IntPtr.Zero, _message, _title, 0);
        }
    }
}
