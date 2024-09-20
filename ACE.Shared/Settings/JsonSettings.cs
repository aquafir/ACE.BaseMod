namespace ACE.Shared.Settings;
public class JsonSettings<T>(string filePath) : SettingsContainer<T>(filePath) where T : ISettings, new()
{
    static private JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public override async Task CreateAsync()
    {
        var json = JsonSerializer.Serialize(Settings, _serializeOptions);
        await File.WriteAllTextAsync(SettingsPath, json);

        if (!SettingsInfo.RetryWrite(jsonString, RETRIES))
        {
            ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
        }
    }

    public override async Task LoadAsync(string settingsFile)
    {
        Settings = JsonSerializer.Deserialize<T>(settingsFile);
    }
}


