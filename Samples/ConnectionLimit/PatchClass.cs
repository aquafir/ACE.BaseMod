using ACE.Server.Managers;
using System.Net;

namespace Tinkering
{
    [HarmonyPatch]
    public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
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
            // Shut down enabled mod...

            if (Mod.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion

        #region Patches
        //Landblock tick approach
        static double lastCheck = 0;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LandblockManager), nameof(LandblockManager.Tick), new Type[] { typeof(double) })]
        public static void PreTick(double portalYearTicks)
        {
            //Gate how often this is checked
            var lapsed = portalYearTicks - lastCheck;
            if (lapsed < Settings.Interval) return;

            lastCheck = portalYearTicks;

            //Todo: probably could be more efficient using sessionMap in NetworkManager
            Dictionary<IPAddress, int> connections = new();
            foreach (var player in PlayerManager.GetAllOnline())
            {
                //Ignore exempt landblocks
                if (Settings.ExemptLandblocks.Contains(player.CurrentLandblock.Id.Landblock))
                    continue;


                //Count other landblocks
                var address = player.Session.EndPointC2S.Address;
                if (!connections.ContainsKey(address))
                    connections.Add(address, 1);
                else connections[address]++;


                //Maximum connections are from ExemptIPAddress in Config.js or if not present the default value MaxNonExempt
                int maxConnections = Settings.ExemptIPAddresses.ContainsKey(address.ToString()) ? Settings.ExemptIPAddresses[address.ToString()] : Settings.MaxNonExempt;

                if (connections[address] > maxConnections)
                {
                    player.SendMessage($"Booting due to exceeding {Settings.MaxNonExempt} allowed outside of exempt areas.");
                    player.Session.LogOffPlayer();
                }
            }
        }

        //Portal approach
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Player), nameof(Player.OnTeleportComplete), new Type[] { })]
        //public static void PreOnTeleportComplete(ref Player __instance)
        //{
        //    int nonexemptCount = 0;
        //    var endpoint = __instance.Session.EndPoint;
        //    var players = PlayerManager.GetAllOnline();
        //    foreach (var p in players.Where(x => x.Session.EndPoint.Address.Address == endpoint.Address.Address))
        //    {
        //        if (Settings.ExemptLandblocks.Contains(p.CurrentLandblock.Id.Landblock))
        //            continue;

        //        if (++nonexemptCount > Settings.AllowedConnections)
        //        {
        //            p.SendMessage($"Booting due to exceeding {Settings.AllowedConnections} allowed outside of exempt areas.");
        //            p.Session.LogOffPlayer();
        //        }
        //    }
        //}
        #endregion
    }
}
