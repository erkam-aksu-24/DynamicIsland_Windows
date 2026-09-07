using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.UseCases;

public class TogglePlayPauseHandler(IMediaTransport transport)
{
    Task ExecuteAsync(string sessionId)
    {
        return transport.TooglePlayPauseAsync(sessionId);
    }
}
