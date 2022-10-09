namespace SimpleMod
{
    [HarmonyPatch]
    public class PatchClass
    {
        public static float CRIT_CHANCE = 100f;
        private static Statistics _stats = new();
        private static string fileName = "Stats.json";

        public static void Start()
        {
            if (File.Exists(fileName))
            {
                try
                {
                    ModManager.Log($"Loading statistics from {fileName}");
                    var jsonString = File.ReadAllText(fileName);
                    _stats = JsonSerializer.Deserialize<Statistics>(jsonString);
                }
                catch (Exception ex)
                {
                    ModManager.Log($"Failed to deserialize statistics from {fileName}");
                    _stats = new Statistics();
                    return;
                }
            }
        }
        public static void Shutdown()
        {
            string jsonString = JsonSerializer.Serialize(_stats);
            File.WriteAllText(fileName, jsonString);
        }


        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponCriticalChance), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
        public static bool Prefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, ref float __result)
        {
            if (target is not Player)
            {
                //ModManager.Log($"Player was found");
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

                        //var player = lastDamagerInfo.TryGetAttacker() as Player;
                        if (count % 5 == 0)
                        {
                            ModManager.Message(name, $"Bonus XP for killing your {count}th {cName}: {__instance.XpOverride}-->{__instance.XpOverride *= 10}");
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
    }
}