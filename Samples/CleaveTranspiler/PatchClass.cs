using System.Reflection.Emit;

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
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.IsCleaving), MethodType.Getter)]
        public static bool IsCleaving(WorldObject __instance, ref bool __result)
        {
            if (Settings.AllMeleeCleaves)
            {
                __result = __instance is MeleeWeapon;
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CleaveTargets), MethodType.Getter)]
        public static bool CleaveNumber(WorldObject __instance, ref int __result)
        {
            __result = Settings.CleaveTargets;
            return false;
        }

        static FieldInfo f_cleaveAngle = AccessTools.Field(typeof(Creature), nameof(Creature.CleaveAngle));
        static FieldInfo f_cleaveCylDistance = AccessTools.Field(typeof(Creature), nameof(Creature.CleaveCylRange));

        //Transpile in references to other values 
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Creature), nameof(Creature.GetCleaveTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].LoadsField(f_cleaveAngle))
                {
                    //ModManager.Log($"Replace cleave angle with modded value: {Settings.CleaveAngle}");
                    codes[i] = new CodeInstruction(OpCodes.Ldc_R4, Settings.CleaveAngle);
                }
                if (codes[i].LoadsField(f_cleaveCylDistance))
                {
                    //ModManager.Log($"Replace cleave angle with modded value: {Settings.CleaveCylRange}");
                    codes[i] = new CodeInstruction(OpCodes.Ldc_R4, Settings.CleaveCylRange);
                }
            }

            return codes.AsEnumerable();
        }
        #endregion
    }
}