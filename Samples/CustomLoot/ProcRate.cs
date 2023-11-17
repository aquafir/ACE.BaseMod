namespace CustomLoot;

[HarmonyPatchCategory(Settings.ProcOverrideCategory)]
public class ProcRate
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Cloak), nameof(Cloak.RollProc), new Type[] { typeof(WorldObject), typeof(float) })]
    public static bool PreRollProc(WorldObject cloak, float damage_percent, ref Cloak __instance, ref bool __result)
    {
        __result = ThreadSafeRandom.Next(0, 1.0f) < PatchClass.Settings.CloakProcRate;
        //__result = true;
        //Return false to override
        return false;
    }
}
