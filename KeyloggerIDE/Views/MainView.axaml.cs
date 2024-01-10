using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using KeyloggerIDE.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using IntermediaryFacade;
using KeyloggerIDE.Models;
using Avalonia.Themes.Fluent;

namespace KeyloggerIDE.Views;

public partial class MainView : UserControl
{
    private readonly TabControlViewModel _tabControlViewModel;

    private readonly SolExplorerViewModel _solExplorer = new();

    private CompletionWindow _completionWindow;

    private readonly Popup? _popup;

    private Button _btn;

    private MenuItem _menuItem;

    private readonly LogFace _controller = new();

    private readonly TextEditor _editor;

    private readonly TabControl _tab;

    private int _state = 0;

    StyleInclude light = new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
    {
        Source = new Uri("resm:Avalonia.Themes.Default.Accents.BaseLight.xaml?assembly=Avalonia.Themes.Default")
    };

    StyleInclude dark = new StyleInclude(new Uri("resm:Styles?assembly=ControlCatalog"))
    {
        Source = new Uri("resm:Avalonia.Themes.Default.Accents.BaseDark.xaml?assembly=Avalonia.Themes.Default")
    };

    public MainView()
    {
        InitializeComponent();
        _popup = this.FindControl<Popup>("About");
        _editor = this.FindControl<TextEditor>("AvalonEditor")!;
        _tab = this.FindControl<TabControl>("TabView")!;

        // init tab control and editor
        TabView.DataContext = _tabControlViewModel = new TabControlViewModel();
        _tabControlViewModel.InitTabControl(AvalonEditor);
        Explorer.DataContext = _solExplorer;
        
        // set editor callbacks
        AvalonEditor.TextArea.TextEntering += editor_TextArea_TextEntered;
    }

    private void This_ShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        Editor.Background = Brushes.Green;
    }

    private void OnAboutButton_Click(object sender, RoutedEventArgs e)
    {
        if (_popup != null)
        {
            if (_popup.IsOpen)
            {
                _popup.IsOpen = false;
            }
            else
            {
                _popup.IsOpen = true;
            }
        }
    }

    private void TabView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TabView != null)
        {
            _tabControlViewModel.ChangeSelection(TabView, AvalonEditor);
        }
    }

    private void AvalonEditor_OnTextChanged(object? sender, EventArgs e)
    {
        _tabControlViewModel.ChangeFileStatus(sender, TabView);
    }

    private void editor_TextArea_TextEntered(object sender, TextInputEventArgs e)
    {
        if (char.IsAsciiLetter(e.Text[0]))
        {
            // Open code completion after the user has entered a matching letter:
            _completionWindow = new CompletionWindow(AvalonEditor.TextArea);
            IList<ICompletionData> data = _completionWindow.CompletionList.CompletionData;

            data.Add(new MyCompletionData("MsgBox(\"\")", "Shows a message box with text"));
            data.Add(new MyCompletionData("MsgBox(\"\", \"\")", "Shows a message box with text and title"));
            data.Add(new MyCompletionData("MousePress(, , )", "Mouse press event(without release)"));
            data.Add(new MyCompletionData("MouseRelease(, , )", "Mouse release event"));
            data.Add(new MyCompletionData("Send(\"\")", "Send text to be typed"));
            data.Add(new MyCompletionData("SendInput(\"\")", "Send key combinations with Ctrl or Alt"));

            _completionWindow.Show();
            _completionWindow.Closed += delegate {
                _completionWindow = null;
            };
        }
    }

    private void btn_OnClick(object? sender, RoutedEventArgs e) 
    {
        _btn = (sender as Button)!;
        string path = "";
        switch (_btn.Name)
        {
            case "NewFile":
                TabView.SelectedIndex = TabView.ItemCount - 1;
                break;
            case "Save":
                _tabControlViewModel.Save(_tab, _editor, null);
                break;
            case "SaveAs":
                _tabControlViewModel.SaveAs(_tab, _editor, null);
                break;
        }
    }

    private async void btnOpen_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(_tab);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open file"
        });

        if (files.Count >= 1)
            _tabControlViewModel.Open(_tab, files[0].Path.AbsolutePath);


    }

    private void menuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        _menuItem = (sender as MenuItem)!;
        string path = "";
        switch (_menuItem.Name)
        {
            case "MenuNewFile":
                TabView.SelectedIndex = TabView.ItemCount - 1;
                break;
            case "MenuSave":
                _tabControlViewModel.Save(_tab, _editor, null);
                break;
            case "MenuSaveAs":
                _tabControlViewModel.SaveAs(_tab, _editor, null);
                break;
        }
    }

    private async void menuOpen_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(_tab);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open file"
        });

        if (files.Count >= 1)
        {
            _tabControlViewModel.Open(_tab, files[0].Path.AbsolutePath);
            _solExplorer.CreateSolExp(files[0].Path.AbsolutePath);
        }
    }

    private void Run_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_state == 0)
        {
            _controller.StartRunning(_editor.Text, 0);
            _state = 1;
            Run.Background = Brushes.DarkRed;
            Record.IsEnabled = false;
            Debug.IsEnabled = false;

            // themes test code
            App.Current.RequestedThemeVariant = ThemeVariant.Light;
            _tabControlViewModel.loadSyntaxDefinition(AvalonEditor);
        }
        else
        {
            _controller.StopRunning();
            _state = 0;
            Run.Background = Brushes.Green;
            Record.IsEnabled = true;
            Debug.IsEnabled = true;

            //themes test code
            App.Current.RequestedThemeVariant = ThemeVariant.Dark;
            _tabControlViewModel.loadSyntaxDefinition(AvalonEditor);
        }

        // themes testing code
    }

    private void Record_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_state == 0)
        {
            _controller.setOutput(new AvalonEditorWriter(_editor));
            _controller.StartRecording();
            _state = 2;
            Record.Background = Brushes.DarkRed;
            Run.IsEnabled = false;
            Debug.IsEnabled = false;
        }
        else
        {
            _controller.StopRecording();
            _state = 0;
            Record.Background = Brushes.Green;
            Run.IsEnabled = true;
            Debug.IsEnabled = true;
        }
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        _tabControlViewModel.CloseTab(TabView, AvalonEditor, (Button)sender);
    }

    private void Debug_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_state == 0)
        {
            _controller.StartRunning(_editor.Text, 1);
            _state = 2;
            Debug.Background = Brushes.DarkRed;
            Run.IsEnabled = false;
            Record.IsEnabled = false;
        }
        else
        {
            _controller.StopRunning();
            _state = 0;
            Debug.Background = Brushes.Green;
            Run.IsEnabled = true;
            Record.IsEnabled = true;
        }
    }
}
