using DynamicIsland.Application.Settings;

namespace DynamicIsland.Application.Ports;

public interface IUserPreferences
{
    IslandSettings LoadSettings();
    void Save(IslandSettings settings);
}
