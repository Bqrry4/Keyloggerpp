using System;

namespace Interpreter.Exceptions
{
    internal class HotkeyNotFoundException : Exception
    {
        private readonly string message;
        public HotkeyNotFoundException() { }
        public HotkeyNotFoundException(string message)
        {
            this.message = message;
        }
    }
}
