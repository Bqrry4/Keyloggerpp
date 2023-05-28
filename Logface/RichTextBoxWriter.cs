/**************************************************************************
*                                                                         *
*  File:        RichTextBoxWriter.cs                                      *
*  Copyright:   (c) Onofrei Grigore                                       *
*               @Kakerou_CLUB                                             *
*  Description: Recorder text box writer for scripts.                     *
*                                                                         *
**************************************************************************/

using System;
using System.Windows.Forms;
using Recorder;

namespace Logface
{
    public class RichTextBoxWriter : IWriter
    {
        private RichTextBox _output;

        /// <summary>
        /// Every scripts starts with a line that describes the stop execution key
        /// </summary>
        /// <param name="output">TextBox to be written in</param>
        public RichTextBoxWriter(RichTextBox output)
        {
            this._output = output;
        }

        /// <summary>
        /// Unused in this class
        /// </summary>
        public void Close() {}

        private void CrossThreadWrite(string str)
        {

        }

        /// <summary>
        /// Write the command using deleget for thread safety
        /// </summary>
        /// <param name="value">Command to be written</param>
        public void Write(string value)
        {
            if(_output.InvokeRequired)
            {
                Action safeWrite = delegate { Write(value); };
                object tst = _output.Invoke(safeWrite);
            }
            else
            {
                _output.Text += value;
            }
        }
    }
}
