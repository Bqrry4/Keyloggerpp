/**************************************************************************
*                                                                         *
*  File:        IWriter.cs                                                *
*  Copyright:   (c) Onofrei Grigore                                       *
*               @Kakerou_CLUB                                             *
*  Description: Interface for writers used in Recorder.                   *
*                                                                         *
**************************************************************************/

namespace Recorder
{
    public interface IWriter
    {
        void Write(string value);

        void Close();
    }
}
