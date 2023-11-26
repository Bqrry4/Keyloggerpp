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
using KeyloggerIDE.ViewModels;
using static KeyloggerIDE.ViewModels.TabControlViewModel;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace KeyloggerIDE.Views;

public partial class MainView : UserControl
{
    private TabControlViewModel tabControlViewModel;

    private Popup? popup;

    public MainView()
    {
        InitializeComponent();
        popup = this.FindControl<Popup>("About");

        // init tab control
        TabView.DataContext = tabControlViewModel = new TabControlViewModel();
        tabControlViewModel.initTabControl();
    }

    private void OnAboutButton_Click(object sender, RoutedEventArgs e)
    {
        if (popup != null)
        {
            if (popup.IsOpen)
            {
                popup.IsOpen = false;
            }
            else
            {
                popup.IsOpen = true;
            }
        }
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
