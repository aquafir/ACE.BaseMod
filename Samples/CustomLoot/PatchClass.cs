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

        PatchCategories();

        Mod.State = ModState.Running;
    }

    public static void Shutdown()
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
    /// Selectively patch ACE to look for non-Aetheria/Cloak items for procs
    /// </summary>
    private static void PatchCategories()
    {
        if (Settings.EnableOnAttackForNonAetheria)
            Mod.Harmony.PatchCategory(Settings.OnAttackCategory);

        if (Settings.EnableOnHitForNonCloak)
            Mod.Harmony.PatchCategory(Settings.OnHitCategory);
    }

    #region Mutation Handlers
    private static void HandleSetMutation(TreasureDeath treasureDeath, TreasureRoll treasureRoll, WorldObject __result)
    {
        //Missing or empty set doesn't roll
        if (!Settings.CustomSets.TryGetValue(treasureRoll.ItemType, out var setList) || setList.Count == 0)
            return;

        //Add a set
        __result.RollEquipmentSet(treasureRoll);
    }

    private static void HandleCloakMutation(TreasureDeath treasureDeath, TreasureRoll treasureRoll, WorldObject __result)
    {
        //Don't roll missing chance
        if (!Settings.CloakMutationChance.TryGetValue(treasureRoll.ItemType, out var odds))
            return;

        //Failed roll
        if (ThreadSafeRandom.Next(0.0f, 1.0f) >= odds)
            return;
        __result.MutateLikeCloak(treasureDeath, treasureRoll);
    }

    private static void HandleSlayerMutation(TreasureDeath treasureDeath, WorldObject __result)
    {
        if (ThreadSafeRandom.Next(0.0f, 1.0f) < Settings.SlayerChance)
        {
            //Check already slayer            
            if (__result.GetProperty(PropertyInt.SlayerCreatureType) is not null)
                return;

            //Use all creatures or just a subset
            var cTypes = Settings.UseCustomSlayers ? Settings.SlayerSpecies : Enum.GetValues<CreatureType>();
            //Get a random type
            var type = cTypes[ThreadSafeRandom.Next(0, cTypes.Length - 1)];
            var power = Settings.SlayerPower[treasureDeath.Tier];

            __result.SetProperty(PropertyInt.SlayerCreatureType, (int)type);
            __result.SetProperty(PropertyFloat.SlayerDamageBonus, power);
        }
    }
    #endregion

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LootGenerationFactory), nameof(LootGenerationFactory.CreateAndMutateWcid), new Type[] { typeof(TreasureDeath), typeof(TreasureRoll), typeof(bool) })]
    public static void PostCreateAndMutateWcid(TreasureDeath treasureDeath, TreasureRoll treasureRoll, bool isMagical, ref WorldObject __result)
    {
        if (treasureDeath is null) return;

        //Todo Add additional spells to armor or weapons like cloaks. You don't need to do rings but maybe dbuffs or heals/mana/stam or rare gem spells
        switch (treasureRoll.ItemType)
        {
            case TreasureItemType_Orig.Caster:
                break;
            case TreasureItemType_Orig.Weapon:
                HandleSlayerMutation(treasureDeath, __result);
                break;
            case TreasureItemType_Orig.Clothing:
            case TreasureItemType_Orig.Jewelry:
            case TreasureItemType_Orig.Armor:
                HandleCloakMutation(treasureDeath, treasureRoll, __result);
                break;
            case TreasureItemType_Orig.Cloak:
                break;
            case TreasureItemType_Orig.PetDevice:
                break;
            case TreasureItemType_Orig.HealKit:
                break;
            case TreasureItemType_Orig.Lockpick:
                break;
        }

        //Check if the set is mutated
        if (Settings.SetMutationChance.TryGetValue(treasureRoll.ItemType, out var setOdds)
            && ThreadSafeRandom.Next(0.0f, 1.0f) < setOdds)
            HandleSetMutation(treasureDeath, treasureRoll, __result);
    }
}
