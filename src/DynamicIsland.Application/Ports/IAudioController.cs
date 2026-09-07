namespace DynamicIsland.Application.Ports;

public interface IAudioController
{
    double GetVolume();
    void SetVolume(double volume);
}
