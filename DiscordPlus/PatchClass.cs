using ACE.Server.Network.GameMessages.Messages;

namespace DiscordPlus
{
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
        [HarmonyPatch(typeof(GameMessageTurbineChat), MethodType.Constructor, 
            new Type[] { typeof(ChatNetworkBlobType), typeof(ChatNetworkBlobDispatchType), typeof(uint), typeof(string), typeof(string), typeof(uint), typeof(ChatType) })]
        public static void Prefix(ChatNetworkBlobType chatNetworkBlobType, ChatNetworkBlobDispatchType chatNetworkBlobDispatchType, uint channel, string senderName, string message, uint senderID, ChatType chatType)
        {
            //ModManager.Log($"Routing message from {senderName}:\n\t{message}");
            DiscordRelay.RelayIngameChat(message, senderName, chatType, channel, senderID, chatNetworkBlobType, chatNetworkBlobDispatchType);
        }
        #endregion

        #region Start/Shutdown
        public static void Start()
        {
            LoadSettings();
            DiscordRelay.Initialize();
        }

        public static void Shutdown()
        {
            DiscordRelay.Shutdown();
        }
        #endregion
    }
}