using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.UseCases;

public class TogglePlayPauseHandler(IMediaTransport transport)
{
    public Task ExecuteAsync(string sessionId)
    {
        return transport.TogglePlayPauseAsync(sessionId);
    }
}
