namespace ACE.BaseMod;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    public static Settings Settings = new();
    private static string filePath = Path.Combine(Mod.ModPath, "Settings.json");
    private static JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private static void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);
        File.WriteAllText(filePath, jsonString);
    }

    private static void LoadSettings()
    {
        if (File.Exists(filePath))
        {
            try
            {
                ModManager.Log($"Loading Settings from {filePath}...");
                var jsonString = File.ReadAllText(filePath);
                Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to deserialize from {filePath}, creating new Settings.json and restarting...");
                Settings = new Settings();
                SaveSettings();

                Mod.Container?.Restart();
                return;
            }
        }
        else
        {
            ModManager.Log($"Creating {filePath}...");
            SaveSettings();
        }
    }
    #endregion

    #region Patches
    //Use Harmony attributes to override Player-on-NP crit chance using Settings
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponCriticalChance), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
    public static bool Prefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, ref float __result)
    {
        if (target is not Player)
        {
            __result = 100;
            return false;
        }

        //Don't skip if not handled
        return true;
    }
    #endregion

    #region Start/Shutdown
    public static void Start()
    {
        LoadSettings();
    }

    public static void Shutdown()
    {
        //Clean up what you need to...
        //SaveSettings();
    }
    #endregion
}

