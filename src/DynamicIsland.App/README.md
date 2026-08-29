# DynamicIsland.App

Entry point and composition root: DI wiring, window shell, tray icon, settings persistence.

## Contents

- `Shell/` — main window management: topmost, layered, region, tray, Alt-Tab exclusion
- `Preferences/` — settings load/save (JSON), monitor selection
- `Diagnostics/` — logging, global exception handlers

## Notes

- Currently a class library so the skeleton builds. When App.xaml + MainWindow are added,
  set `<OutputType>WinExe</OutputType>`.
- Single instance enforced via named Mutex; app must not appear in Alt-Tab (WS_EX_TOOLWINDOW).
