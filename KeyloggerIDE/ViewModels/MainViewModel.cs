using System;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Media.Imaging;
using KeyloggerIDE.Models;
using ReactiveUI;

namespace KeyloggerIDE.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ObservableCollection<File> _folder;
    public ObservableCollection<File> Folder
    {
        get => _folder;
        set
        {
            this.RaiseAndSetIfChanged(ref _folder, value);
        }
    }

    public MainViewModel()
    {
    }

    public void createSolExp(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            while (path != "")
            {
                int charLocation = path.IndexOf("/", StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    var file = new File(path.Substring(0, charLocation));
                    _folder.Add(file);
                    path = path.Remove(0, charLocation);
                }
            }
        }

        this.RaisePropertyChanged(nameof(_folder));
    }
}
