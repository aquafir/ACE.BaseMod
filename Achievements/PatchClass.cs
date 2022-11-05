using System.Text.Json.Serialization;

namespace Achievements;

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
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GetDeathMessage), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
    public static void CountKills(DamageHistoryInfo lastDamagerInfo, DamageType damageType, bool criticalHit, ref Creature __instance)
    {
        if (lastDamagerInfo.IsPlayer)
        {
            //Add player to kill stats if they don't exist
            var name = lastDamagerInfo.Name;
            if (!Settings.Kills.ContainsKey(name))
            {
                ModManager.Log($"Creating kill stats for {lastDamagerInfo.Name}...");
                Settings.Kills.Add(name, new Dictionary<string, uint>());
            }

            if (!Settings.Kills.TryGetValue(name, out var kills))
            {
                ModManager.Log($"Still can't find kills?");
                //Settings.Kills.Add(name, new Dictionary<string, uint>());
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

                    //var player = lastDamagerInfo.TryGetAttacker() as Player;
                    if (count % Settings.Interval == 0)
                    {
                        ModManager.Message(name, $"Bonus XP for killing your {count}th {cName}: {__instance.XpOverride}-->{__instance.XpOverride *= Settings.Multiplier}");
                        //__instance.XpOverride *= 10;
                    }
                    else
                        ModManager.Message(name, $"You've killed {count} {cName}.");
                }
            }
        }
    }
    #endregion

    #region Start/Shutdown
    public static void Start()
    {
        LoadSettings();
    }

    public static void Shutdown()
    {
        //Save out the tracked achievements (Settings shouldn't be used for this-- use a DB instead)
        SaveSettings();
    }
    #endregion
}
