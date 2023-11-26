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

namespace KeyloggerIDE.ViewModels
{
    public class TabControlViewModel
    {
        private const string SavefilePath = "savefile.xml";

        private ObservableCollection<TabControlPageViewModelItem> tabs = new ObservableCollection<TabControlPageViewModelItem>();

        public ObservableCollection<TabControlPageViewModelItem> Tabs
        {
            get => tabs;
        }

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

        public void initTabControl()
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

                    tabs.Add(new TabControlPageViewModelItem
                    {
                        Header = item.InnerText, 
                        FilePath = item.Attributes["path"].Value, 
                        Content = content,
                        Status = "",
                        IsSaved = true
                    });
                }
            }
            else
            {
                tabs.Add(new TabControlPageViewModelItem
                {
                    Header = "new tab",
                    FilePath = "",
                    Content = "",
                    Status = "*",
                    IsSaved = false
                });
            }

            tabs.Add(new TabControlPageViewModelItem
            {
                Header = "+", 
                FilePath = "", 
                Content = "",
                Status = "",
                IsSaved = true
            });
        }

        /// <summary>
        /// Handle scroll for line numeration 
        /// </summary>
        /// <param name="sender">target text box</param>
        /// <param name="e">(currently unused)</param>
        public void handleScroll(object? sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ItemsControl lineNums = textBox.Parent.Parent.GetLogicalDescendants().OfType<ItemsControl>().First();

            // count number of lines in the text
            int lineCount = textBox.Text.Count(c => c == '\n') + 1;
            if (lineNums.Items.Count < lineCount) // add needed line numbers
            {
                for (int i = lineNums.Items.Count + 1; i <= lineCount; ++i)
                {
                    lineNums.Items.Add(i);
                }
            }
            else if (lineNums.Items.Count > lineCount) // remove surplus
            {
                for (int i = lineNums.Items.Count - 1; i >= lineCount; --i)
                {
                    lineNums.Items.RemoveAt(i);
                }
            }

            // scroll to the same position
            ScrollViewer lineScroll = (ScrollViewer)lineNums.Parent.Parent;
            ScrollViewer editorScroll = (ScrollViewer)textBox.Parent;
            lineScroll.Offset = editorScroll.Offset;
        }

        /// <summary>
        /// Mark file as not saved ('*') if it was changed
        /// </summary>
        /// <param name="sender">target text box</param>
        /// <param name="tabView">target tab control</param>
        public void changeFileStatus(object? sender, TabControl tabView)
        {
            TextBox textBox = (TextBox)sender;
            if (tabs[tabView.SelectedIndex].IsSaved && tabs[tabView.SelectedIndex].Content != textBox.Text)
            {
                tabs[tabView.SelectedIndex].IsSaved = false;
            }
        }

        /// <summary>
        /// Save file associated with selected tab
        /// </summary>
        /// <param name="tabView">target tab control</param>
        async public void Save(TabControl tabView)
        {
            if (tabView.SelectedItem == null)
                return;

            // if file path is null or empty, open a save file dialog
            TabControlPageViewModelItem page = (TabControlPageViewModelItem)tabView.SelectedItem;
            if (string.IsNullOrEmpty(page.FilePath))
            {
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
                    streamWriter.Write(page.Content);

                    page.Header = file.Name;
                    page.FilePath = file.Path.AbsolutePath;
                    page.IsSaved = true;
                }
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
    }
}
