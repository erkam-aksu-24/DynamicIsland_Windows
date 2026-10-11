using DynamicIsland.Application.Ports;
using DynamicIsland.Application.Settings;

namespace DynamicIsland.App.Preferences;

public class JsonPreferencesStore :IUserPreferences
{
    public IslandSettings LoadSettings()
    {
        throw new NotImplementedException();
    }

    public void Save(IslandSettings settings)
    {
        throw new NotImplementedException();
    }
}
