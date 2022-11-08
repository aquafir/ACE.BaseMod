using System.Text.Json.Serialization;

namespace Achievements;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    //More complex state tracking may be needed
    private static bool _loadError = true;
    private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(5);

    public static Settings Settings = new();
    public static History History = new();
    private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
    private static string historyPath = Path.Combine(Mod.ModPath, "History.json");

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
            //ModManager.Log($"Loading Settings from {settingsPath}...");
            var loadDelay = Stopwatch.StartNew();

            using var fs = new FileStream(settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sr = new StreamReader(fs);
            while (true)
            {
                try
                {
                    if (loadDelay.Elapsed <= TIMEOUT)
                    {
                        string jsonString = await sr.ReadToEndAsync();
                        Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
                       
                        break;
                    }
                    else
                    {
                        ModManager.Log($"Failed for {TIMEOUT.TotalSeconds} seconds to load {settingsPath}...");
                        Debugger.Break();

                        fs?.Close();
                        sr?.Close();

                        _loadError = true;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }
             
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

    private static void SaveHistory()
    {
        string jsonString = JsonSerializer.Serialize(History, _serializeOptions);
        File.WriteAllText(historyPath, jsonString);
    }

    private static async Task LoadHistoryAsync()
    {
        if (File.Exists(historyPath))
        {
            try
            {
                ModManager.Log($"Loading History from {historyPath}...");
                using var fs = new FileStream(historyPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var sr = new StreamReader(fs);

                string jsonString = await sr.ReadToEndAsync().WaitAsync(TIMEOUT);
                History = JsonSerializer.Deserialize<History>(jsonString, _serializeOptions);

            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to deserialize from {historyPath}...");

                _loadError = true;
                return;
            }
        }
        else
        {
            ModManager.Log($"Creating {historyPath}...");
            SaveHistory();
        }
        _loadError = false;
    }

    #region Start/Shutdown
    public static async Task StartAsync()
    {
        await LoadSettingsAsync();

        if (!_loadError)
            await LoadHistoryAsync();

        if (_loadError)
        {
            ModManager.DisableModByPath(Mod.ModPath);
        }
    }

    public static void Shutdown()
    {
        //Save out the tracked achievements (should use a DB instead)
        if (_loadError)
        {
            ModManager.Log($"Improper shutdown.", ModManager.LogLevel.Fatal);
            return;
        }
        SaveHistory();
    }
    #endregion
}
