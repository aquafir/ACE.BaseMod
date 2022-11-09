namespace Achievements;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
    const int RETRIES = 10;

    public static Settings Settings = new();
    private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
    private static FileInfo settingsInfo = new(settingsPath);

    public static History History = new();
    private static string historyPath = Path.Combine(Mod.ModPath, "History.json");
    private static FileInfo historyInfo = new(historyPath);

    private static JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }

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
        if (settingsInfo.Exists)
        {
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
        else
        {
            ModManager.Log($"Creating {settingsInfo}...");
            SaveSettings();
        }
    }
    #endregion

    private static void SaveHistory()
    {
        string jsonString = JsonSerializer.Serialize(History, _serializeOptions);

        if (!historyInfo.RetryWrite(jsonString, RETRIES))
        {
            ModManager.Log($"Failed to save history to {historyPath}...", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
        }
    }

    private static void LoadHistory()
    {
        if (historyInfo.Exists)
        {
            ModManager.Log($"Loading history from {historyPath}...");

            if (!historyInfo.RetryRead(out string jsonString, RETRIES))
            {
                Mod.State = ModState.Error;
                return;
            }

            try
            {
                History = JsonSerializer.Deserialize<History>(jsonString, _serializeOptions);
            }
            catch (Exception)
            {
                ModManager.Log($"Failed to deserialize: {historyPath}", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
                return;
            }
        }
        else
        {
            ModManager.Log($"Creating {historyInfo}...");
            SaveHistory();
        }
    }

    #region Start/Shutdown
    public static void Start()
    {
        //Need to decide on async use
        Mod.State = ModState.Loading;
        LoadSettings();

        if (Mod.State != ModState.Error)
            LoadHistory();

        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        Mod.State = ModState.Running;
    }

    public static void Shutdown()
    {
        //Save out the tracked achievements (should use a DB instead)
        if (Mod.State == ModState.Running)
            SaveHistory();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GetDeathMessage), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
    public static void CountKills(DamageHistoryInfo lastDamagerInfo, DamageType damageType, bool criticalHit, ref Creature __instance)
    {
        if (lastDamagerInfo.IsPlayer)
        {
            //Add player to kill stats if they don't exist
            var name = lastDamagerInfo.Name;
            if (!History.Kills.ContainsKey(name))
            {
                ModManager.Log($"Creating kill stats for {lastDamagerInfo.Name}...");
                History.Kills.Add(name, new Dictionary<string, uint>());
            }

            if (!History.Kills.TryGetValue(name, out var kills))
            {
                ModManager.Log($"Still can't find kills?");
                //History.Kills.Add(name, new Dictionary<string, uint>());
            }
            else
            {
                var cName = __instance.Name;

                if (!kills.TryGetValue(cName, out var count))
                {
                    ModManager.Log($"Tracking {name} kills of {cName}");
                    kills.Add(cName, 1);
                }
                else
                {
                    kills[cName] = ++count;
                    ModManager.Log($"{name} has killed {count} {cName}");

                    if (count % Settings.Interval == 0)
                    {
                        ModManager.Message(name, $"Bonus XP for killing your {count}th {cName}: {__instance.XpOverride}-->{__instance.XpOverride *= Settings.Multiplier}");
                    }
                    else
                        ModManager.Message(name, $"You've killed {count} {cName}.");
                }
            }
        }
    }
    #endregion
}
