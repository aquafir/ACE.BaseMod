using ACE.Common;
using ACE.Entity;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.WorldObjects;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Respawn
{
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

            //if (!Settings.Initialized)
            //    SpawnHelper.GenerateLandblockCreatureCounts();

            if (Mod.State == ModState.Error)
            {
                ModManager.DisableModByPath(Mod.ModPath);
                return;
            }

            Mod.State = ModState.Running;
        }

        public static void Shutdown()
        {
            //if (Mod.State == ModState.Running)
            //    SaveSettings();

            if (Mod.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion

        #region Commands
        [CommandHandler("left", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
        public static void HandleLeft(Session session, params string[] parameters)
        {
            var lb = session.Player.CurrentLandblock;

            if (lb.IsDungeon)
            {
                var sum = lb.GetMaxSpawns();

                session.Player.SendMessage($"There are {lb.GetCreatures().Count}/{sum} creatures remaining ({lb.PercentAlive():P2})");
            }
            else
            {
                // session.Network.EnqueueSend(new GameMessageSystemChat($"Only enabled in dungeon landblocks.", ChatMessageType.Broadcast));
            }
        }

        [CommandHandler("gencounts", AccessLevel.Admin, CommandHandlerFlag.None)]
        public static void HandleDumpGens(Session session, params string[] parameters)
        {
            SpawnHelper.GenerateLandblockCreatureCounts(true);
        }

        [CommandHandler("respawn", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, "/respawn", "/respawn")]
        public static void HandleRespawn(Session session, params string[] parameters)
        {
            var lb = session.Player.CurrentLandblock;

            if (lb.IsDungeon)
                lb.RespawnCreatures(true);
        }
        #endregion

        #region Patches
        //Todo: decide how to keep LB records
        //private static Player[,] lastKills = new Player[255, 255];
        private static Dictionary<Landblock,Player> lastKills = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Creature), nameof(Creature.GetDeathMessage), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
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
                if (lastKills.ContainsKey(lb))
                    lastKills[lb] = player;
                else
                    lastKills.Add(lb, player);

                player.SendMessage($"{lb.GetCreatures().Count} / {lb.GetMaxSpawns()} killed ({lb.PercentAlive():P2} remaining");
            }
        }

        static double last = 0;     //Last portal time
        static double interval = 5; //Trigger every 5 seconds
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
                        //var player = lastKills[lb.Id.LandcellX, lb.Id.LandcellY];
                        if (!lastKills.TryGetValue(lb, out var player))
                            continue;

                        if (player is not null)
                        {
                            player.SendMessage($"You triggered a respawn! 10M rewarded.");
                            player.GrantXP(10000000, XpType.Quest, ShareType.None);

                            //lastKills[lb.Id.LandcellX, lb.Id.LandcellY] = null;
                            lastKills[lb] = null;
                        }

                        ModManager.Log($"Respawning {lb.Id} ({percentAlive:P2} alive)");
                        lb.RespawnCreatures();
                    }
                }
            }
        }
        #endregion

    }
}