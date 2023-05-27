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

                string firstLine = Logger.StopKey + "::\n{\n";
                _output.Write(Encoding.UTF8.GetBytes(firstLine), 0, firstLine.Length);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Write last line of script and close FileStream to save changes
        /// </summary>
        void IWriter.Close()
        {
            _output.Write(Encoding.UTF8.GetBytes("}"), 0, "}".Length);
            _output.Close();
        }

        void IWriter.Write(string value)
        {
            _output.Write(Encoding.UTF8.GetBytes("\t" + value + "\r\n"), 0, value.Length);
        }
    }
}
