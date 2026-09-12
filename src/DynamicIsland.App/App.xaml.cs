using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicIsland.App;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e); // Wpf`in kendi kurulumunu unutma  her override`da base çağrısı.

        var container = Composition.ContainerConfig.Build();
        container.GetRequiredService<Shell.IslandWindow>().Show();
    }
}
