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

namespace KeyloggerIDE.ViewModels
{
    public class TabControlViewModel
    {
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
                        IsSaved = true
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
                    IsSaved = false
                });
            }

            _tabs.Add(new TabControlPageViewModelItem
            {
                Header = "+", 
                FilePath = "", 
                Content = "",
                Status = "",
                IsSaved = true
            });

            // load syntax highlighting rules
            using (FileStream s = File.Open("syntax_definition.xshd", FileMode.Open))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            
            _initComplete = true;
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
                // write content from text box to file
                StreamWriter sw = new StreamWriter(page.FilePath!);
                sw.Write(page.Content);
                sw.Close();

                page.IsSaved = true;
            }
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
                    IsSaved = false
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
    }
}
