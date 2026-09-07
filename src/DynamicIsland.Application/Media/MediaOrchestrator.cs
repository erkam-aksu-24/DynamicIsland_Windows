using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.Media;

public class MediaOrchestrator(IMediaEventSink? sink) : IDisposable
{
    //Durum
    private List<MediaSessionsSnapshot> _sessions = new List<MediaSessionsSnapshot>();
    private string? _activeId = null;

    //Yayın Noktaları
    event EventHandler<TrackInfo>? ActiveTrackChanged; // <- Buraya '?' koyulmalı
    event EventHandler<bool>? ActivePlaybackChanged;
    event EventHandler<string>? ActiveSessionChanged;

    void OnSessionChanged(List<MediaSessionsSnapshot> snapshot)
    {
        _sessions = snapshot;
        var newActive = SelectActive(snapshot);
        if (newActive != _activeId)
        {
            _activeId = newActive;
            ActiveSessionChanged?.Invoke(this, _activeId);
        }
    }

    // Parametre adını geri 'sessionId' yapıp, class'taki '_activeId' ile eşleşiyor mu kontrol ediyoruz.
    void OnTrackChanged(string sessionId, TrackInfo trackInfo)
    {
        if (sessionId != _activeId) return; // Sadece aktif olan session track değiştirirse bildir

        ActiveTrackChanged?.Invoke(this, trackInfo);
    }

    // Burada da parametre adı 'sessionId' kalmalı ve kontrol edilmeli
    void OnPlaybackStateChanged(string sessionId, bool isPlaying)
    {
        if (sessionId != _activeId) return; // Sadece aktif olan session play/pause değiştirirse bildir

        ActivePlaybackChanged?.Invoke(this, isPlaying);
    }

    private static string? SelectActive(IReadOnlyList<MediaSessionsSnapshot> snapshots)
    {
        // Buradaki '.SessionId' kullanımı DOĞRU ✅
        return (snapshots.Where(s => s.IsPlaying).OrderByDescending(s => s.LastChangedUtc).FirstOrDefault() ??
                snapshots.OrderByDescending(s => s.LastChangedUtc).FirstOrDefault())?.SessionId;
    }


    public void Dispose()
    {
    }
}
