namespace DynamicIsland.Application.Ports;

public interface IMediaTransport
{
    Task TooglePlayPauseAsync(string sessionId);
    Task NextAsync(string sessionId);
    Task PreviousAsync(string sessionId);
}
