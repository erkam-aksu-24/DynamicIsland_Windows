# ADR-0001: Stack choice

Status: Proposed

## Context
Floating media widget needs reliable HWND control (topmost, layered, click-through region),
WinRT access (SMTC, notification listener), quality animations, and high learning value
for Windows platform mastery.

## Decision
WPF on .NET 8 (C#), target framework `net8.0-windows10.0.19041.0`, WinRT interop via
CsWinRT projections. WinUI 3 is the alternative if modern XAML/Mica becomes a priority.

## Consequences
- Direct HWND control via WindowInteropHelper/HwndSource; SMTC is one TFM away.
- Trade-off: no Mica/Acrylic out of the box; DWM interop needed for backdrops.
