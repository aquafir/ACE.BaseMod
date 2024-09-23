using ACE.Database.Models.Shard;
using ACE.Server.Command;
using ACE.Server.Managers;
using System.Text;
using Tinkering.Bank;
using Tinkering.Floor;

namespace Tinkering;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
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

        SetupFeatures();

        BankExtensions.Init();
        FloorExtensions.Init();

        //DoThing();

        Mod.State = ModState.Running;
    }

    static void DoThing()
    {
        var custom = ContentHelpers.GetCustomWeenies().Where(x => x is not null);
        //&& x.WeenieType == WeenieType.Creature);
        //var sb = new StringBuilder();
        //foreach (var weenie in custom)
        //    sb.Append($"{weenie.GetName()}");
        var str = String.Join(", ", custom.Select(x => x.GetName()));
        ModManager.Log($"\nCustom creatures are:\n{str}");
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


    /// <summary>
    /// Adds additional features to ACE that may be needed by custom loot
    /// </summary>
    private static void SetupFeatures()
    {
        //Add enabled Feature patches
        foreach (var feature in Settings.Features)
        {
            Mod.Harmony.PatchCategory(feature.ToString());

            if (Settings.Verbose)
                ModManager.Log($"Enabled feature: {feature}");
        }

        //Add commands of enabled features
        var commandRegex = String.Join("|", Settings.Features);
        Mod.Container.RegisterCommandCategory(commandRegex);
    }
}
