using System.Windows;
using DynamicIsland.Adapters.SmtcMedia;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicIsland.App;
public partial class App
{
    private Mutex? _mutex;
    private Shell.TrayIconHost? _trayIconHost;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e); // Wpf`in kendi kurulumunu unutma  her override`da base çağrısı.

        var container = Composition.ContainerConfig.Build();
        container.GetRequiredService<Shell.IslandWindow>().Show();
        container.GetRequiredKeyedService<SmtcMediaController>(null).Start();


        var mutex = new Mutex(initiallyOwned: true, name: @"Local/DynamicIsland", out var createdNew);
        if (!createdNew)
        {
            mutex.Dispose();
            Shutdown();
            return;
        }

        _mutex = mutex;
        container.GetRequiredService<Shell.IslandWindow>().Show();
        _trayIconHost = container.GetRequiredService<Shell.TrayIconHost>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIconHost?.Dispose();
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
        base.OnExit(e);
    }
}
