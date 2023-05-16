

using Balance.Patches;
using System.Text.Json;

namespace Balance
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
            //ReferenceHandler = ReferenceHandler.Preserve,
            //MaxDepth = 60,
            //IncludeFields = true,
        };

        private static void SaveSettings()
        {
            string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);
            Debugger.Break();
            if (!settingsInfo.RetryWrite(jsonString, RETRIES))
            {
                ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
            }
        }

        private static void LoadSettings()
        {
            if (!File.Exists(settingsInfo.FullName))
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

            var p = new GrantExperience();
            var a = new List<GrantExperience>(){ 
                p 
            };
            var j1 = JsonSerializer.Serialize(p, _serializeOptions);
            var j2 = JsonSerializer.Serialize(a, _serializeOptions);
            var j3 = JsonSerializer.Serialize(Settings, _serializeOptions);
            Debugger.Break();
            LoadSettings();

            foreach (var patch in Settings.Formulas)
            {
                if (patch.Enabled)
                    patch.Start();
            }

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

            //Shutdown/unpatch everything on settings change to support repatching by category
            foreach (var patch in Settings.Formulas)
            {
                if (patch.Enabled)
                    patch.Shutdown();
            }
            Mod.Harmony.UnpatchAll();

            if (Mod.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion

        #region Patches
        #endregion
    }
}