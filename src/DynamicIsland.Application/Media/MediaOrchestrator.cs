using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.Media;

public class MediaOrchestrator : IMediaEventSink, IMediaFeed
{
    private readonly Dictionary<string,TrackInfo> _trackCache = new();

    //Durum
    private string? _activeId;


    //Yayın Noktaları
    public string? ActiveSessionId => _activeId;
    public event EventHandler<TrackInfo>? ActiveTrackChanged; // <- Buraya '?' koyulmalı
    public event EventHandler<bool>? ActivePlaybackChanged;
    public event EventHandler<string?>? ActiveSessionChanged;

    public void OnSessionsChanged(IReadOnlyList<MediaSessionSnapshot> sessions)
    {
        var live = sessions.Select(s => s.SessionId).ToHashSet();

        var dead = _trackCache.Keys.Where(id => !live.Contains(id)).ToList();
        foreach (var id in dead) _trackCache.Remove(id);

        var newActive = SelectActive(sessions);
        if (newActive != _activeId)
        {
            _activeId = newActive;
            ActiveSessionChanged?.Invoke(this, _activeId);
            if (_activeId != null && _trackCache.TryGetValue(_activeId, out var cached))
            {
                ActiveTrackChanged?.Invoke(this, cached); // Önbelleğr alınmış oturuuda invoke eder.
            }

        }
    }

    public void OnTrackChanged(string sessionId, TrackInfo trackInfo)
    {
        _trackCache[sessionId] = trackInfo;
        if (sessionId == _activeId)
        {
            ActiveTrackChanged?.Invoke(this, trackInfo);
        }
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












