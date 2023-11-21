using ACE.Server.Command;
using ACE.Server.Network;

namespace CustomLoot;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
    const int RETRIES = 10;

    public static Settings Settings = new();
    private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
    private static FileInfo settingsInfo = new(settingsPath);

    private static JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
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
    public static void Start()
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
        SetupMutators();

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

    /// <summary>
    /// Adds additional features to ACE that may be needed by custom loot
    /// </summary>
    private static void SetupFeatures()
    {
        foreach (var feature in PatchClass.Settings.Features)
        {
            Mod.Harmony.PatchCategory(nameof(feature));
            ModManager.Log($"Enabled feature: {feature}");
        }
    }

    private static List<Mutator> enabledPatches = new();
    private static void SetupMutators()
    {
        enabledPatches.Clear();

        foreach (var mutatorOptions in Settings.Mutators)
        {
            if (!mutatorOptions.Enabled)
                continue;

            try
            {
                var mutator = mutatorOptions.CreateMutator();
                mutator.Start();
                enabledPatches.Add(mutator);

                if (PatchClass.Settings.Verbose)
                    ModManager.Log($"Enabled mutator: {mutatorOptions.PatchType}");
            }
            catch (Exception ex)
            {
                if (PatchClass.Settings.Verbose)
                    ModManager.Log($"Failed to patch {mutatorOptions.PatchType}: {ex.Message}", ModManager.LogLevel.Error);
            }
        }
    }

    /// <summary>
    /// Entry point for mutation.  After loot is generated it is passed to mutators to try to change
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LootGenerationFactory), nameof(LootGenerationFactory.CreateAndMutateWcid), new Type[] { typeof(TreasureDeath), typeof(TreasureRoll), typeof(bool) })]
    public static void PostCreateAndMutateWcid(TreasureDeath treasureDeath, TreasureRoll treasureRoll, bool isMagical, ref WorldObject __result)
    {
        if (treasureDeath is null) return;

        //Keeps track of what mutations have been applied
        HashSet<Mutation> mutations = new();

        foreach (var mutator in enabledPatches)
        {
            //Check for elligible item type
            if (!mutator.Mutates(treasureDeath, treasureRoll, mutations, __result))
                continue;

            //If an item was mutated add the type
            if (mutator.TryMutate(treasureDeath, treasureRoll, mutations, __result))
                mutations.Add(mutator.MutationType);
        }

        if (PatchClass.Settings.Verbose && mutations.Count > 0)
            ModManager.Log($"{__result.Name} was mutated with: {String.Join(", ", mutations)}");

    }


    [CommandHandler("clean", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void Clean(Session session, params string[] parameters)
    {
        // @delete - Deletes the selected object. Players may not be deleted this way.

        var player = session.Player;

        Debugger.Break();
        foreach (var item in player.Inventory)
        {
            player.TryRemoveFromInventoryWithNetworking(item.Key, out var i, Player.RemoveFromInventoryAction.None);
        }
    }
}
