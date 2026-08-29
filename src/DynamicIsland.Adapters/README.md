# DynamicIsland.Adapters

Implementations of Application ports over Windows APIs.

## Contents

- `SmtcMedia/` — WinRT SMTC (`GlobalSystemMediaTransportControlsSessionManager`) →
  `IMediaTransport`. Track info, playback state, play/pause/next/prev.
- `CoreAudio/` — `IAudioEndpointVolume` (master) / `IAudioSessionManager2` (per-app) →
  `IAudioController`.
- `NotificationListener/` — (v2) `UserNotificationListener` → `INotificationSource`.
  Requires user consent in Windows Settings.
- `ClipboardListener/` — (v2) Win32 clipboard listener, ephemeral chip only.

## Rules

- Raise domain events on a captured SynchronizationContext/TaskScheduler — never let
  WinRT/Win32 callback threads leak upward.
- Consume WinRT streams deterministically (thumbnails: convert once, cache by track id, dispose).
