namespace Expansion.Features;

[CommandCategory(nameof(Feature.ProcRateOverride))]
[HarmonyPatchCategory(nameof(Feature.ProcRateOverride))]
public class ProcRateOverride
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Cloak), nameof(Cloak.RollProc), new Type[] { typeof(WorldObject), typeof(float) })]
    public static bool PreRollProc(WorldObject cloak, float damage_percent, ref Cloak __instance, ref bool __result)
    {
        __result = ThreadSafeRandom.Next(0, 1.0f) < PatchClass.Settings.CloakProcRate;

        return false;
    }

    //Proc chance is based on level / augs / combat mode
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Aetheria), nameof(Aetheria.CalcProcRate), new Type[] { typeof(WorldObject), typeof(Creature) })]
    public static bool PreCalcProcRate(WorldObject aetheria, Creature wielder, ref Aetheria __instance, ref float __result)
    {
        __result = PatchClass.Settings.AetheriaProcRate;

        return false;
    }
}
