# DynamicIsland.Platform

Raw Win32/DWM helpers. No business logic.

## Contents

- `Hwnd/` — window styles (WS_POPUP, WS_EX_TOPMOST, WS_EX_LAYERED, WS_EX_NOACTIVATE,
  WS_EX_TOOLWINDOW), SetWindowRgn, WM_MOUSEACTIVATE → MA_NOACTIVATE
- `Dpi/` — PerMonitorV2 awareness, WM_DPICHANGED handling, monitor work-area enumeration
- `Fullscreen/` — SHQueryUserNotificationState polling, foreground-window analysis
  for auto-hide during fullscreen apps/games

## Rules

- P/Invoke declarations live here only (`LibraryImport`/`DllImport`).
- Expose small static/instance helpers; no knowledge of island features.
