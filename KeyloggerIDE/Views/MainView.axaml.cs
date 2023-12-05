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
    private TabControlViewModel tabControlViewModel;

    private CompletionWindow completionWindow;

    private IList<ICompletionData> _data = new List<ICompletionData>();

    private Popup? popup;

    private Button btn;

    private MenuItem menuItem;

    // private LogFace _controller = new LogFace();

    public MainView()
    {
        InitializeComponent();
        popup = this.FindControl<Popup>("About");

        // init tab control and editor
        TabView.DataContext = tabControlViewModel = new TabControlViewModel();
        tabControlViewModel.InitTabControl(AvalonEditor);

        // set editor callbacks and completions data
        AvalonEditor.TextArea.TextEntering += editor_TextArea_TextEntered;
        initCompletionData();
    }

    private void initCompletionData()
    {
        _data.Add(new MyCompletionData("MsgBox", "Shows a message box"));
        _data.Add(new MyCompletionData("MousePress", "Mouse press event(without release)"));
        _data.Add(new MyCompletionData("MouseRelease", "Mouse release event"));
        _data.Add(new MyCompletionData("Send", "Send text to be typed"));
        _data.Add(new MyCompletionData("SendInput", "Send key combinations with Ctrl or Alt"));
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

    private void TabView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TabView != null)
        {
            tabControlViewModel.ChangeSelection(TabView, AvalonEditor);
        }
    }

    private void AvalonEditor_OnTextChanged(object? sender, EventArgs e)
    {
        tabControlViewModel.ChangeFileStatus(sender, TabView);
    }

    void editor_TextArea_TextEntered(object sender, TextInputEventArgs e)
    {
        if (char.IsAsciiLetter(e.Text[0]) && completionWindow == null)
        {
            // Open code completion after the user has entered a matching letter:
            completionWindow = new CompletionWindow(AvalonEditor.TextArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

            // find matching entries
            foreach (MyCompletionData keyword in _data)
            {
                if (keyword.Text.StartsWith(e.Text[0]))
                {
                    data.Add(keyword);
                }
            }

            completionWindow.Show();
            completionWindow.Closed += delegate {
                completionWindow = null;
            };
        }
    }

    public void btn_OnClick(object? sender, EventArgs e) 
    {
        btn = (sender as Button)!;
        var currentTab = this.FindControl<TabControl>("TabView")!;
        var editor = this.FindControl<TextEditor>("AvalonEditor")!;

        switch (btn.Name)
        {
            case "NewFile":
                break;
            case "Save":
                tabControlViewModel.Save(currentTab, editor, null);
                break;
            case "SaveAs":
                tabControlViewModel.SaveAs(currentTab, editor, null);
                break;
            case "Open":
                break;
            default:
                break;
        }
    }

    public void menuItem_OnClick(object? sender, EventArgs e)
    {
        menuItem = (sender as MenuItem)!;
        var currentTab = this.FindControl<TabControl>("TabView")!;
        var editor = this.FindControl<TextEditor>("AvalonEditor")!;

        switch (menuItem.Name)
        {
            case "Menu_NewFile":
                break;
            case "Menu_Save":
                tabControlViewModel.Save(currentTab, editor, null);
                break;
            case "Menu_SaveAs":
                tabControlViewModel.SaveAs(currentTab, editor, null);
                break;
            case "Menu_Open":
                break;
            default:
                break;
        }
    }

}
