using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public class RichTextBoxWriter : IWriter
    {
        private RichTextBox output;

        /// <summary>
        /// Every scripts starts with a line that describes the stop execution key
        /// </summary>
        /// <param name="output">TextBox to be written in</param>
        public RichTextBoxWriter(RichTextBox output)
        {
            this.output = output;
            this.output.Text += Recorder.StopKey + "::{\n";
        }

        /// <summary>
        /// Write last line of code "}"
        /// </summary>
        public void Close()
        {
            this.output.Text += "}";
        }

        /// <summary>
        /// Write the script command and a tab before it
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            output.Text += "\t" + value;
        }
    }
}
