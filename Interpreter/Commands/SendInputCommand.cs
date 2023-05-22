using System;

namespace Interpreter
{
    /// <summary>
    /// Command that sends modified keypresses
    /// </summary>
    internal class SendInputCommand : IKlppCommand
    {
        private string _text;

        public SendInputCommand(string text)
        {
            _text = text;
        }

        public void Execute()
        {
            //TODO: SEND MODIFIED KEY
            throw new NotImplementedException();
        }
    }
}
