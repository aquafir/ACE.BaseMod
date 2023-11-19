using ACE.Server.Network;

namespace ChatFilter;

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

        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        PatchCategories();
        SetupFilter();

        Mod.State = ModState.Running;
    }

    public void Shutdown()
    {
        //if (Mod.State == ModState.Running)
        // Shut down enabled mod...

        //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
        //SaveSettings();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    private void PatchCategories()
    {
        if (Settings.FilterChat)
            Mod.Harmony.PatchCategory(Settings.ChatCategory);

        if (Settings.FilterTells)
            Mod.Harmony.PatchCategory(Settings.TellCategory);
    }

    static ProfanityFilter.ProfanityFilter filter = new();
    private void SetupFilter()
    {
        //Create default or empty list
        filter = Settings.UseDefaultList ? new ProfanityFilter.ProfanityFilter() : new ProfanityFilter.ProfanityFilter(new string[] { });

        //Load blacklist
        var watch = Stopwatch.StartNew();
        if (File.Exists(Settings.BlackList))
        {
            var list = File.ReadAllLines(Settings.BlackList);
            filter.AddProfanity(list);

            ModManager.Log($"ChatFilter: Blacklisted {list.Length} words after {watch.ElapsedMilliseconds} ms");
            watch.Restart();

        }
        if (File.Exists(Settings.WhiteList))
        {
            var list = File.ReadAllLines(Settings.WhiteList);
            filter.RemoveProfanity(list);

            ModManager.Log($"ChatFilter: Whitelisted {list.Length} words after {watch.ElapsedMilliseconds} ms");
            watch.Stop();
        }
    }

    public static bool TryHandleToxicity(string message, Player player, ChatSource source)
    {
        var hasProfanity = filter.ContainsProfanity(message);

        //Unsure about a short circuit.  Allow non-profane messages, or block all?
        if (!hasProfanity)
            return false;

        player.IncreaseChatInfractionCount();

        //Check shadowban before anything.  Skips recipient but sends user?
        if (Settings.ShadowBan && !player.IsShadowBanned())
            player.SetShadowBanned(true);

        if(player.IsShadowBanned())
            if (TryHandleShadowBan(message, player, source))
                return true;

        //Replace words but don't stop
        if (Settings.CensorText)
        {
            message = filter.CensorString(message);
            return false;
        }

        if (Settings.GagPlayer)
            player.GagPlayer();

        if (Settings.BanAccount)
            player.BanPlayerAccount();

        //player.SendMessage($"Banned for: {message}");

        //var chain = new ActionChain();
        //chain.AddDelaySeconds(3);
        //chain.AddAction(player, () => player.Session.LogOffPlayer(true));
        //chain.EnqueueChain();

        return true;
    }

    public static bool TryHandleShadowBan(string message, Player player, ChatSource source)
    {
        if (!player.IsShadowBanned()) return false;

        return false;
    }
}
public enum ChatSource
{
    Chat,
    Tell
}
