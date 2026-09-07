using DynamicIsland.Application.Media;

namespace DynamicIsland.Application.Ports;

public interface IMediaEventSink
{
    void OnSessionsChanged(IReadOnlyList<MediaSessionsSnapshot> sessions);
    void OnTrackChanged(string sessionId, TrackInfo trackInfo);
    void OnPlaybackStateChanged(string sessionId, bool isPlaying);
}
