using System.IO;
using System.Text;

namespace Recorder
{
    internal class TextFileWriter : IWriter
    {
        private FileStream output;

        public TextFileWriter(string path)
        {
            output = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            string firstLine = Recorder.StopKey + "::{\n";
            output.Write(Encoding.UTF8.GetBytes(firstLine), 0, firstLine.Length);
        }

        void IWriter.Close()
        {
            output.Write(Encoding.UTF8.GetBytes("}"), 0, "}".Length);
            output.Close();
        }

        void IWriter.Write(string value)
        {
            output.Write(Encoding.UTF8.GetBytes(value), 0, value.Length);
        }
    }
}
