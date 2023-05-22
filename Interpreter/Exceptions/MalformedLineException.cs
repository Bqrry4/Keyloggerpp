using System;

namespace Interpreter.Exceptions
{
    internal class MalformedLineException : Exception
    {
        private readonly string message;
        public MalformedLineException() { }
        public MalformedLineException(string message) 
        {
            this.message = message;
        }
    }
}
