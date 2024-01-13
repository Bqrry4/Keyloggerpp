using Avalonia.Controls;
using Avalonia.Interactivity;

namespace KeyloggerIDE.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void setPath(string path)
        {
            Path.Text = path;
        }

        private void ChangePath_OnClick(object? sender, RoutedEventArgs e)
        {

        }
    }
}
