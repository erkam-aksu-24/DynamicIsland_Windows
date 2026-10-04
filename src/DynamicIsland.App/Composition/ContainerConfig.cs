using DynamicIsland.Adapters.SmtcMedia;
using DynamicIsland.Application.Media;
using DynamicIsland.Application.Ports;
using DynamicIsland.Presentation.Island;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicIsland.App.Composition;

public static class ContainerConfig
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Shell.IslandWindow>();
        services.AddSingleton<Shell.TrayIconHost>();
        services.AddSingleton<MediaOrchestrator>();
        services.AddSingleton<IMediaEventSink>(sp => sp.GetRequiredService<MediaOrchestrator>());
        services.AddSingleton<IMediaFeed>(sp => sp.GetRequiredService<MediaOrchestrator>());
        services.AddSingleton<SmtcMediaController>();
        services.AddSingleton<IMediaTransport>(sp => sp.GetRequiredService<SmtcMediaController>());
        services.AddSingleton<IslandViewModel>();



        return services.BuildServiceProvider();
    }
}
