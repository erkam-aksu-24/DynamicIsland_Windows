namespace DynamicIsland.Application.Ports;

public interface IMediaTransport
{
    Task TogglePlayPauseAsync(string sessionId);
    Task NextAsync(string sessionId);
    Task PreviousAsync(string sessionId);
}
