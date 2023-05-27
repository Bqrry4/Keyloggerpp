/**************************************************************************
 *                                                                        *
 *  File:        MalformedLineException.cs                                *
 *  Copyright:   (c) Olăreț Radu                                          *
 *               @Kakerou_CLUB                                            *
 *  Description: Exception class for malformed klpp commands.             *
 *                                                                        *
 **************************************************************************/

using System;

namespace Interpreter.Exceptions
{
    internal class MalformedLineException : Exception
    {
        private readonly string _message;
        public MalformedLineException() { }
        public MalformedLineException(string message) 
        {
            this._message = message;
        }
    }
}
