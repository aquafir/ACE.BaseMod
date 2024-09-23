namespace Respawn
{
    [HarmonyPatch]
    public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
    {
        #region Settings
        //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
        const int RETRIES = 10;

        public static Settings Settings = new();
        private static string settingsPath = Path.Combine(ModC.ModPath, "Settings.json");
        private static FileInfo settingsInfo = new(settingsPath);

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
                ModC.State = ModState.Error;
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
                ModC.State = ModState.Error;
                return;
            }

            try
            {
                Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
            }
            catch (Exception)
            {
                ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
                ModC.State = ModState.Error;
                return;
            }
        }
        #endregion

        #region Start/Shutdown
        public static void Start()
        {
            //Need to decide on async use
            ModC.State = ModState.Loading;
            LoadSettings();

            if (Settings.RewardLastKill)
            {
                //Manually patch GetDeathMessage only if a reward is given in Start
                var original = typeof(Creature).GetMethod(nameof(Creature.GetDeathMessage));
                var prefix = typeof(PatchClass).GetMethod(nameof(CountKills));
                ModC.Harmony.Patch(original, new HarmonyMethod(prefix));
                ModManager.Log("Rewarding respawn kills...");
            }

            if (ModC.State == ModState.Error)
            {
                ModManager.DisableModByPath(ModC.ModPath);
                return;
            }

            ModC.State = ModState.Running;
        }

        public static void Shutdown()
        {
            //if (Mod.State == ModState.Running)
            //    SaveSettings();

            if (ModC.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {ModC.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Displays remaining creatures in a landblock
        /// </summary>
        [CommandHandler("left", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
        public static void HandleLeft(Session session, params string[] parameters)
        {
            var lb = session.Player.CurrentLandblock;

            var creatures = lb.GetCreatures();

            var sb = new StringBuilder($"\nThere are {creatures.Count}/{lb.GetMaxSpawns()} creatures remaining ({lb.PercentAlive():P2})\n");
            if (Settings.DetailedDump)
            {
                //Get creature groups by weenie
                var creatureGroups = creatures.GroupBy(x => x.WeenieClassId);
                //Map WCID to name
                var wcidToName = creatures.DistinctBy(c => c.WeenieClassId).ToDictionary(c => c.WeenieClassId, c => c.Name);
                //Get generator profiles
                var profileWcidCount = lb.GetCreatureProfiles().GroupBy(cp => cp.Biota.WeenieClassId).ToDictionary(c => c.Key, c => c.Count());
                //var profiles = lb.GetCreatureProfiles().GroupBy(cp => cp.WeenieClassId).Sum(s => s.Count());

                foreach (var group in creatureGroups)
                {
                    if (wcidToName.TryGetValue(group.Key, out var name) && profileWcidCount.TryGetValue(group.Key, out var max))
                    {
                        sb.Append($"{name,-40} {group.Count()} of {max}\n");
                    }
                    //Skip if not found?
                }
            }

            ModManager.Log(sb.ToString());
            session.Player.SendMessage(sb.ToString());
        }

        /// <summary>
        /// Spawns all or a number of creatures from a landblocks generator profiles
        /// </summary>
        [CommandHandler("spawn", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, -1)]
        public static void HandleRespawn(Session session, params string[] parameters)
        {
            var lb = session.Player.CurrentLandblock;

            int spawnCount;
            if (parameters.Length < 1 || !int.TryParse(parameters[0], out spawnCount))
            {
                lb.RespawnCreatures();
                return;
            }

            //Todo: go over max / spawn weighted by WCID count?
            foreach (var profile in lb.GetCreatureProfiles())
            {
                //Add immediate spawns for each missing and trigger
                var missing = profile.MaxCreate - profile.CurrentCreate;
                var amt = Math.Min(spawnCount, missing);
                for (var i = 0; i < amt; i++)
                {
                    profile.SpawnQueue.Add(DateTime.MinValue);
                }
                profile.ProcessQueue();
                profile.SpawnQueue.Clear();

                spawnCount -= amt;
                if (spawnCount <= 0)
                    return;
            }
        }

        /// <summary>
        /// Kills all of a set number of creatures from a landblock and displays their name and number
        /// </summary>
        [CommandHandler("smote", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, -1)]
        public static void HandleSmote(Session session, params string[] parameters)
        {
            var player = session.Player;
            var lb = player.CurrentLandblock;

            var creatures = lb.GetCreatures().GroupBy(c => c.Name);

            var sb = new StringBuilder("\nSmote:\n");

            //Smite a number / all
            int smiteCount;
            if (parameters.Length < 1 || !int.TryParse(parameters[0], out smiteCount))
                smiteCount = int.MaxValue;

            foreach (var group in creatures)
            {
                var amt = Math.Min(smiteCount, group.Count());
                smiteCount -= amt;

                sb.Append($"{group.Key,-40}{amt} of {group.Count()}\n");

                foreach (var creature in group.Take(amt))
                    creature.Smite(player);
            }

            ModManager.Log(sb.ToString());

            if (Settings.SpamPlayer)
                player.SendMessage(sb.ToString());
        }

        //[CommandHandler("gens", AccessLevel.Admin, CommandHandlerFlag.None)]
        //public static void HandleDumpGens(Session session, params string[] parameters)
        //{
        //    SpawnHelper.GenerateLandblockCreatureCounts(true);
        //}
        #endregion

        #region Patches
        //Todo: decide how to keep LB records
        //private static Player[,] lastKills = new Player[255, 255];
        private static Dictionary<Landblock, Player> _lastKills = new();
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Creature), nameof(Creature.GetDeathMessage), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
        public static void CountKills(DamageHistoryInfo lastDamagerInfo, DamageType damageType, bool criticalHit, ref Creature __instance)
        {
            if (lastDamagerInfo is null)
                return;

            if (lastDamagerInfo.IsPlayer)
            {
                var player = PlayerManager.GetOnlinePlayer(lastDamagerInfo.Guid);

                if (player is null)
                    return;

                var lb = player.CurrentLandblock;

                //lastKills[lb.Id.LandblockX, lb.Id.LandblockY] = player;
                if (_lastKills.ContainsKey(lb))
                    _lastKills[lb] = player;
                else
                    _lastKills.Add(lb, player);

                if (Settings.SpamPlayer)
                    player.SendMessage($"{lb.GetCreatures().Count} / {lb.GetMaxSpawns()} killed ({lb.PercentAlive():P2} remaining");
            }
        }

        //Last portal time
        static double last = 0;

        //Used to loop through landblocks
        static List<LandblockGroup> landblockGroups = Traverse.Create(typeof(LandblockManager)).Field<List<LandblockGroup>>("landblockGroups").Value;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LandblockManager), nameof(LandblockManager.Tick), new Type[] { typeof(double) })]
        public static void LandblockTick(double portalYearTicks)
        {
            //Only run periodically
            if (portalYearTicks - last < Settings.Interval)
                return;

            last = portalYearTicks;
            //ModManager.Log($"Respawn tick @ {last}");

            foreach (var landblockGroup in landblockGroups)
            {
                foreach (var lb in landblockGroup.Where(lb => lb.HasDungeon))
                {
                    var percentAlive = lb.PercentAlive();

                    if (percentAlive < Settings.RespawnTreshold)
                    {
                        ModManager.Log($"Respawning {lb.Id} ({percentAlive:P2} alive)");
                        lb.RespawnCreatures();

                        //Optional reward
                        if (!Settings.RewardLastKill || !_lastKills.TryGetValue(lb, out var player))
                            continue;

                        if (player is not null)
                        {
                            player.SendMessage($"You got the respawn kill!");
                            player.GrantXP(Settings.RewardAmount, XpType.Quest, ShareType.None);

                            //lastKills[lb.Id.LandcellX, lb.Id.LandcellY] = null;
                            _lastKills[lb] = null;
                        }
                    }
                }
            }
            #endregion
        }
    }
}