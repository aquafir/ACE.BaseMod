namespace CritOverride
{
    [HarmonyPatch]
    public class PatchClass
    {
        public static float critChance = 100f;
        private static Settings _settings = new();
        private static string filePath = Path.Combine(Mod.ModPath, "Settings.json");

        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponCriticalChance), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
        public static bool Prefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, ref float __result)
        {
            //if (target is not Player)
            //{
            //    //ModManager.Log($"Player was found");
            //    __result = critChance;
            //    __result = 100f;
            //    return false;
            //}

            ////Don't skip if not handled
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponMagicCritFrequency), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
        public static bool MagicCritPrefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, double __state, ref float __result)
        {
            ModManager.Log("Override mCrit");
            __result = _settings.MagicCritChance;
            return false;
        }

        public static void Start()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    ModManager.Log($"Loading settings from {filePath}");
                    var jsonString = File.ReadAllText(filePath);
                    _settings = JsonSerializer.Deserialize<Settings>(jsonString);
                    critChance = _settings.CritChance;
                }
                catch (Exception ex)
                {
                    ModManager.Log($"Failed to deserialize settings from {filePath}");
                    _settings = new Settings();
                    return;
                }
            }
            else
            {
                ModManager.Log($"Creating settings at {filePath}");
                string jsonString = JsonSerializer.Serialize(_settings);
                File.WriteAllText(filePath, jsonString);
            }
        }

        public static void Shutdown()
        {
            //Reloading settings would save on shutdown, making it harder to edit Settings.json-->reload
            //string jsonString = JsonSerializer.Serialize(_settings);
            //File.WriteAllText(filePath, jsonString);
        }
    }
}