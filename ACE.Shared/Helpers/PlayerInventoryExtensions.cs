using static ACE.Server.WorldObjects.Player;

namespace ACE.Shared.Helpers;

public static class PlayerInventoryExtensions
{
    /// <summary>
    /// Attempts to take an amount of items with a WCID from a player. Based on EmoteType.TakeItems
    /// </summary>
    public static bool TryTakeItems(this Player player, uint weenieClassId, int amount = 1)
    {
        if (player is null) return false;

        if (amount < 1)
        {
            //ModManager.Log($"Invalid amount of items to take: {amount} of WCID {weenieClassId}", ModManager.LogLevel.Warn);
            return false;
        }

        //Only try to consume from inventory, not equipped
        //|| player.GetNumEquippedObjectsOfWCID(weenieClassId) > 0 && player.TryConsumeFromEquippedObjectsWithNetworking(weenieClassId, amount == -1 ? int.MaxValue : amount))
        var owned = player.GetNumInventoryItemsOfWCID(weenieClassId);
        if (owned < 0 || owned < amount)
            return false;

        return player.TryConsumeFromInventoryWithNetworking(weenieClassId, amount);
    }

    /// <summary>
    /// Returns inventory including side packs
    /// </summary>
    public static List<WorldObject> AllItems(this Player player)
    {
        if (player is null) return new();

        List<WorldObject> items = new(player.Inventory.Values);

        var containers = player.Inventory.Values.OfType<Container>().ToList();
        containers.Sort((a, b) => (a.Placement ?? 0).CompareTo(b.Placement ?? 0));

        foreach (var sidePack in containers)
            items.AddRange(sidePack.Inventory.Values);

        return items;
    }

    /// <summary>
    /// Repurpose the fumble command (Dequip commands failed)
    /// </summary>
    public static bool TryDropItem(this Player player, WorldObject item, DequipObjectAction action = DequipObjectAction.DequipToPack)
    {
        var session = player.Session;
#if REALM
        var playerLoc = new Server.Realms.InstancedPosition(player.Location);
#else
        var playerLoc = new Position(player.Location);
#endif
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

#if REALM
        destItem.Location = new Server.Realms.InstancedPosition(playerLoc).SetPositionZ(playerLoc.PositionZ + .5f);
#else
        destItem.Location = new Position(playerLoc);
        destItem.Location.PositionZ += .5f;
#endif
        destItem.Placement = Placement.Resting;  // This is needed to make items lay flat on the ground.

        //Drop item to world
        // increased precision for non-ethereal objects
        var ethereal = destItem.Ethereal;
        destItem.Ethereal = true;

        if (player.CurrentLandblock?.AddWorldObject(destItem) ?? false)
        {
            // try slide to new position
#if REALM
            destItem.Location = destItem.Location.SetLandblockId(new LandblockId(destItem.Location.GetCell()));
            var transit = destItem.PhysicsObj.transition(destItem.PhysicsObj.Position, new Server.Physics.Common.PhysicsPosition(destItem.Location), false);
#else
            destItem.Location.LandblockId = new LandblockId(destItem.Location.GetCell());
            var transit = destItem.PhysicsObj.transition(destItem.PhysicsObj.Position, new Server.Physics.Common.Position(destItem.Location), false);
#endif


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

    /// <summary>
    /// Remove any equipped items
    /// </summary>
    /// <param name="player"></param>
    public static void DequipAllItems(this Player player)
    {
        var equippedObjects = player.EquippedObjects.Keys.ToList();

#if REALM
        var iid = player.Location.Instance;
        foreach (var equippedObject in equippedObjects)
            player.HandleActionPutItemInContainer(new ObjectGuid(equippedObject.Full, iid), player.Guid, 0);
#else
        foreach (var equippedObject in equippedObjects)
            player.HandleActionPutItemInContainer(equippedObject.Full, player.Guid.Full, 0);
#endif        
}

    /// <summary>
    /// Remove items from inventory and optionally equipment
    /// </summary>
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

    /// <summary>
    /// Delete an item from a player's inventory
    /// </summary>
    public static void DeleteItem(this Player player, WorldObject wo)
    {
        wo.DeleteObject(player);
        //Unsure if needed?
        //player.Session.Network.EnqueueSend(new GameMessageDeleteObject(wo));
    }

    public static bool TryGetOwner(this WorldObject wo, out Player owner)
    {
        owner = PlayerManager.GetOnlinePlayer(wo.OwnerId ?? 0);
        return owner is not null;
    }
}
