using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.UseCases;

public class SkipPreviousHandler(IMediaTransport transport)
{
    public Task ExecuteAsync(string sessionId) => transport.PreviousAsync(sessionId);
}
