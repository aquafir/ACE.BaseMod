namespace ACE.BaseMod;

[HarmonyPatch]
public class PatchClass
{
    public static float CRIT_CHANCE = 100f;
    private static Statistics _stats = new();
    private static string filePath = Path.Combine(Mod.ModPath, "Stats.json");

    public static void Start()
    {
        if (File.Exists(filePath))
        {
            try
            {
                ModManager.Log($"Loading from {filePath}");
                var jsonString = File.ReadAllText(filePath);
                _stats = JsonSerializer.Deserialize<Statistics>(jsonString);
            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to deserialize from {filePath}");
                _stats = new Statistics();
                return;
            }
        }
        else
        {
            ModManager.Log($"Creating {filePath}");
            string jsonString = JsonSerializer.Serialize(_stats);
            File.WriteAllText(filePath, jsonString);
        }
    }
    public static void Shutdown()
    {
        string jsonString = JsonSerializer.Serialize(_stats);
        File.WriteAllText(filePath, jsonString);
    }


    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponCriticalChance), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
    public static bool Prefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, ref float __result)
    {
        if (target is not Player)
        {
            __result = CRIT_CHANCE;
            return false;
        }

        //Don't skip if not handled
        return true;
    }

    public static void CountKills(DamageHistoryInfo lastDamagerInfo, DamageType damageType, bool criticalHit, ref Creature __instance)
    {
        if (lastDamagerInfo.IsPlayer)
        {
            //Add player to kill stats if they don't exist
            var name = lastDamagerInfo.Name;
            if (!_stats.Kills.ContainsKey(name))
            {
                ModManager.Log($"Creating kill stats for {lastDamagerInfo.Name}...");
                _stats.Kills.Add(name, new Dictionary<string, uint>());
            }

            if (!_stats.Kills.TryGetValue(name, out var kills))
            {
                ModManager.Log($"Still can't find kills?");
                //_stats.Kills.Add(name, new Dictionary<string, uint>());
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

                    if (count % 5 == 0)
                    {
                        ModManager.Message(name, $"Bonus XP for killing your {count}th {cName}: {__instance.XpOverride}-->{__instance.XpOverride *= 10}");
                    }
                    else
                        ModManager.Message(name, $"You've killed {count} {cName}.");
                }
            }
        }
    }
}

public class Statistics
{
    public Dictionary<string, Dictionary<string, uint>> Kills { get; set; } = new();
}