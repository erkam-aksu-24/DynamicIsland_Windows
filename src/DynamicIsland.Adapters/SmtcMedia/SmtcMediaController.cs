using System.Diagnostics;
using Windows.Media.Control;
using DynamicIsland.Application.Ports;
using DynamicIsland.Application.Media;

namespace DynamicIsland.Adapters.SmtcMedia;

public class SmtcMediaController(IMediaEventSink sink) : IMediaTransport
{
    private SynchronizationContext? _ui;
    private GlobalSystemMediaTransportControlsSessionManager _sessionManager;
    private readonly Dictionary<string, SessionBundle> _sessions = new();

    internal sealed class SessionBundle()
    {
        public GlobalSystemMediaTransportControlsSession? Session { get; init;}
        public DateTimeOffset LastUpdated { get; set;}
    }

    public async void Start()
    {
        try
        {
            Debug.Assert(SynchronizationContext.Current != null);
            _ui = SynchronizationContext.Current;

            _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            _sessionManager.SessionsChanged += OnManagerSessionsChanged;

            foreach (var session in  _sessionManager.GetSessions())
            {
                AttachSession(session);
            }

            PushSnapshot();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[SMTC] start hatası: {e}");
        }
    }

    private void PushSnapshot()
    {
        var snapshots = _sessions.Values.Select(b => new MediaSessionSnapshot(
            b.Session.SourceAppUserModelId,
            b.Session.GetPlaybackInfo().PlaybackStatus ==
            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing,
            b.LastUpdated)).ToList();

        Debug.WriteLine($"[SMTC] snapshot: {snapshots.Count()}");

        _ui.Post(_ => sink.OnSessionsChanged(snapshots.ToArray()), null);
    }

    private void OnManagerSessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
    {
        try
        {
            var current = sender.GetSessions();
            var currentIds = current.Select(s => s.SourceAppUserModelId).ToHashSet();

            foreach (var session in current.Where(s => !_sessions.ContainsKey(s.SourceAppUserModelId)))
            {
                AttachSession(session);
            }

            foreach (var id in _sessions.Keys.ToList().Where(id => !currentIds.Contains(id)))
            {
                DetachSession(id);
            }

            PushSnapshot();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[SMTC] sessions hatası: {e}");
        }
    }

    private void OnPlaybackChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
    {
        try
        {
            if (_sessions.TryGetValue(sender.SourceAppUserModelId, out var sessionBundle))
            {
                sessionBundle.LastUpdated = DateTimeOffset.UtcNow;
                PushSnapshot();
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[SMTC] playback hatası {e}");
        }
    }

    private void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender,
        MediaPropertiesChangedEventArgs args)
    {
        // TODO Tur B : track bilgisi buradan çekilecek
        Debug.WriteLine("[SMTC] media properties changed");
    }

    private void AttachSession(GlobalSystemMediaTransportControlsSession session)
    {
        _sessions[session.SourceAppUserModelId] = new SessionBundle { Session = session};
        session.PlaybackInfoChanged += OnPlaybackChanged; // method group — her seferinde aynı referans
        session.MediaPropertiesChanged += OnMediaPropertiesChanged;
    }

    private void DetachSession(string id)
    {
        if (_sessions.Remove(id, out var bundle))
        {
            Debug.Assert(bundle.Session != null, "bundle.Session != null");
            bundle.Session.PlaybackInfoChanged -= OnPlaybackChanged;
            bundle.Session.MediaPropertiesChanged -= OnMediaPropertiesChanged;
        }
    }

    public Task TogglePlayPauseAsync(string sessionId) => Task.CompletedTask;

    public Task NextAsync(string sessionId) => Task.CompletedTask;

    public Task PreviousAsync(string sessionId) => Task.CompletedTask;

}
