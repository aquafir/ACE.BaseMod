using System.Text.Json.Serialization;

namespace CriticalOverride
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
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        private static void SaveSettings()
        {
            string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);
            File.WriteAllText(filePath, jsonString);
        }
        #endregion

        #region Patches
        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponCriticalChance), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
        public static bool Prefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, ref float __result)
        {
            //Proceed normally with player targets
            if (target is Player)
                return true;

            __result = Settings.CritChance;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponMagicCritFrequency), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
        public static bool MagicCritPrefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, double __state, ref float __result)
        {
            //Proceed normally with player targets
            if (target is Player)
                return true;

            __result = Settings.MagicCritChance;
            return false;
        }
        #endregion

        #region Start/Shutdown
        public static void Start()
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
                    ModManager.Log($"Failed to deserialize from {filePath}, creating new Settings.json...");
                    Settings = new Settings();
                    return;
                }
            }
            else
            {
                ModManager.Log($"Creating {filePath}...");
                string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);
                File.WriteAllText(filePath, jsonString);
            }
        }

        public static void Shutdown()
        {
            //Clean up what you need to...
            //SaveSettings();
        }
        #endregion
    }
}