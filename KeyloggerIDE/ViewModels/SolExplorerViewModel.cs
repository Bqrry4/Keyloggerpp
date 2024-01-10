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

    private ObservableCollection<File> _folder;
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
        while (string.IsNullOrEmpty(path))
        {
            int charLocation = path.IndexOf("/", StringComparison.Ordinal);

            if (charLocation > 0)
            {
                var file = new File(path.Substring(0, charLocation));
                Folder.Add(file);
                path = path.Remove(0, charLocation);
                IsChanged = true;
            }
        }
    }
}