using System.Text.Encodings.Web;

namespace Raise
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

            if (Mod.State == ModState.Error)
            {
                ModManager.DisableModByPath(Mod.ModPath);
                return;
            }

            SetMaxLevel();

            Mod.State = ModState.Running;
        }

        public static void Shutdown()
        {
            if (Mod.State == ModState.Running)
            {
                RestoreMaxLevel();
            }

            if (Mod.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion

        //Probably shouldn't be doing this except at the start/end...
        readonly static List<ulong> storedCosts = DatManager.PortalDat.XpTable.CharacterLevelXPList.ToList();
        readonly static List<uint> storedCredits = DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.ToList();
        private static void SetMaxLevel()
        {
            RestoreMaxLevel();

            //Add levels up to max
            for (int i = DatManager.PortalDat.XpTable.CharacterLevelXPList.Count; i <= PatchClass.Settings.MaxLevel; i++)
            {
                var cost = DatManager.PortalDat.XpTable.CharacterLevelXPList.Last() + PatchClass.Settings.CostPerLevel;
                var credits = (uint)(i % PatchClass.Settings.CreditInterval == 0 ? 1 : 0);
                DatManager.PortalDat.XpTable.CharacterLevelXPList.Add(cost);
                DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Add(credits);
                //session?.Player?.SendMessage($"Adding level {i} for {cost}.  {credits} skill credits.");
            }

            ModManager.Log($"Set max level to {PatchClass.Settings.MaxLevel}");
        }

        private static void RestoreMaxLevel()
        {
            //Restored the original values...
            DatManager.PortalDat.XpTable.CharacterLevelXPList.Clear();
            DatManager.PortalDat.XpTable.CharacterLevelXPList.AddRange(storedCosts);
            DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Clear();
            DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.AddRange(storedCredits);

            ModManager.Log($"Restored max level to {DatManager.PortalDat.XpTable.CharacterLevelXPList.Count}");
        }
    }
}
