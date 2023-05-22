using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    /// <summary>
    /// Command that sends raw text
    /// </summary>
    internal class SendCommand : IKlppCommand
    {
        private string _text;

        public SendCommand(string text)
        {
            _text = text;
        }
        
        public void Execute()
        {
            //TODO: SEND TEXT
            throw new NotImplementedException();
        }
    }
}
