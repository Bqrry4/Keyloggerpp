using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using KeyloggerIDE.ViewModels;
using System;
using System.Collections.Generic;
using IntermediaryFacade;

namespace KeyloggerIDE.Views;

public partial class MainView : UserControl
{
    private readonly TabControlViewModel _tabControlViewModel;

    private CompletionWindow _completionWindow;

    private readonly Popup? _popup;

    private Button _btn;

    private MenuItem _menuItem;

    // private readonly LogFace _controller = new();

    private readonly TextEditor _editor;

    private readonly TabControl _tab;

    private int _state = 0;

    public MainView()
    {
        InitializeComponent();
        _popup = this.FindControl<Popup>("About");
        _editor = this.FindControl<TextEditor>("AvalonEditor")!;
        _tab = this.FindControl<TabControl>("TabView")!;

        // init tab control and editor
        TabView.DataContext = _tabControlViewModel = new TabControlViewModel();
        _tabControlViewModel.InitTabControl(AvalonEditor);

        // set editor callbacks
        AvalonEditor.TextArea.TextEntering += editor_TextArea_TextEntered;
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

            data.Add(new MyCompletionData("MsgBox", "Shows a message box"));
            data.Add(new MyCompletionData("MousePress", "Mouse press event(without release)"));
            data.Add(new MyCompletionData("MouseRelease", "Mouse release event"));
            data.Add(new MyCompletionData("Send", "Send text to be typed"));
            data.Add(new MyCompletionData("SendInput", "Send key combinations with Ctrl or Alt"));

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
                break;
            case "Save":
                _tabControlViewModel.Save(_tab, _editor, null);
                break;
            case "SaveAs":
                _tabControlViewModel.SaveAs(_tab, _editor, null);
                break;
            case "Open":
                _tabControlViewModel.Open(_tab, path);
                break;
        }
    }

    private void menuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        _menuItem = (sender as MenuItem)!;
        string path = "";
        switch (_menuItem.Name)
        {
            case "Menu_NewFile":
                break;
            case "Menu_Save":
                _tabControlViewModel.Save(_tab, _editor, null);
                break;
            case "Menu_SaveAs":
                _tabControlViewModel.SaveAs(_tab, _editor, null);
                break;
            case "Menu_Open":
                _tabControlViewModel.Open(_tab, path);
                break;
        }
    }

    private void Run_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_state == 0)
        {
            //_controller.StartRunning(_editor.Text);
            _state = 1;
        }
        else
        {
            //_controller.StopRunning();
            _state = 0;
        }
    }

    private void Record_OnClick(object? sender, RoutedEventArgs e)
    {
        //_controller.setOutput(new AvalonEditorWriter(_editor));
        if (_state == 0)
        {
            //_controller.StartRecording();
            _state = 2;
        }
        else
        {
            //_controller.StopRecording();
            _state = 0;
        }
    }
}
