namespace ACE.Shared.Mods;
public class JsonSettings<T>(string filePath = "Settings.json") : SettingsContainer<T>(filePath) where T : class?, new()
{
    static private JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        IncludeFields = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,         
    };

    protected override async Task<T> LoadSettingsAsync()
    {
        if (!File.Exists(SettingsPath))
            return null;

        // Try loading the settings from file
        using (FileStream fs = File.OpenRead(SettingsPath))
        {
            return await JsonSerializer.DeserializeAsync<T>(fs, _serializeOptions);
        }
    }

    protected override async Task<bool> SaveSettingsAsync(T settings)
    {
        try
        {
            using (FileStream fs = File.Create(SettingsPath))
            {
                await JsonSerializer.SerializeAsync(fs, settings, _serializeOptions);
                return true;
            }
        }
        catch (Exception ex)
        {
            ModManager.Log($"Error saving settings to ${SettingsPath}: {ex.Message}", ModManager.LogLevel.Error);
            return false;
        }
    }
}
