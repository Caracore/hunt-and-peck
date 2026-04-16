using System.IO;
using System.Text.Json;

namespace HuntAndPeck;

/// <summary>
/// Simple settings class that persists to a JSON file
/// </summary>
public static class AppSettings
{
    private static readonly string SettingsFile;
    private static SettingsData _settings = new();
    private static bool _loaded = false;

    static AppSettings()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appDir = Path.Combine(appData, "HuntAndPeck");
        Directory.CreateDirectory(appDir);
        SettingsFile = Path.Combine(appDir, "settings.json");
    }

    private static void EnsureLoaded()
    {
        if (_loaded) return;
        _loaded = true;
        
        try
        {
            if (File.Exists(SettingsFile))
            {
                var json = File.ReadAllText(SettingsFile);
                _settings = JsonSerializer.Deserialize<SettingsData>(json) ?? new SettingsData();
            }
        }
        catch
        {
            _settings = new SettingsData();
        }
    }

    private static void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFile, json);
        }
        catch
        {
            // Ignore save errors
        }
    }

    public static string FontSize
    {
        get
        {
            EnsureLoaded();
            return _settings.FontSize;
        }
        set
        {
            EnsureLoaded();
            _settings.FontSize = value;
            Save();
        }
    }

    private class SettingsData
    {
        public string FontSize { get; set; } = "14";
    }
}
