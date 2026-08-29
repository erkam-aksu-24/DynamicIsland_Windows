# ADR-0002: Click-through strategy

Status: Proposed

## Context
A full-width transparent topmost window swallows clicks on the taskbar and title bars
below it — the #1 failure mode of this app category. Click-through must be per-region,
not per-window.

## Decision
Window rect equals the pill rect (no full-width strip). Re-apply SetWindowRgn after every
resize. WS_EX_NOACTIVATE + WM_MOUSEACTIVATE→MA_NOACTIVATE so clicks never steal focus.

## Consequences
- Test by clicking the taskbar clock and a title bar while the pill is visible.
- Animated resizes must re-apply the region every frame it changes.
