/**************************************************************************
*                                                                         *
*  File:        TextFileWriter.cs                                         *
*  Copyright:   (c) Onofrei Grigore                                       *
*               @Kakerou_CLUB                                             *
*  Description: Recorder file writer for scripts.                         *
*                                                                         *
**************************************************************************/

using System;
using System.IO;
using System.Text;
using Recorder;

namespace Logface
{
    internal class TextFileWriter : IWriter
    {
        private FileStream _output;

        /// <summary>
        /// Creates or open a new file to write in
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <exception cref="System.Exception">FileStream related exceptions</exception>
        public TextFileWriter(string path)
        {
            try
            {
                _output = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Close FileStream to save changes
        /// </summary>
        void IWriter.Close()
        {
            _output.Close();
        }

        /// <summary>
        /// Writes a command to file
        /// </summary>
        /// <param name="value">Command to be written</param>
        void IWriter.Write(string value)
        {
            _output.Write(Encoding.UTF8.GetBytes(value), 0, value.Length);
        }
    }
}
