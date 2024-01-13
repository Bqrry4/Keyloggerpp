using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using System;
using System.IO;
using System.Xml;

namespace KeyloggerIDE.Views
{
    public partial class SettingsView : UserControl
    {
        public string default_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public SettingsView()
        {
            InitializeComponent();
            if (App.Current.RequestedThemeVariant == ThemeVariant.Light)
                Light.IsChecked = true;
            else
                Dark.IsChecked = true;
            Path.Text = default_path;
        }

        private void setPath(string path)
        {
            default_path = path;
            Path.Text = path;
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
            string file;
            
            if (Light.IsChecked == true)
            {
                App.Current.RequestedThemeVariant = ThemeVariant.Light;
                file = "syntax_definition_light.xshd";
            }
            else
            {
                App.Current.RequestedThemeVariant = ThemeVariant.Dark;
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
