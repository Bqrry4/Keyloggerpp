/**************************************************************************
 *                                                                        *
 *  File:        HotkeyNotFoundException.cs                               *
 *  Copyright:   (c) Olăreț Radu                                          *
 *               @Kakerou_CLUB                                            *
 *  Description: Exception class for interpreter.                         *
 *                                                                        *
 **************************************************************************/

using System;

namespace Interpreter.Exceptions
{
    internal class HotkeyNotFoundException : Exception
    {
        private readonly string _message;
        public HotkeyNotFoundException() { }
        public HotkeyNotFoundException(string message)
        {
            this._message = message;
        }
    }
}
