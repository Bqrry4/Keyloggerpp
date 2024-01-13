using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KeyloggerIDE.Models;

namespace KeyloggerIDE.ViewModels;

public class SolExplorerViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ObservableCollection<File> _folder = new ObservableCollection<File>();
    public ObservableCollection<File> Folder
    {
        get => _folder;
        set
        {
            _folder = value;
            RaisePropertyChanged();
        }
    }

    private bool _isChanged;

    public bool IsChanged
    {
        get => _isChanged;
        set
        {
            _isChanged = value;
            RaisePropertyChanged();
        }
    }

    public void CreateSolExp(string path)
    {
        if (string.IsNullOrEmpty(path) == false)
        {
            int charLocation = path.LastIndexOf("/", StringComparison.Ordinal);
            var path_scan = path.Substring(0, charLocation);
            string[] directories = System.IO.Directory.GetDirectories(path_scan);
            string[] files = System.IO.Directory.GetFiles(path_scan);
            File folder = new File(path_scan);
            folder.SubFiles = new ObservableCollection<File>();

            if (directories.Length > 0)
            {
                foreach (string name in directories)
                {
                    charLocation = name.IndexOf("\\", 0);
                    folder.SubFiles.Add(new File(name.Substring(charLocation + 1)));
                }
            }

            if (files.Length > 0)
            {
                foreach (string name in files)
                {
                    charLocation = name.IndexOf("\\", 0);
                    folder.SubFiles.Add(new File(name.Substring(charLocation + 1)));
                }
            }
            _folder.Add(folder);
            IsChanged = true;
        }
        else
        {
            IsChanged = false;
        }
    }
}