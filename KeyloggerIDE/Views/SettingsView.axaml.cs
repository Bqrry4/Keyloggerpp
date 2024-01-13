using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using System.IO;
using System.Xml;

namespace KeyloggerIDE.Views
{
    public partial class SettingsView : UserControl
    {
        public string default_path;
        public SettingsView()
        {
            InitializeComponent();
            Path.Text = default_path;
        }

        private void setPath(string path)
        {
            default_path = path;
        }

        private async void ChangePath_OnClick(object? sender, RoutedEventArgs e)
        {
            TopLevel? topLevel = TopLevel.GetTopLevel((Button?)sender);
            var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Pick folder"
            });

            setPath(folder[0].Path.AbsolutePath);
        }

        private void Save_OnClick(object? sender, RoutedEventArgs e)
        {
            var file = "";
            
            if (Light.IsChecked == true)
            {
                Dark.IsChecked = false;
                file = "syntax_definition.xshd";
            }
            else
            {
                Light.IsChecked = false;
                Dark.IsChecked = true;
                file = "syntax_definition_dark.xshd";
            }
            using (FileStream s = File.Open(file, FileMode.Open))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    MainView.Instance._editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}
