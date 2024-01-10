using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting.Xshd;
using AvaloniaEdit.Highlighting;
using System.Reflection;
using Avalonia.Styling;

namespace KeyloggerIDE.ViewModels
{
    public class TabControlViewModel
    {
        private const string DarkSyntaxFile = "syntax_definition_dark.xshd";

        private const string LightSyntaxFile = "syntax_definition_light.xshd";

        private const string DarkDefaultSyntax = "<SyntaxDefinition name=\"k++\" xmlns=\"http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008\">\r\n    <Color name=\"Comment\" foreground=\"Green\" />\r\n    <Color name=\"String\" foreground=\"rgb(232,201,187)\" />\r\n    \r\n    <!-- This is the main ruleset. -->\r\n    <RuleSet>        \r\n        <Span color=\"String\">\r\n            <Begin>\"</Begin>\r\n            <End>\"</End>\r\n        </Span>\r\n        \r\n        <Keywords fontWeight=\"bold\" foreground=\"rgb(220,220,170)\">\r\n            <Word>MsgBox</Word>\r\n            <Word>MousePress</Word>\r\n            <Word>MouseRelease</Word>\r\n            <Word>Send</Word>\r\n            <Word>SendInput</Word>\r\n        </Keywords>\r\n        \r\n        <Rule foreground=\"rgb(184,215,163)\">\r\n            \\d+\r\n        </Rule>\r\n\t\t\r\n\t\t<Rule foreground=\"#b33f3b\">\r\n\t\t\t[A-Za-z]+\\s+[A-Za-z]::\r\n\t\t</Rule>\r\n    </RuleSet>\r\n</SyntaxDefinition>";

        private const string LightDefaultSyntax = "<SyntaxDefinition name=\"k++\" xmlns=\"http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008\">\r\n    <Color name=\"Comment\" foreground=\"Green\" />\r\n    <Color name=\"String\" foreground=\"rgb(232,201,187)\" />\r\n    \r\n    <!-- This is the main ruleset. -->\r\n    <RuleSet>        \r\n        <Span color=\"String\">\r\n            <Begin>\"</Begin>\r\n            <End>\"</End>\r\n        </Span>\r\n        \r\n        <Keywords fontWeight=\"bold\" foreground=\"rgb(220,220,170)\">\r\n            <Word>MsgBox</Word>\r\n            <Word>MousePress</Word>\r\n            <Word>MouseRelease</Word>\r\n            <Word>Send</Word>\r\n            <Word>SendInput</Word>\r\n        </Keywords>\r\n        \r\n        <Rule foreground=\"rgb(184,215,163)\">\r\n            \\d+\r\n        </Rule>\r\n\t\t\r\n\t\t<Rule foreground=\"#f83f3b\">\r\n\t\t\t[A-Za-z]+\\s+[A-Za-z]::\r\n\t\t</Rule>\r\n    </RuleSet>\r\n</SyntaxDefinition>";
        
        /// <summary>
        /// Path to file which stores opened tabs info
        /// </summary>
        private const string SavefilePath = "savefile.xml";

        /// <summary>
        /// Index of selected tab (used for selection changed event)
        /// </summary>
        private int _selectedIndex = 0;

        /// <summary>
        /// Prevent handling events that are happening on initialization
        /// </summary>
        private bool _initComplete = false;

        private readonly ObservableCollection<TabControlPageViewModelItem> _tabs = new();

        /// <summary>
        /// Observable collection bound to tab control
        /// </summary>
        public ObservableCollection<TabControlPageViewModelItem> Tabs => _tabs;

        /// <summary>
        /// Tab item model class for binding
        /// </summary>
        public class TabControlPageViewModelItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private string? _header;

            public string? Header
            {
                get => _header;
                set
                {
                    _header = value;
                    RaisePropertyChanged();
                }
            }

            public string? FilePath { get; set; }

            public string? Content { get; set; }

            public string? Status { get; set; }

            private bool _isSaved = true;

