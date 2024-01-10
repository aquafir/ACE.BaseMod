using ACE.Entity;
using ACE.Entity.Models;
using ACE.Server.Network.GameMessages.Messages;
using static ACE.Server.WorldObjects.Player;

namespace ACE.Shared.Helpers;

public static class InventoryExtensions
{
    public static bool TryTakeItems(this Player player, uint weenieClassId, int amount = 1)
    {
        if (player is null) return false;

        if (amount < 1)
        {
            ModManager.Log($"Invalid amount of items to take: {amount} of WCID {weenieClassId}", ModManager.LogLevel.Warn);
            return false;
        }

        if (player.GetNumInventoryItemsOfWCID(weenieClassId) > 0 && player.TryConsumeFromInventoryWithNetworking(weenieClassId, amount == -1 ? int.MaxValue : amount)
            || player.GetNumEquippedObjectsOfWCID(weenieClassId) > 0 && player.TryConsumeFromEquippedObjectsWithNetworking(weenieClassId, amount == -1 ? int.MaxValue : amount))
        {
            var itemTaken = DatabaseManager.World.GetCachedWeenie(weenieClassId);
            if (itemTaken != null)
            {
                var amt = amount == -1 ? "all" : amount.ToString();
                var msg = $"You hand over {amount} of your {itemTaken.GetPluralName()}.";

                player.Session.Network.EnqueueSend(new GameMessageSystemChat(msg, ChatMessageType.Broadcast));
                return true;
            }
        }
        return true;
    }

    /// <summary>
    /// Repurpose the fumble command (Dequip commands failed)
    /// </summary>
    public static bool TryDropItem(this Player player, WorldObject item, DequipObjectAction action = DequipObjectAction.DequipToPack)
    {
        var session = player.Session;
        var playerLoc = new Position(player.Location);
        WorldObject destItem;

        //Todo: fix inventory drop
        //Drop from inventory
        //var equipped = player.EquippedObjects.ContainsKey(item.Guid);
        //if (!equipped && !player.TryRemoveFromInventoryWithNetworking(item.Guid, out destItem, Player.RemoveFromInventoryAction.DropItem))
        //    return false;


        if (action == DequipObjectAction.DequipToPack)
        {
            //DequipWithNetworking changes stance/mode, sends these (inc. encumberance if off player):
            //new GameMessagePublicUpdateInstanceID(item, PropertyInstanceId.Wielder, ObjectGuid.Invalid),
            //    new GameMessagePublicUpdatePropertyInt(item, PropertyInt.CurrentWieldedLocation, 0),
            //    new GameMessagePickupEvent(item),
            //    new GameMessageSound(Guid, Sound.UnwieldObject));
            if (!player.TryDequipObjectWithNetworking(item.Guid.Full, out destItem, action))
                return false;

            if (!player.TryCreateInInventoryWithNetworking(destItem, out var container))
                return false;

            player.Session.Network.EnqueueSend(
            new GameMessagePublicUpdateInstanceID(destItem, PropertyInstanceId.Container, destItem.Container.Guid),
            //new GameMessagePublicUpdateInstanceID(destItem, PropertyInstanceId.Wielder, ObjectGuid.Invalid),
            new GameEventItemServerSaysMoveItem(player.Session, destItem)
            //new GameMessageUpdatePosition(destItem)
            );



            //player.EnqueueBroadcast(new GameMessageSound(player.Guid, Sound.DropItem));

            destItem.EmoteManager.OnDrop(player);
            destItem.SaveBiotaToDatabase();
            return true;
        }

        //Drop equipped
        if (!player.TryDequipObjectWithNetworking(item.Guid.Full, out destItem, DequipObjectAction.DropItem))
            return false;

        player.SavePlayerToDatabase();

        destItem.Location = new Position(playerLoc);
        destItem.Location.PositionZ += .5f;
        destItem.Placement = Placement.Resting;  // This is needed to make items lay flat on the ground.

        //Drop item to world
        // increased precision for non-ethereal objects
        var ethereal = destItem.Ethereal;
        destItem.Ethereal = true;

        if (player.CurrentLandblock?.AddWorldObject(destItem) ?? false)
        {
            destItem.Location.LandblockId = new LandblockId(destItem.Location.GetCell());

            // try slide to new position
            var transit = destItem.PhysicsObj.transition(destItem.PhysicsObj.Position, new Server.Physics.Common.Position(destItem.Location), false);

            if (transit != null && transit.SpherePath.CurCell != null)
            {
                destItem.PhysicsObj.SetPositionInternal(transit);
                destItem.SyncLocation();
                destItem.SendUpdatePosition(true);
            }
            destItem.Ethereal = ethereal;

            // drop success
            player.Session.Network.EnqueueSend(
                new GameMessagePublicUpdateInstanceID(destItem, PropertyInstanceId.Container, ObjectGuid.Invalid),
                new GameMessagePublicUpdateInstanceID(destItem, PropertyInstanceId.Wielder, ObjectGuid.Invalid),
                new GameEventItemServerSaysMoveItem(player.Session, destItem),
                new GameMessageUpdatePosition(destItem));

            player.EnqueueBroadcast(new GameMessageSound(player.Guid, Sound.DropItem));

            destItem.EmoteManager.OnDrop(player);
            destItem.SaveBiotaToDatabase();
            return true;
        }

        return false;
    }

    public static void WipeInventory(this Player player, bool equipment = false)
    {
        foreach (var item in player.Inventory.Values)
            item.DeleteObject(player);

        if (equipment)
        {
            foreach (var item in player.EquippedObjects.Values)
                item.DeleteObject(player);
        }

        player.SendMessage($"Inventory wiped.");
    }
}
