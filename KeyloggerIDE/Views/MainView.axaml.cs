using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace KeyloggerIDE.Views;

public partial class MainView : UserControl
{
    private Popup? popup;
    public MainView()
    {
        InitializeComponent();
        popup = this.FindControl<Popup>("About");
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
}
