namespace Expansion.Features;

[CommandCategory(nameof(Feature.OverrideCheckUseRequirements))]
[HarmonyPatchCategory(nameof(Feature.OverrideCheckUseRequirements))]
internal class OverrideCheckUseRequirements
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CheckUseRequirements), new Type[] { typeof(WorldObject) })]
    public static bool PreCheckUseRequirements(WorldObject activator, ref WorldObject __instance, ref ActivationResult __result)
    {
        __result = activator?.CheckUseRequirements();
        return false;
    }
}


