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
            int charLocation = 0;
            while (string.IsNullOrEmpty(path) == false && charLocation != -1)
            {
                charLocation = path.IndexOf("/", StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    var file = new File(path.Substring(0, charLocation));
                    _folder.Add(file);
                    path = path.Remove(0, charLocation + 1);
                    
                }
            }
            IsChanged = true;
        }
        else
        {
            IsChanged = false;
        }
    }
}