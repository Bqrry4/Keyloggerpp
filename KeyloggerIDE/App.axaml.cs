using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using KeyloggerIDE.ViewModels;
using KeyloggerIDE.Views;

namespace KeyloggerIDE;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += This_ShutdownRequested;
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    protected virtual void This_ShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        Debug.WriteLine($"App.{nameof(This_ShutdownRequested)}");
        OnShutdownRequested(e);
    }

    protected virtual void OnShutdownRequested(ShutdownRequestedEventArgs e)
    {
        ShutdownRequested?.Invoke(this, e);
    }

    public event EventHandler<ShutdownRequestedEventArgs>? ShutdownRequested;
}
