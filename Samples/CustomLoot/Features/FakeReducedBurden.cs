using ACE.Entity;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;
using CustomLoot.Enums;
using System.ComponentModel;
using System.Xml.Linq;
using static ACE.Server.WorldObjects.Player;


namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakeReducedBurden))]
internal class FakeReducedBurden
{
    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(Container), nameof(Container.TryAddToInventory), new Type[] { typeof(WorldObject), typeof(Container), typeof(int), typeof(bool), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
    //public static void PostTryAddToInventory(WorldObject worldObject, Container container, int placementPosition, bool limitToMainPackOnly, bool burdenCheck, ref Container __instance, ref bool __result)
    //{
    //    //Skip on failure
    //    if (!__result)
    //        return;

    //    //Check if container is or resides on player

    //    if (container is not Player player)
    //    {
    //        var guid = new ObjectGuid(container.OwnerId ?? 0u);

    //        if (!guid.IsPlayer())
    //            return;

    //        player = PlayerManager.GetOnlinePlayer(guid);
    //    }

    //    //Item added to container has weight offset
    //    var reduction = container.GetProperty(FakeFloat.ReducedBurdenPercent) ?? 1;

    //    //Find/store original weight        
    //    var originalWeight = worldObject.GetProperty(FakeInt.OriginalWeight);
    //    if (originalWeight is null)
    //    {
    //        worldObject.SetProperty(FakeInt.OriginalWeight, worldObject.EncumbranceVal ?? 0);
    //        originalWeight = worldObject.EncumbranceVal;
    //    }
    //    var newWeight = (int)(originalWeight * reduction);
    //    var change = originalWeight - newWeight;

    //    //Set weight of object
    //    worldObject.EncumbranceVal = newWeight;

    //    //Old weight got added already to parents so subtract the difference
    //    //if (container.Guid != player.Guid) 
    //    //    container.EncumbranceVal -= change;
    //    player.EncumbranceVal -= change;

    //    player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(worldObject, PropertyInt.EncumbranceVal, container.EncumbranceVal ?? 0));
    //    player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(container, PropertyInt.EncumbranceVal, container.EncumbranceVal ?? 0));
    //    player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.EncumbranceVal, container.EncumbranceVal ?? 0));
    //    player.SendMessage($"{worldObject.Name} was {originalWeight} before being reduced {reduction:P0} to {worldObject.EncumbranceVal} for a change of {change}.");
    //}

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(Container), nameof(Container.TryRemoveFromInventory), new Type[] { typeof(ObjectGuid), typeof(WorldObject), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal })]
    //public static void PostTryRemoveFromInventory(ObjectGuid objectGuid, WorldObject item, bool forceSave, ref Container __instance, ref bool __result)
    //{
    //    //Skip if this isn't a player or unable to add
    //    if (__instance is not Player player || !__result)
    //        return;

    //    //Item added to container has weight offset
    //    var reduction = container.GetProperty(FakeFloat.ReducedBurden);
    //    if (reduction is null)
    //        return;

    //    var change = (int)(worldObject.EncumbranceVal * reduction);
    //    player.EncumbranceVal -= change;
    //    player.SendMessage($"{worldObject.Name} had weight reduced by {change}, {reduction:P2} of {worldObject.EncumbranceVal}");
    //}

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(Player), nameof(Player.TryRemoveFromInventoryWithNetworking), new Type[] { typeof(ObjectGuid), typeof(WorldObject), typeof(RemoveFromInventoryAction) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal })]
    //public static void PostTryRemoveFromInventoryWithNetworking(ObjectGuid objectGuid, WorldObject item, RemoveFromInventoryAction removeFromInventoryAction, ref Player __instance, ref bool __result)
    //{
    //    //Skip if this isn't a player or unable to add
    //    if (!__result)
    //        return;

    //    //Item added to container has weight offset
    //    var reduction = item.Container?.GetProperty(FakeFloat.ReducedBurdenPercent);
    //    if (reduction is null)
    //        return;

    //    var change = (int)(item.EncumbranceVal * reduction);
    //    __instance.EncumbranceVal += change;
    //    __instance.SendMessage($"Player no longer has {item.Name}'s weight reduced by {change}, {reduction:P2} of {item.EncumbranceVal}");
    //}

}
