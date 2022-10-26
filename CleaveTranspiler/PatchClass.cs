using System.Reflection.Emit;

namespace CleaveTranspiler
{
    [HarmonyPatch]
    public class PatchClass
    {
        private static Settings _settings = new();
        private static string filePath = Path.Combine(Mod.ModPath, "Settings.json");

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
            __result = _settings.CleaveTargets;
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
                    //ModManager.Log($"Replace cleave angle with modded value: {_settings.CleaveAngle}");
                    codes[i] = new CodeInstruction(OpCodes.Ldc_R4, _settings.CleaveAngle);
                }
                if(codes[i].LoadsField(f_cleaveCylDistance))
                {
                    //ModManager.Log($"Replace cleave angle with modded value: {_settings.CleaveCylRange}");
                    codes[i] = new CodeInstruction(OpCodes.Ldc_R4, _settings.CleaveCylRange);
                }
            }

            return codes.AsEnumerable();
        }


        public static void Start()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    ModManager.Log($"Loading from {filePath}");
                    var jsonString = File.ReadAllText(filePath);
                    _settings = JsonSerializer.Deserialize<Settings>(jsonString);
                }
                catch (Exception ex)
                {
                    ModManager.Log($"Failed to deserialize from {filePath}");
                    _settings = new Settings();
                    return;
                }
            }
            else
            {
                ModManager.Log($"Creating {filePath}");
                string jsonString = JsonSerializer.Serialize(_settings);
                File.WriteAllText(filePath, jsonString);
            }
        }
        public static void Shutdown()
        {
            //string jsonString = JsonSerializer.Serialize(_settings);
            //File.WriteAllText(filePath, jsonString);
        }
    }
}