            public bool IsSaved
            {
                get => _isSaved;

                set
                {
                    _isSaved = value;
                    Status = IsSaved ? "" : "*";
                    RaisePropertyChanged(nameof(Status));
                }
            }

            public bool CloseBtn { get; set; }
        }

        /// <summary>
        /// Init tabs from save file
        /// </summary>
        /// <param name="editor">TextEditor control</param>
        public void InitTabControl(TextEditor editor)
        {
            // init tabs from savefile
            if (!File.Exists(SavefilePath))
            {
                XmlWriter writer = XmlWriter.Create(SavefilePath);
                writer.WriteRaw("\r\n<TabControl>\r\n</TabControl>");
                writer.Flush();
                writer.Close();
            }

            XmlDocument savefile = new XmlDocument();
            savefile.Load(SavefilePath);

            // load tab items if there are any
            XmlNodeList? tabItems = savefile.SelectNodes("/TabControl/FileTabItem");
            if ((tabItems != null) && (tabItems.Count != 0))
            {
                foreach (XmlNode item in tabItems)
                {
                    StreamReader sr = new StreamReader(item.Attributes["path"].Value);
                    string content = sr.ReadToEnd();

                    _tabs.Add(new TabControlPageViewModelItem
                    {
                        Header = item.InnerText,
                        FilePath = item.Attributes["path"].Value,
                        Content = content,
                        Status = "",
                        IsSaved = true,
                        CloseBtn = true
                    });
                }

                // set editor text to last opened file
                editor.Text = _tabs.First().Content;
            }
            else
            {
                _tabs.Add(new TabControlPageViewModelItem
                {
                    Header = "new tab",
                    FilePath = "",
                    Content = "",
                    Status = "*",
                    IsSaved = false,
                    CloseBtn = true
                });
            }

            _tabs.Add(new TabControlPageViewModelItem
            {
                Header = "+", 
                FilePath = "", 
                Content = "",
                Status = "",
                IsSaved = true,
                CloseBtn = false
            });

            loadSyntaxDefinition(editor);
            
            _initComplete = true;
        }

        public void loadSyntaxDefinition(TextEditor editor)
        {
            string file;
            string definition;

            if (App.Current.RequestedThemeVariant == ThemeVariant.Light)
            {
                file = LightSyntaxFile;
                definition = LightDefaultSyntax;
            }
            else
            {
                file = DarkSyntaxFile;
                definition = DarkDefaultSyntax;
            }

            // create syntax rules file if it doesn't exist
            if (!File.Exists(file))
            {
                XmlWriter writer = XmlWriter.Create(file);

                // write default syntax rules
                writer.WriteRaw(definition);
                writer.Flush();
                writer.Close();
            }

            // load syntax highlighting rules
            using (FileStream s = File.Open(file, FileMode.Open))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        /// <summary>
        /// Mark file as not saved ('*') if it was changed
        /// </summary>
        /// <param name="sender">target text box</param>
        /// <param name="tabView">target tab control</param>
        public void ChangeFileStatus(object? sender, TabControl tabView)
        {
            TextEditor editor = (TextEditor)sender;
            if (_tabs[tabView.SelectedIndex].IsSaved && editor.Text != _tabs[tabView.SelectedIndex].Content)
            {
                _tabs[tabView.SelectedIndex].IsSaved = false;
            }
        }

        /// <summary>
        /// Save all opened tabs
        /// </summary>
        /// <param name="tabView">tab control</param>
        /// <param name="editor">avalonia editor</param>
        public void SaveAll(TabControl tabView, TextEditor editor)
        {
            for (int i = 0; i < _tabs.Count - 1; ++i)
            {
                Save(tabView, editor, _tabs[i]);
            }
        }

        /// <summary>
        /// Save file as
        /// </summary>
        /// <param name="tabView">tab control</param>
        /// <param name="editor">avalonia editor</param>
        /// <param name="page">target tab, can be omitted in which case will be selected tab</param>
        public async void SaveAs(TabControl tabView, TextEditor editor, TabControlPageViewModelItem? page)
        {
            if (page == null)
            {
                page = _tabs[tabView.SelectedIndex];
            }

            // sync tab view model and editor
            page.Content = editor.Text;

            // get file dialog from TopLevel
            TopLevel topLevel = TopLevel.GetTopLevel(tabView);
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = page.Header
            });

