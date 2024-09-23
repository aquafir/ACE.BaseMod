namespace EasyEnlightenment;

[HarmonyPatchCategory(nameof(WieldRequirements))]
public class WieldRequirements
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), "CheckWieldRequirements", new Type[] { typeof(WorldObject) })]
    public static bool PreCheckWieldRequirements(WorldObject item, ref Player __instance, ref WeenieError __result)
    {
        var req = item.GetProperty(FakeInt.ItemWieldRequirementEnlightenments);
        if (req is null)
            return true;

        else if (__instance.Enlightenment < req)
        {
            __result = WeenieError.SkillTooLow;
            __instance.SendMessage($"Unable to wield until you have {req} Enlightenments.");
            return false;
        }

        //Do regular checks
        return true;
    }
}