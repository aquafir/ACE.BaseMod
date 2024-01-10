namespace Ironman.Restrictions;

[HarmonyPatchCategory(nameof(RestrictContainerVerify))]
public static class RestrictContainerVerify
{
    //Verify container / pickup check
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionPutItemInContainer_Verify), new Type[] { typeof(uint), typeof(uint), typeof(int), typeof(Container), typeof(WorldObject), typeof(Container), typeof(Container), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Out, ArgumentType.Out, ArgumentType.Out })]
    public static void PreHandleActionPutItemInContainer_Verify(uint itemGuid, uint containerGuid, int placement, Container itemRootOwner, WorldObject item, Container containerRootOwner, Container container, bool itemWasEquipped, ref Player __instance, ref bool __result)
    {
        //Many parameters will not be populated in a prefix.  Runs other checks first
        //If the container is Ironman and the item is not reject it
        if (containerRootOwner is Player player && player?.GetProperty(FakeBool.Ironman) == true && item?.GetProperty(FakeBool.Ironman) != true)
        {
            player.Session.Network.EnqueueSend(new GameEventWeenieError(player.Session, WeenieError.YoureTooBusy));
            player.Session.Network.EnqueueSend(new GameEventInventoryServerSaveFailed(player.Session, itemGuid));
            player.SendMessage($"Unable to loot non-Ironman {item?.Name}");
            __result = false;
        }
    }

}
//[HarmonyPatchCategory(nameof(Restrict))]
//public static class Restrict
//{
//}
