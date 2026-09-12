using DynamicIsland.Application.Tests.Fakes;
using DynamicIsland.Application.UseCases;
using Xunit;

namespace DynamicIsland.Application.Tests;

public class TogglePlayPauseHandlerTests
{
    [Fact]
    public async Task ExecuteAsync_DelegatesToTransport()
    {
        var calls = new List<string>();
        var handler = new TogglePlayPauseHandler(new FakeMediaTransport(calls));
        await handler.ExecuteAsync("spotify");
        Assert.Contains("toogle:spotify", calls);
    }
}
