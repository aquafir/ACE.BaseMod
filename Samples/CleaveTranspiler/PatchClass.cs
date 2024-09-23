using System.Reflection.Emit;

namespace CleaveTranspiler;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override Task OnStartSuccess()
    {
        Debugger.Break();
        ModC.RegisterCommandsTower(nameof(TranspilerPatches));


        return Task.CompletedTask;
    }
}

[HarmonyPatchCategory(nameof(TranspilerPatches))]
public class TranspilerPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.IsCleaving), MethodType.Getter)]
    public static bool IsCleaving(WorldObject __instance, ref bool __result)
    {
        if (PatchClass.Settings.AllMeleeCleaves)
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
        __result = PatchClass.Settings.CleaveTargets;
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
                codes[i] = new CodeInstruction(OpCodes.Ldc_R4, PatchClass.Settings.CleaveAngle);
            }
            if (codes[i].LoadsField(f_cleaveCylDistance))
            {
                //ModManager.Log($"Replace cleave angle with modded value: {Settings.CleaveCylRange}");
                codes[i] = new CodeInstruction(OpCodes.Ldc_R4, PatchClass.Settings.CleaveCylRange);
            }
        }

        return codes.AsEnumerable();
    }
}