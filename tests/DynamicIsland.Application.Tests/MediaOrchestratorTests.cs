using DynamicIsland.Application.Media;
using Xunit;


namespace DynamicIsland.Application.Tests;

public class MediaOrchestratorTests
{

    [Fact]
    public void OnSessionsChanged_PlayingSessionBeatsPausedOne()
    {
        // ARRANGE
        var orchestrator = new MediaOrchestrator();

        var sessions = new[]
        {
            new MediaSessionSnapshot("spotify", IsPlaying: true,  LastChangedUtc: DateTimeOffset.Parse("10:00")),
            new MediaSessionSnapshot("chrome",  IsPlaying: false, LastChangedUtc: DateTimeOffset.Parse("10:01")),
        };

        // ACT
        orchestrator.OnSessionsChanged(sessions);

        // ASSERT
        Assert.Equal("spotify", orchestrator.ActiveSessionId);
    }

    [Fact]
    public void OnTrackChanged_EventOnlyFiresForActiveSession()
    {
        // ARRANGE
        var orchestrator = new MediaOrchestrator();
        var caught = new List<TrackInfo>();
        orchestrator.ActiveTrackChanged += (_, trackInfo) => caught.Add(trackInfo);
        orchestrator.OnSessionsChanged(new[] { new MediaSessionSnapshot("spotify", IsPlaying: true, LastChangedUtc:DateTimeOffset.Parse("10:00")) });

// ACT + ASSERT (iki aşama)
        orchestrator.OnTrackChanged("chrome",   new TrackInfo("Başka Şarkı", "Sanatçı", "Albüm"));
        Assert.Empty(caught);                                      // pasif oturumdan sızma YOK
        orchestrator.OnTrackChanged("spotify", new TrackInfo("Başka Şarkı", "Sanatçı", "Albüm"));
        Assert.Single(caught);                                     // aktif oturumdan TAM BİR haber
    }

    [Fact]
    public void OnSessionsChanged_EmptyList_ClearsActiveSession()
    {
        var orchestrator = new MediaOrchestrator();
        var caught = new List<string?>();  // event tipi EventHandler<string?> — listeye dikkat

        orchestrator.ActiveSessionChanged += (_, id) => caught.Add(id);

        orchestrator.OnSessionsChanged(new[]{new MediaSessionSnapshot("spotify", IsPlaying:true, LastChangedUtc:DateTimeOffset.Parse("10:00") )}); // ACT 1: aktif seçilir
        orchestrator.OnSessionsChanged(new[]{new MediaSessionSnapshot("spotify", IsPlaying:true, LastChangedUtc:DateTimeOffset.Parse("10:00") )});
        orchestrator.OnSessionsChanged(Array.Empty<MediaSessionSnapshot>());  // ACT 2: Spotify öldü

        Assert.Null(orchestrator.ActiveSessionId);
        Assert.Equal(2, caught.Count); // 2 event: seçim + temizlenme
        Assert.Equal("spotify", caught[0]);
        Assert.Null(caught[1]);  // ikinci event null getirdi
    }
}
