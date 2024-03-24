extern alias Json;
using Json.System.Text.Json;
using Json.System.Text.Json.Serialization;

using ACE.Entity.Models;
using System.Runtime.CompilerServices;
using ACE.Server.Factories;

namespace Ironman;

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
    private static int Alive;

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

        PatchFlaggingCategories();
        PatchRestrictionCategories();

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

    private void PatchFlaggingCategories()
    {
        foreach (var p in Settings.FlagItemEvents)
            Mod.Harmony.PatchCategory(p);
    }
    private void PatchRestrictionCategories()
    {
        foreach (var p in Settings.Restrictions)
            Mod.Harmony.PatchCategory(p);
    }

    [CommandHandler("di", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleDie(Session session, params string[] parameters)
    {
        session.Player.TakeDamage(null, DamageType.Fire, 1000000);
    }


    static Dictionary<CharacterOption, bool> defaultOptions = new()
    {
        [CharacterOption.SideBySideVitals] = true,
        [CharacterOption.AcceptCorpseLootingPermissions] = true,
        [CharacterOption.AutomaticallyAcceptFellowshipRequests] = true,
        [CharacterOption.AlwaysDaylightOutdoors] = true,
    };
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerFactory), nameof(PlayerFactory.CharacterCreateSetDefaultCharacterOptions), new Type[] { typeof(Player) })]
    public static void PostCharacterCreateSetDefaultCharacterOptions(Player player)
    {
        foreach(var option in defaultOptions)
            player.SetCharacterOption(option.Key, option.Value);
    }

}
