namespace DynamicIsland.Application.Settings;

public record IslandSettings(
    double WheelVolumeStep = 0.05,
    int CollapseAfterSeconds = 5,
    int MonitorIndex = 0);
