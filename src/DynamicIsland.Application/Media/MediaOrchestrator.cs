using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.Media;

public class MediaOrchestrator : IMediaEventSink
{
    //Durum
    private string? _activeId;

    //Yayın Noktaları
    public string? ActiveSessionId => _activeId;
    public event EventHandler<TrackInfo>? ActiveTrackChanged; // <- Buraya '?' koyulmalı
    public event EventHandler<bool>? ActivePlaybackChanged;
    public event EventHandler<string?>? ActiveSessionChanged;

    public void OnSessionsChanged(IReadOnlyList<MediaSessionSnapshot> sessions)
    {

        var newActive = SelectActive(sessions);
        if (newActive != _activeId)
        {
            _activeId = newActive;
            ActiveSessionChanged?.Invoke(this, _activeId);
        }
    }

    public void OnTrackChanged(string sessionId, TrackInfo trackInfo)
    {
        if (sessionId != _activeId) return; // Sadece aktif olan session track değiştirirse bildir

        ActiveTrackChanged?.Invoke(this, trackInfo);
    }

    public void OnPlaybackStateChanged(string sessionId, bool isPlaying)
    {
        if (sessionId != _activeId) return; // Sadece aktif olan session play/pause değiştirirse bildir

        ActivePlaybackChanged?.Invoke(this, isPlaying);
    }

    private static string? SelectActive(IReadOnlyList<MediaSessionSnapshot> snapshots)
    {
        // Buradaki '.SessionId' kullanımı DOĞRU ✅
        return (snapshots.Where(s => s.IsPlaying).OrderByDescending(s => s.LastChangedUtc).FirstOrDefault() ??
                snapshots.OrderByDescending(s => s.LastChangedUtc).FirstOrDefault())?.SessionId;
    }

}












