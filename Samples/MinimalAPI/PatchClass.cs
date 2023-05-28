using ACE.Server.Managers;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ACE.Server.WorldObjects;

namespace MinimalAPI
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
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
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

            StartServer();

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

            //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
            //SaveSettings();

            StopServer();

            if (Mod.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion


        private static WebApplication app;
        private static void StartServer()
        {
            //var builder = WebApplication.CreateBuilder(new string[] {}) {
            //    n
            //}

            //builder.Configuration.AddJsonFile("appsettings.json");
            //builder.Configuration.AddIniFile("appsettings.ini");

            //var app = builder.Build();
            app = WebApplication.Create(new string[] { });

            var handler = () => "This is a lambda variable!";
            app.MapGet("/foo", handler);

            app.MapGet("/", () => string.Join(Environment.NewLine, PlayerManager.GetAllPlayers().Select(x => x.Name).ToList()));

            app.MapGet("/player/", () =>
                string.Join(Environment.NewLine, PlayerManager.GetAllPlayers().Select(x => $"{x.Name} is {(x is OfflinePlayer ? "offline" : "online")}").ToList()));
            app.MapGet("/player/{name}", (string name) =>
            {
                var player = PlayerManager.GetAllPlayers().Where(x => x.Name == name).FirstOrDefault();
                if (player is null)
                    return $"{name} does not exist";

                if (player is OfflinePlayer)
                    return $"{name} is offline";

                if (player is not Player p)
                    return "Error";

                var sb = new StringBuilder(p.Name + "\r\n");
                foreach (var item in p.Inventory)
                    sb.AppendLine($"{item.Key} -- {item.Value.Name}");

                return sb.ToString();
            });
            app.MapGet("/player/{name}/{query}", (string name, string query) =>
            {
                var player = PlayerManager.GetAllPlayers().Where(x => x.Name == name).FirstOrDefault();
                if (player is null)
                    return $"{name} does not exist";

                if (player is not Player p || string.IsNullOrEmpty(query))
                    return "Error";

                var sb = new StringBuilder(p.Name + "\r\n");
                
                foreach (var item in p.Inventory.Where(x => x.Value.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)))
                    sb.AppendLine($"{item.Key} -- {item.Value.Name}");
                
                return sb.ToString();
            });

            app.Run();
        }

        private static void StopServer()
        {
            app.StopAsync().GetAwaiter().GetResult();

            WaitForPortAvailability(5000, TimeSpan.FromSeconds(30));

            Console.WriteLine("Port {0} is now available.", 5000);
        }


        public static void WaitForPortAvailability(int port, TimeSpan timeout)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.Add(timeout);

            while (DateTime.Now < endTime)
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        client.Connect(IPAddress.Loopback, port);
                        return; // Port is now available, exit the method
                    }
                }
                catch (SocketException)
                {
                    // Port is still not available, continue waiting
                    Thread.Sleep(1000); // Wait for 1 second before retrying
                }
            }

            // Timeout reached, port is not available within the specified time
            throw new TimeoutException("Timeout waiting for port availability.");
        }

        #region Patches
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Creature), nameof(Creature.GetDeathMessage), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
        //public static void PreDeathMessage(DamageHistoryInfo lastDamagerInfo, DamageType damageType, bool criticalHit, ref Creature __instance)
        //{
        //  ...
        //}
        #endregion
    }
}