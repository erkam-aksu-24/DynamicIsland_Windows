using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.Tests.Fakes;

public class FakeAudioController : IAudioController
{
    public FakeAudioController(double initial = 0.5) => Current = initial;
    public double Current;
    public double LastSet { get; private set; }

    public double GetVolume() => Current;
    public void SetVolume(double volume) => LastSet = volume;
}
