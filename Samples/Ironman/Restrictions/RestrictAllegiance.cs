namespace Ironman.Restrictions;

[HarmonyPatchCategory(nameof(RestrictAllegiance))]
public static class RestrictAllegiance
{
    //Check allegiance
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.IsPledgable), new Type[] { typeof(Player) })]
    public static bool PreIsPledgable(Player target, ref Player __instance, ref bool __result)
    {
        if (target.GetProperty(FakeBool.Ironman) == true && __instance.GetProperty(FakeBool.Ironman) != true)
        {
            __instance.SendMessage($"You can't swear to {target.Name}.  Blame Ironmode");
            __instance.Session.Network.EnqueueSend(new GameEventWeenieError(__instance.Session, WeenieError.OlthoiCannotJoinAllegiance));
            __result = false;
            return false;
        }

        return true;
    }
}