            if (file is not null)
            {
                // Open writing stream from the file.
                await using var stream = await file.OpenWriteAsync();
                using var streamWriter = new StreamWriter(stream);

                // Write some content to the file.
                await streamWriter.WriteAsync(editor.Text);

                page.Header = file.Name;
                page.FilePath = file.Path.AbsolutePath;
                page.IsSaved = true;
            }
        }

        /// <summary>
        /// Save file associated with selected tab
        /// </summary>
        /// <param name="tabView">tab control</param>
        /// <param name="editor">avalonia editor</param>
        /// <param name="page">target tab, can be omitted in which case will be selected tab</param>
        public void Save(TabControl tabView, TextEditor editor, TabControlPageViewModelItem? page = null)
        {
            if (tabView.SelectedItem == null)
                return;

            if (page == null)
            {
                page = _tabs[tabView.SelectedIndex];
            }

            // if file path is null or empty, open a save file dialog
            if (string.IsNullOrEmpty(page.FilePath))
            {
                SaveAs(tabView, editor, page);
            }
            else
            {
                // sync tab view model and editor
                page.Content = editor.Text;

                // write content from text box to file
                StreamWriter sw = new StreamWriter(page.FilePath!);
                sw.Write(page.Content);
                sw.Close();

                page.IsSaved = true;
            }
        }

        /// <summary>
        /// Open file from system
        /// </summary>
        /// <param name="tabView">tab control</param>
        /// <param name="path">absolute file path</param>
        public void Open(TabControl tabView, string path)
        {
            // replace %20 (space) if there are any
            path = path.Replace("%20", " ");

            StreamReader sr = new StreamReader(path);
            string content = sr.ReadToEnd();

            _tabs.Add(new TabControlPageViewModelItem
            {
                Header = path.Substring(path.LastIndexOf('/') + 1),
                FilePath = path,
                Content = content,
                Status = "",
                IsSaved = true,
                CloseBtn = true
            });

            // move '+' tab to the end
            _tabs.Move(_tabs.Count - 2, _tabs.Count - 1);

            tabView.SelectedIndex = _tabs.Count - 2;
        }

        /// <summary>
        /// Change editor file according to tab selection
        /// </summary>
        /// <param name="tabView">tab control</param>
        /// <param name="editor">avalonia editor</param>
        public void ChangeSelection(TabControl tabView, TextEditor editor)
        {
            // if '+' was selected, add new tab
            if (tabView.SelectedIndex == _tabs.Count - 1 && _initComplete)
            {
                _tabs.Add(new TabControlPageViewModelItem
                {
                    Header = "new tab",
                    FilePath = "",
                    Content = "",
                    Status = "*",
                    IsSaved = false,
                    CloseBtn = true
                });

                // move '+' tab to the end
                _tabs.Move(_tabs.Count - 2, _tabs.Count - 1);

                tabView.SelectedIndex = _tabs.Count - 2;
            }
            // change editor text according to selected tab
            else if (tabView.SelectedIndex != _selectedIndex && _initComplete)
            {
                _tabs[_selectedIndex].Content = editor.Text;
                bool b = _tabs[tabView.SelectedIndex].IsSaved;
                editor.Text = _tabs[tabView.SelectedIndex].Content;
                if (b)
                {
                    _tabs[tabView.SelectedIndex].IsSaved = true;
                }
                _selectedIndex = tabView.SelectedIndex;
            }
        }

        public void CloseTab(TabControl tabView, TextEditor editor, Button btn)
        {
            TabItem item = (TabItem)btn.Parent.Parent;
            TabControlPageViewModelItem page = (TabControlPageViewModelItem)item.Content;

            if (page.Header == "new tab" && page.Content == "")
            {
                _tabs.Remove(page);
            }
            else
            {
                Save(tabView, editor, page);
                _tabs.Remove(page);
            }
        }
    }
}
