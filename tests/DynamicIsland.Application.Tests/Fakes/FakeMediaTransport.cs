using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.Tests.Fakes;

public class FakeMediaTransport:IMediaTransport
{
    private List<string> Calls;

    public FakeMediaTransport(List<string> calls)
    {
        Calls = calls;
    }

    public Task TogglePlayPauseAsync(string sessionId)
    {
        Calls.Add($"toogle:{sessionId}");
        return Task.CompletedTask;
    }

    public Task NextAsync(string sessionId)
    {
        Calls.Add($"next:{sessionId}");
        return Task.CompletedTask;
    }

    public Task PreviousAsync(string sessionId)
    {
        Calls.Add($"previous:{sessionId}");
        return Task.CompletedTask;
    }
}
