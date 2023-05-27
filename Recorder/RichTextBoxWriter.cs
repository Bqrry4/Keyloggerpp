/**************************************************************************
*                                                                         *
*  File:        RichTextBoxWriter.cs                                      *
*  Copyright:   (c) Onofrei Grigore                                       *
*               @Kakerou_CLUB                                             *
*  Description: Recorder text box writer for scripts.                     *
*                                                                         *
**************************************************************************/

using System.Windows.Forms;

namespace Recorder
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
            this._output.Text += Logger.StopKey + "::\n{\n";
        }

        /// <summary>
        /// Write last line of code "}"
        /// </summary>
        public void Close()
        {
            this._output.Text += "}";
        }

        /// <summary>
        /// Write the script command and a tab before it
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            _output.Text += "\t" + value + "\r\n";
        }
    }
}
