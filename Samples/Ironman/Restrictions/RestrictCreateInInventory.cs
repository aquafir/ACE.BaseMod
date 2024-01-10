namespace Ironman.Restrictions;

//Check on add to inventory
[HarmonyPatchCategory(nameof(RestrictCreateInInventory))]
public static class RestrictCreateInInventory
{
    //Check on add to inventory
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.TryCreateInInventoryWithNetworking), new Type[] { typeof(WorldObject), typeof(Container) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    public static bool PreTryCreateInInventoryWithNetworking(WorldObject item, Container container, ref Player __instance, ref bool __result)
    {
        //Skip non-Ironman players
        if (__instance is null || __instance.GetProperty(FakeBool.Ironman) != true)
            return true;

        if (item.GetProperty(FakeBool.Ironman) != true)
        {
            __result = false;
            __instance.SendMessage($"{item.Name} unable to be added to inventory of non-Ironman");
            return false;
        }

        return true;
    }
}
//[HarmonyPatchCategory(nameof(Restrict))]
//public static class Restrict
//{
//}
