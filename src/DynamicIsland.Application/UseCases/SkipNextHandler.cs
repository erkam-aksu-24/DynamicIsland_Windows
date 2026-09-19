using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.UseCases;

public class SkipNextHandler(IMediaTransport transport)
{
    public Task ExecuteAsync(string sessionId) => transport.NextAsync(sessionId);
}
