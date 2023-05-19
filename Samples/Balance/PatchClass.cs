namespace Balance;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
    const int RETRIES = 10;

    public static Settings Settings = new();
    private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
    private static FileInfo settingsInfo = new(settingsPath);

    private static List<AngouriMathPatch> enabledPatches = new();

    private static JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);
        if (!settingsInfo.RetryWrite(jsonString, RETRIES))
        {
            ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
        }
    }

    private static void LoadSettings()
    {
        if (!File.Exists(settingsInfo.FullName))
        {
            ModManager.Log($"Creating {settingsInfo}...");
            SaveSettings();
        }
        else
            ModManager.Log($"Loading settings from {settingsPath}...");

        if (!settingsInfo.RetryRead(out string jsonString, RETRIES))
        {
            Mod.State = ModState.Error;
            return;
        }

        try
        {
            Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
        }
        catch (Exception)
        {
            ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
            return;
        }
    }
    #endregion

    #region Start/Shutdown
    public static void Start()
    {
        //Need to decide on async use
        Mod.State = ModState.Loading;

        LoadSettings();

        enabledPatches.Clear();
        bool defaultFormulaUsed = false; 

        var sb = new StringBuilder("\n");
        foreach (var patchSettings in Settings.Formulas)
        {
            //Basic check for if the patch is supplying the default formula to the settings.  Settings saved if true
            if (string.IsNullOrWhiteSpace(patchSettings.Formula)) defaultFormulaUsed = true;

            var patch = patchSettings.CreatePatch();

            try
            {
                if (patchSettings.Enabled)
                {
                    patch.Start();
                    enabledPatches.Add(patch);
                }
                sb.AppendLine($"{patchSettings.Type} patched with:\n  {patch.Formula}");

            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to patch {patchSettings.Type}: {ex.Message}", ModManager.LogLevel.Error);
                sb.AppendLine($"Failed to patch {patchSettings.Type}:\n  {patch.Formula}");
            }
        }
        ModManager.Log(sb.ToString());

        if (defaultFormulaUsed)
            SaveSettings();

        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        Mod.State = ModState.Running;
    }

    public static void Shutdown()
    {
        //if (Mod.State == ModState.Running)

        //Shutdown/unpatch everything on settings change to support repatching by category
        foreach (var patch in enabledPatches)
        {
            patch.Shutdown();
        }
        Mod.Harmony.UnpatchAll();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    #region Patches
    #endregion
}