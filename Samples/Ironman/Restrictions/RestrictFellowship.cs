namespace Ironman.Restrictions;

[HarmonyPatchCategory(nameof(RestrictFellowship))]
public static class RestrictFellowship
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.FellowshipRecruit), new Type[] { typeof(Player) })]
    public static bool PreFellowshipRecruit(Player newPlayer, ref Player __instance)
    {
        if (newPlayer.GetProperty(FakeBool.Ironman) == true && __instance.GetProperty(FakeBool.Ironman) != true)
        {
            __instance.SendMessage($"You can't recruit {newPlayer.Name}.  Blame Ironmode");
            __instance.Session.Network.EnqueueSend(new GameEventWeenieError(__instance.Session, WeenieError.FellowshipIllegalLevel));
            return false;
        }

        return true;
    }

}
//[HarmonyPatchCategory(nameof(Restrict))]
//public static class Restrict
//{
//}
