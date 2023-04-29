using ACE.DatLoader.FileTypes;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects.Managers;

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

        #region Commands

        //One approach would be replacing the XpTable
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(PortalDatDatabase), nameof(PortalDatDatabase.XpTable), MethodType.Getter)]
        //public static bool Get_XpTable(ref XpTable __result)
        //{
        //    __result = someXpTable;

        //    //Return false to override
        //    return false;
        //}


        [CommandHandler("setmax", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 1, "Set max level.")]
        public static void HandleMax(Session session, params string[] parameters)
        {
            //Try to parse first param for max level
            if (parameters.Length < 1 || !uint.TryParse(parameters[0], out var maxLevel)) 
                return;
            //1 billion xp per level post 275
            var costPerLevel = 1_000_000_000u;
            //Add a skill credit every 10 levels
            var creditInterval = 10;

            for (int i = DatManager.PortalDat.XpTable.CharacterLevelXPList.Count; i <= maxLevel; i++) {
                var cost = DatManager.PortalDat.XpTable.CharacterLevelXPList.Last() + costPerLevel;
                var credits = (uint)(i % creditInterval == 0 ? 1 : 0); // + DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Last();
                DatManager.PortalDat.XpTable.CharacterLevelXPList.Add(cost);
                DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Add(credits);
                session?.Player?.SendMessage($"Adding level {i} for {cost}.  {credits} skill credits.");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(XpTable), nameof(XpTable.CharacterLevelSkillCreditList), MethodType.Getter)]
        public static void PostGet_CharacterLevelSkillCreditList(ref XpTable __instance, ref List<uint> __result)
        {
            //Your code here
        }


        //[CommandHandler("vassalxp", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Shows full experience from vassals beyond what the UI shows.")]
        //public static void HandleShowVassalXp(Session session, params string[] parameters)
        //{
        //    var player = session.Player;

        //    var sb = new StringBuilder("Experience from vassals:");
        //    foreach (var vassalNode in AllegianceManager.GetAllegianceNode(player).Vassals.Values)
        //    {
        //        var vassal = vassalNode.Player;
        //        sb.Append($"{vassal.Name,-30}{vassal.AllegianceXPGenerated,-20:N0}");
        //    }

        //    player.SendMessage(sb.ToString());
        //}
        #endregion

        #region Patches


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), nameof(Player.GetXPBetweenLevels),  new Type[] { typeof(int), typeof(int) })]
        public static bool PreGetXPBetweenLevels(int levelA, int levelB, ref Player __instance, ref ulong __result)
        {
            var maxLevel = (int)Player.GetMaxLevel();

            levelA = Math.Clamp(levelA, 1, maxLevel - 1);
            levelB = Math.Clamp(levelB, 1, maxLevel);

            var levelA_totalXP = DatManager.PortalDat.XpTable.CharacterLevelXPList[levelA];
            var levelB_totalXP = DatManager.PortalDat.XpTable.CharacterLevelXPList[levelB];

            __result = levelB_totalXP - levelA_totalXP;

            //Override
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), nameof(Player.GetXPToNextLevel), new Type[] { typeof(int) })]
        public static bool PreGetXPToNextLevel(int level, ref Player __instance, ref ulong __result)
        {
            //Return false to override
            //return false;

            //Return true to execute original
            return true;
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnchantmentManager), nameof(EnchantmentManager.GetNetherDotDamageRating), new Type[] { })]
        public static void VoidCap(EnchantmentManager __instance, ref int __result)
        {
            //Repeated work, but it will be cached and probably not a huge performance issue
            var type = EnchantmentTypeFlags.Int | EnchantmentTypeFlags.SingleStat | EnchantmentTypeFlags.Additive;
            var debuffs = __instance.GetEnchantments_TopLayer(type, (uint)PropertyInt.NetherOverTime).Count;
            var cap = Math.Min(Settings.NetherRatingCap, Settings.NetherPerDebuffCap * debuffs);

            ModManager.Log($"{__result} capped to {Settings.NetherRatingCap} or {debuffs} * {Settings.NetherPerDebuffCap}");

            __result = Math.Min(cap, __result);
        }
        #endregion
    }
}