using System.Reflection.Emit;
using System.Text.Json.Serialization;

namespace CleaveTranspiler
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
        [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.IsCleaving), MethodType.Getter)]
        public static bool IsCleaving(WorldObject __instance, ref bool __result)
        {
            //All melee weapons are cleaving
            __result = __instance is MeleeWeapon;

            //Override IsCleaving
            return false;
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

        #region Start/Shutdown
        public static void Start()
        {
            LoadSettings();
        }

        public static void Shutdown()
        {
            //Clean up what you need to...
            //SaveSettings();
        }
        #endregion
    }
}