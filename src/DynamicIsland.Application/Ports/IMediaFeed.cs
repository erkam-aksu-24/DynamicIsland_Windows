using DynamicIsland.Application.Media;

namespace DynamicIsland.Application.Ports;

public interface IMediaFeed
{
    public event EventHandler<TrackInfo>? ActiveTrackChanged;
    public event EventHandler<bool>? ActivePlaybackChanged;
    public event EventHandler<string?>? ActiveSessionChanged;
}
