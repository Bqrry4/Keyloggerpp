using System;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using KeyloggerIDE.Models;

namespace KeyloggerIDE.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ObservableCollection<File> Folder { get; }

    public MainViewModel()
    {
        Folder = new ObservableCollection<File>()
        {
            new File("File1", new ObservableCollection<File>()
            {
                new File("File2"), new File("File3")
            })
        };
    }

}
