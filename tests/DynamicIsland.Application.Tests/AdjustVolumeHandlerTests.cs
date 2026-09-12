using DynamicIsland.Application.Tests.Fakes;
using DynamicIsland.Application.UseCases;
using Xunit;

namespace DynamicIsland.Application.Tests;

public class AdjustVolumeHandlerTests
{
    [Fact] void AdjustVolume_AddsDelta()
    {
        var audio = new FakeAudioController(0.50);
        new AdjustVolumeHandler(audio).Execute(+0.05);
        Assert.Equal(0.55, audio.LastSet, precision: 2); // "virgülden sonra 2 haneye kadar karşılaştır"
    }
    [Fact] void AdjustVolume_ClampsAtOne()
    {
        var audio = new FakeAudioController(0.98);
        new AdjustVolumeHandler(audio).Execute(+0.05);
        Assert.Equal(1.0, audio.LastSet);
    }
}
