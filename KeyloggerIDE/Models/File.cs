using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyloggerIDE.Models
{
    public class File
    {
        public ObservableCollection<File>? SubFiles { get; }
        public string FileName { get; }

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