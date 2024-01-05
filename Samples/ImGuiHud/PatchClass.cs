namespace ImGuiTest;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    const int RETRIES = 10;

    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);

    private JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

        if (!settingsInfo.RetryWrite(jsonString, RETRIES))
        {
            ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
        }
    }

    private void LoadSettings()
    {
        if (!settingsInfo.Exists)
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
    public void Start()
    {
        //Need to decide on async use
        Mod.State = ModState.Loading;
        LoadSettings();

        StartImGui();

        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        Mod.State = ModState.Running;
    }

    public void Shutdown()
    {
        if (Mod.State == ModState.Running)
            StopImGui();

        //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
        //SaveSettings();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    private void StartImGui()
    {
        try
        {
            Task.Run(async () => StartOverlay()).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            ModManager.Log(ex.Message, ModManager.LogLevel.Error);
        }
    }
    private void StopImGui()
    {
        try
        {
            Overlay?.Close();
            Thread.Sleep(1000);
            Overlay?.Dispose();
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            ModManager.Log(ex.Message, ModManager.LogLevel.Error);
        }
    }

    public static GUI Overlay;
    private static async Task<GUI> StartOverlay()
    {
        Overlay = new();
        await Overlay.Run();
        return Overlay;
    }

    [CommandHandler("close", AccessLevel.Admin, CommandHandlerFlag.None, 0)]
    public static void HandleSelect(Session session, params string[] parameters)
    {
        Overlay.Close();
        Overlay.Run();
        //ImGui.ShowFontSelector("Foo");
    }

    [CommandHandler("start", AccessLevel.Admin, CommandHandlerFlag.None, 0)]
    public static void HandleOpen(Session session, params string[] parameters)
    {
        Overlay.Start();
        //ImGui.ShowFontSelector("Foo");
    }
    [CommandHandler("run", AccessLevel.Admin, CommandHandlerFlag.None, 0)]
    public static void HandleRun(Session session, params string[] parameters)
    {
        Overlay.Run();
        //ImGui.ShowFontSelector("Foo");
    }

}

