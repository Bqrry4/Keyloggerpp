using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaEdit;
using Recorder;
using static System.Net.Mime.MediaTypeNames;

namespace KeyloggerIDE.Views
{
    internal class AvalonEditorWriter : IWriter
    {
        /// <summary>
        /// TextEditor to which commands will be written
        /// </summary>
        private TextEditor Output;

        /// <summary>
        /// Constructor with output parameter
        /// </summary>
        /// <param name="output">TextEditor for commands output</param>
        public AvalonEditorWriter(TextEditor output)
        {
            Output = output;
            Dispatcher.UIThread.Invoke(new Action(() => { Output.Text = ""; }));
        }

        /// <summary>
        /// Unused in this class
        /// </summary>
        public void Close() { }

        /// <summary>
        /// Write to output
        /// </summary>
        /// <param name="text">Text to be written</param>
        public async void Write(string text)
        {
            await Dispatcher.UIThread.InvokeAsync(new Action(() => { Output.Text += text; }));
        }
    }
}
