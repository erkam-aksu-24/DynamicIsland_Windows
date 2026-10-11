using System.Diagnostics;
using System.IO;
using System.Text.Json;
using DynamicIsland.Application.Ports;
using DynamicIsland.Application.Settings;

namespace DynamicIsland.App.Preferences;

public class JsonPreferencesStore : IUserPreferences
{
    private readonly string _roamingPath;
    private readonly string _fileFolder;
    private readonly string _filePath;

    public JsonPreferencesStore()
    {
        _roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _fileFolder = Path.Combine(_roamingPath, "DynamicIsland");
        _filePath = Path.Combine(_fileFolder, "settings.json");
        Directory.CreateDirectory(_fileFolder);

    }

    public IslandSettings LoadSettings()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return new IslandSettings();
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<IslandSettings>(json);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return new IslandSettings();
        }
    }

    public void Save(IslandSettings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception e)
        {
           Debug.WriteLine(e);
        }
    }
}
