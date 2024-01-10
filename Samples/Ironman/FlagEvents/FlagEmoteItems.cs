namespace Ironman.FlagEvents;

[HarmonyPatchCategory(nameof(FlagEmoteItems))]
public static class FlagEmoteItems
{
    //Add Ironman to emote given items
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.TryCreateForGive), new Type[] { typeof(WorldObject), typeof(WorldObject) })]
    public static void PreTryCreateForGive(WorldObject giver, WorldObject itemBeingGiven, ref Player __instance, ref bool __result)
    {
        if (__instance is null || itemBeingGiven is null)
            return;

        if (__instance.GetProperty(FakeBool.Ironman) == true)
            itemBeingGiven.SetProperty(FakeBool.Ironman, true);

        __instance.SendMessage($"{itemBeingGiven.Name} now Ironman");
    }

}
