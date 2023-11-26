using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using KeyloggerIDE.CustomControls;
using KeyloggerIDE.ViewModels;
using static KeyloggerIDE.ViewModels.TabControlViewModel;

namespace KeyloggerIDE.Views;

public partial class MainView : UserControl
{
    private const string SavefilePath = "savefile.xml";

    //private ObservableCollection<TabControlPageViewModelItem> files = new ObservableCollection<TabControlPageViewModelItem>();
    private TabControlViewModel tabControlViewModel;

    public MainView()
    {
        InitializeComponent();

        
        TabView.DataContext = tabControlViewModel = new TabControlViewModel();
        tabControlViewModel.initTabControl();
    }

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        tabControlViewModel.handleScroll(sender, e);
    }

    private void TextBox_OnTextChanging(object? sender, TextChangingEventArgs e)
    {
        tabControlViewModel.changeFileStatus(sender, TabView);
    }
}
