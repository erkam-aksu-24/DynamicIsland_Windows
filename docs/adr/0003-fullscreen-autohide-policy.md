# ADR-0003: Fullscreen auto-hide policy

Status: Proposed

## Context
Topmost windows still cover borderless-fullscreen games and videos; true-exclusive
fullscreen covers the island anyway. Fighting it breaks games.

## Decision
Poll SHQueryUserNotificationState (~1 s while active); auto-hide on
QUNS_RUNNING_D3D_FULL_SCREEN / QUNS_BUSY. Add a per-app hide list later.

## Consequences
- The island hides instead of fighting; restores when the state clears.
- Detection logic lives in DynamicIsland.Platform/Fullscreen.
