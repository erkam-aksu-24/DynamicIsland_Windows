# DynamicIsland.Application

Use cases, ports (interfaces) and domain events. The heart of the domain.

## Hard rules

- MUST NOT reference `Windows.*`, `System.Windows`, WPF, Win32 P/Invoke, or any
  `net8.0-windows`-only API. Target framework stays `net8.0`.
- Platform specifics enter only as interfaces (ports) here; implementations live in
  `DynamicIsland.Adapters` and `DynamicIsland.Platform`.

## Contents

- `UseCases/` — GetNowPlaying, TogglePlayPause, SetVolume, ShowIsland, CollapseIsland...
- `Ports/` — IMediaTransport, IAudioController, INotificationSource, IUserPreferences,
  IAnimationClock, (future) IAiAssistant
- `DomainEvents/` — MediaSessionChanged, TrackChanged, PlaybackStateChanged...
