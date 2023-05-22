using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
