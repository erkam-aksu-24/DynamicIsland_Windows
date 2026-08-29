# DynamicIsland.Presentation

Views, view-models and animation orchestration.

## Contents

- `Island/` — pill visual states (collapsed / expanded), positioning
- `Panels/` — Media panel first; later Timer, Notifications, Clipboard, AI. Implement the
  pluggable panel pattern (IIslandPanel) so panels never modify core code.
- `Animations/` — spring/easing wrappers and collapsed↔expanded state transitions

## Rules

- May reference Application and Platform; never Adapters directly (App wires them).
- WinRT events must arrive marshalled to the UI thread — view-models never handle threads.
