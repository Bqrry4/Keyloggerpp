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

namespace Recorder
{
    internal class TextFileWriter : IWriter
    {
        private FileStream output;

        /// <summary>
        /// Creates or open a new file to write in
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <exception cref="System.Exception">FileStream related exceptions</exception>
        public TextFileWriter(string path)
        {
            try
            {
                output = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

                string firstLine = Logger.StopKey + "::{\n";
                output.Write(Encoding.UTF8.GetBytes(firstLine), 0, firstLine.Length);
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
            output.Write(Encoding.UTF8.GetBytes("}"), 0, "}".Length);
            output.Close();
        }

        void IWriter.Write(string value)
        {
            output.Write(Encoding.UTF8.GetBytes("\t" + value + "\r\n"), 0, value.Length);
        }
    }
}
