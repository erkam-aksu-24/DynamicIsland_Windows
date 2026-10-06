using DynamicIsland.Application.Media;
using Xunit;

namespace DynamicIsland.Application.Tests;

public class OnTrackChanged_Cache_Test
{
    [Fact]
    public void PassiveSessionTest()
    {
        //ARRANGE
        var orchestrator = new MediaOrchestrator();
        var caught = new List<TrackInfo>();
        orchestrator.ActiveTrackChanged += (_, trackInfo) => caught.Add(trackInfo);

        var trackA = new TrackInfo("Chrome", "Chrome", null);

        //ACT 1 - Spotify ı aktif yap
        orchestrator.OnSessionsChanged(new []{new MediaSessionSnapshot("spotify", IsPlaying: true, LastChangedUtc: DateTimeOffset.Parse("10:00"))});

        //ACT 2 - Chrome nin tracki gelecek. Spotify hala aktif.
        orchestrator.OnTrackChanged("Chrome", trackA);

        //ACT 3 - Spotify ölür, chrome çalmaya başlar.
        orchestrator.OnSessionsChanged(new []{new MediaSessionSnapshot("Chrome", IsPlaying:true, LastChangedUtc:DateTimeOffset.Parse("10:01"))});

        //ASSERT - replay çalıştı mı?
        Assert.Single(caught);
        Assert.Equal(trackA, caught[0]);
    }

    [Fact]
    public void GhostingSessionsTest()
    {
        //ARRANGE
        var orchestrator = new MediaOrchestrator();
        var caught = new List<TrackInfo>();
        orchestrator.ActiveTrackChanged += (_, trackInfo) => caught.Add(trackInfo);

        var trackA = new TrackInfo("Spotify", "Spotify", null);

        //ACT 1 - Spotify aktif
        orchestrator.OnSessionsChanged(new []{new MediaSessionSnapshot("Spotify", IsPlaying:true, LastChangedUtc:DateTimeOffset.Parse("10:00"))});
        orchestrator.OnTrackChanged("Spotify", trackA);

        //ACT 2 - Spotify ölsün
        orchestrator.OnSessionsChanged(Array.Empty<MediaSessionSnapshot>());

        caught.Clear();

        //ACT 3 - Spotify yeniden çalışacak.
        orchestrator.OnSessionsChanged(new []{new MediaSessionSnapshot("Spotify", IsPlaying:true, LastChangedUtc:DateTimeOffset.Parse("10:05"))});

        //ASSERT
        Assert.Empty(caught); //Yöntem 1
        Assert.DoesNotContain(trackA, caught); //Yöntem 2

    }
}
