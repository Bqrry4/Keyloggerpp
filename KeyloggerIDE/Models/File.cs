using System.Collections.ObjectModel;

namespace KeyloggerIDE.Models
{
    public class File
    {
        public ObservableCollection<File>? SubFiles { get; set; }
        public string FileName { get; }

        public File() { }

        public File(string fileName)
        {
            FileName = fileName;
        }

        public File(string fileName, ObservableCollection<File> subFiles)
        {
            FileName = fileName;
            SubFiles = subFiles;
        }
    }
}