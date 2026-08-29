# DynamicIsland

A Dynamic Island-style floating media widget for Windows — C#/.NET 8, WPF + WinRT interop.

## Build

```bash
dotnet build DynamicIsland.sln
dotnet test DynamicIsland.sln
```

> `DynamicIsland.App` currently builds as a class library. When you add `App.xaml` /
> `App.xaml.cs` and the first window, switch its `OutputType` to `WinExe`.

## Architecture

Dependency direction (never violated):

```text
App ──► Presentation ──► Application ◄── Adapters
  │            │              ▲
  └──────────► Platform ───────┘   (Adapters + Platform reference Application)
```

- **DynamicIsland.Application** — use cases, ports (interfaces), domain events. PURE: no
  `Windows.Media`, no `System.Windows`, no Win32/WinRT references. Ever.
- **DynamicIsland.Adapters** — implements Application ports using platform APIs
  (SMTC, Core Audio, notification listener, clipboard).
- **DynamicIsland.Platform** — raw Win32/DWM helpers: HWND utils, DPI, fullscreen detection.
- **DynamicIsland.Presentation** — views, view-models, animation orchestration.
- **DynamicIsland.App** — composition root (DI), window shell, tray, settings persistence.

The smell to grep for: `using Windows.Media;` or `using System.Windows;` inside
`DynamicIsland.Application`. If it appears, the architecture has failed.

## Future-proofing

- Panels are pluggable modules (`IIslandPanel` pattern) — a new feature is a new panel + adapter, not a core rewrite.
- A built-in AI assistant will be added later via an `IAiAssistant` port in Application. Door stays open; no AI code for now.

## Roadmap

See `docs/adr/` for architecture decisions. Phases:

1. Phase 0 — window spike (topmost pill, transparent corners, DPI, click-through)
2. Phase 1 — media MVP (SMTC track info + play/pause/next/prev + volume wheel)
3. Phase 2 — polish (acrylic, session picker, settings UI, soak test)
4. Phase 3 — notifications, clipboard chip, timer, AI panel
