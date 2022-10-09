namespace KillCount
{
    [HarmonyPatch]
    public class PatchClass
    {
        private static Statistics _stats = new();
        private static string statsPatch = Path.Combine(Mod.ModPath, "Stats.json");

        public static void Start()
        {
            if (File.Exists(statsPatch))
            {
                try
                {
                    ModManager.Log($"Loading statistics from {statsPatch}");
                    var jsonString = File.ReadAllText(statsPatch);
                    _stats = JsonSerializer.Deserialize<Statistics>(jsonString);
                }
                catch (Exception ex)
                {
                    ModManager.Log($"Failed to deserialize statistics from {statsPatch}");
                    _stats = new Statistics();
                    return;
                }
            }
        }
        public static void Shutdown()
        {
            string jsonString = JsonSerializer.Serialize(_stats);
            File.WriteAllText(statsPatch, jsonString);
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

                        //var player = lastDamagerInfo.TryGetAttacker() as Player;
                        if (count % _stats.Interval == 0)
                        {
                            ModManager.Message(name, $"Bonus XP for killing your {count}th {cName}: {__instance.XpOverride}-->{__instance.XpOverride *= _stats.Multiplier}");
                            //__instance.XpOverride *= 10;
                        }
                        else
                            ModManager.Message(name, $"You've killed {count} {cName}.");
                    }
                }

                //ModManager.Log($"{lastDamagerInfo.Name} has killed {__instance.Name}");
                //ModManager.Log($"{name} has killed {_stats.Kills[name]}");
            }
        }
    }

    public class Statistics
    {
        public Dictionary<string, Dictionary<string, uint>> Kills { get; set; } = new();
        public int Interval { get; set; } = 10;
        public int Multiplier { get; set; } = 7;
    }
}