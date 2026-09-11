using DynamicIsland.Application.Media;

namespace DynamicIsland.Application.Ports;

public interface IMediaEventSink
{
    void OnSessionsChanged(IReadOnlyList<MediaSessionSnapshot> sessions);
    void OnTrackChanged(string sessionId, TrackInfo trackInfo);
    void OnPlaybackStateChanged(string sessionId, bool isPlaying);
}
