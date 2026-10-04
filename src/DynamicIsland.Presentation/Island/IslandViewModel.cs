using DynamicIsland.Application.Media;
using DynamicIsland.Application.Ports;

namespace DynamicIsland.Presentation.Island;

public class IslandViewModel : ObservableObject
{
    private readonly IMediaFeed _feed;

    public IslandViewModel(IMediaFeed feed)
    {
        _feed = feed;

        _feed.ActiveTrackChanged += OnTrackChanged;
        _feed.ActivePlaybackChanged += (_, p) => IsPlaying = p;
        _feed.ActiveSessionChanged += OnSessionChanged;
    }

    private void OnSessionChanged(object? sender, string? sessionId)
    {
        if (sessionId == null)
        {
            IsPlaying = false;
        }
    }

    private void OnTrackChanged(object? sender, TrackInfo e)
    {
        Title = e.Title;
        Artist = e.Artist;
    }

    private string _title = "";

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;
            _title = value;
            OnPropertyChanged();
        }
    }
    private string _artist = "";

    public string Artist
    {
        get => _artist;
        set
        {
            if (_artist == value) return;
            _artist = value;
            OnPropertyChanged();
        }
    }

    private bool _isPlaying = false;

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (_isPlaying == value) return;
            _isPlaying = value;
            OnPropertyChanged();
        }
    }


}
