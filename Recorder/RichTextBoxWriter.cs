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

        public RichTextBoxWriter(RichTextBox output)
        {
            this.output = output;
            this.output.Text += Recorder.StopKey + "::{\n";
        }

        public void Close()
        {
            this.output.Text += "}";
        }

        public void Write(string value)
        {
            output.Text += "\t" + value;
        }
    }
}
