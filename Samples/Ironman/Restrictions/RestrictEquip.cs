namespace Ironman.Restrictions;

[HarmonyPatchCategory(nameof(RestrictEquip))]
public static class RestrictEquip
{
    //Check on wield
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.CheckWieldRequirements), new Type[] { typeof(WorldObject) })]
    public static bool PreCheckWieldRequirementsCustom(WorldObject item, ref Player __instance, ref WeenieError __result)
    {
        //Add check only to Ironman players
        if (__instance is null || __instance.GetProperty(FakeBool.Ironman) != true)
            return true;

        if (item.GetProperty(FakeBool.Ironman) != true)
        {
            __instance.SendMessage($"Unable to wield non-Ironman items!");

            __result = WeenieError.BeWieldedFailure;
            return false;
        }

        //Do regular checks
        return true;
    }
}
//[HarmonyPatchCategory(nameof(Restrict))]
//public static class Restrict
//{
//}
