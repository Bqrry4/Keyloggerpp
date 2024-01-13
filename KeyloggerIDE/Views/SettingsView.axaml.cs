using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace KeyloggerIDE.Views
{
    public partial class SettingsView : Window
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
            TopLevel? topLevel = GetTopLevel((Button?)sender);
            var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Pick folder"
            });

            setPath(folder[0].Path.AbsolutePath);
        }
    }
}
