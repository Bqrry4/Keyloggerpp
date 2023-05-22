using System.Windows.Forms;

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

        public void Execute()
        {
            MessageBox.Show(_message, _title);
        }
    }
}
