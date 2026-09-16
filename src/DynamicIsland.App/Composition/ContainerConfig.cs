using Microsoft.Extensions.DependencyInjection;

namespace DynamicIsland.App.Composition;

public static class ContainerConfig
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Shell.IslandWindow>();

        services.AddSingleton<Shell.TrayIconHost>();

        return services.BuildServiceProvider();
    }
}
