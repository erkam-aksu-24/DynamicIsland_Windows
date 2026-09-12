using DynamicIsland.Application.Ports;

namespace DynamicIsland.Application.UseCases;

public class AdjustVolumeHandler(IAudioController audioController)
{
    public void Execute(double delta)
    {
        var current = audioController.GetVolume();
        audioController.SetVolume(Math.Clamp(current + delta, 0.0, 1.0));
    }
}
