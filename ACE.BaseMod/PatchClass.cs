using System.Threading;

namespace ACE.BaseMod;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    private static bool _loadError = true;
    public static Settings Settings = new();
    private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
    private static JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private static void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);
        File.WriteAllText(settingsPath, jsonString);
    }

    private static async Task LoadSettingsAsync()
    {
        if (File.Exists(settingsPath))
        {
            try
            {
                ModManager.Log($"Loading Settings from {settingsPath}...");

                using var fs = new FileStream(settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var sr = new StreamReader(fs);

                string jsonString = await sr.ReadToEndAsync().WaitAsync(TIMEOUT);
                Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to deserialize from {settingsPath}...");

                _loadError = true;
                return;
            }
        }
        else
        {
            ModManager.Log($"Creating {settingsPath}...");
            SaveSettings();
        }
        _loadError = false;
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
    public static async Task StartAsync()
    {
        await LoadSettingsAsync();

        if (_loadError)
            Mod.Container?.Shutdown();
    }


    public static void Shutdown()
    {
        if (_loadError)
        {
            ModManager.Log($"Improper shutdown.", ModManager.LogLevel.Fatal);
            return;

        }

        //Clean up what you need to...
        //SaveSettings();
    }
    #endregion
}

