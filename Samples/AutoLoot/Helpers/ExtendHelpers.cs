using ACE.Database;
using ACE.Entity;
using ACE.Entity.Models;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects.Entity;
using static ACE.Server.WorldObjects.Player;

namespace AutoLoot.Helpers;
public static class InventoryHelpers
{
    public static bool TryTakeItems(this Player player, uint weenieClassId, int amount = 1)
    {
        if (player is null) return false;

        if (amount < 1)
        {
            ModManager.Log($"Invalid amount of items to take: {amount} of WCID {weenieClassId}", ModManager.LogLevel.Warn);
            return false;
        }

        if ((player.GetNumInventoryItemsOfWCID(weenieClassId) > 0 && player.TryConsumeFromInventoryWithNetworking(weenieClassId, amount == -1 ? int.MaxValue : amount))
            || (player.GetNumEquippedObjectsOfWCID(weenieClassId) > 0 && player.TryConsumeFromEquippedObjectsWithNetworking(weenieClassId, amount == -1 ? int.MaxValue : amount)))
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
            var transit = destItem.PhysicsObj.transition(destItem.PhysicsObj.Position, new ACE.Server.Physics.Common.Position(destItem.Location), false);

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
}

//public static class BiotaHelpers
//{
//    public static bool IsNpc(this ACE.Entity.Models.Biota biota)
//    {
//        //Assume npc
//        if (biota is null || biota.PropertiesBool is null || biota.PropertiesInt is null) return true;

//        //IsNPC => !(this is Player) && !Attackable && TargetingTactic == TargetingTactic.None;
//        //Attackable = GetProperty(PropertyBool.Attackable) ?? true;
//        if (!biota.PropertiesBool.TryGetValue(PropertyBool.Attackable, out var value) || value != false)
//            return false;

//        //TargetingTactic = (TargetingTactic)(GetProperty(PropertyInt.TargetingTactic) ?? 0);
//        if (!biota.PropertiesInt.TryGetValue(PropertyInt.TargetingTactic, out var target) || target != (int)ACE.Entity.Enum.TargetingTactic.None)
//            return false;

//        return true;
//    }
//}

//public static class WeenieHelpers
//{
//    public static bool IsNpc(this Weenie weenie)
//    {
//        //Assume npc
//        if (weenie is null) return true;

//        //Check NPC as no target, unattackable
//        var target = weenie.GetProperty(PropertyInt.TargetingTactic);
//        if (target is null || target != (int)ACE.Entity.Enum.TargetingTactic.None)
//            return false;

//        if (weenie.GetProperty(PropertyBool.Attackable) ?? true)
//            return false;

//        return true;
//    }
//}

public static class WorldObjectHelpers
{
    /// <summary>
    /// Play animation on a WorldObject
    /// </summary>
    public static void PlayAnimation(this WorldObject worldObject, PlayScript script, float speed = 1f) =>
        worldObject.EnqueueBroadcast(new GameMessageScript(worldObject.Guid, script, speed));

    //From AdminCommand TryCreateObject
    //public static void CreateInFront(this WorldObject obj, Player player, int? palette = null, float? shade = null, int? lifespan = null)
    //{
    //    obj.MoveInFrontOf(player);
    //    obj.EnterWorld();
    //}

    public static void MoveInFrontOf(this WorldObject obj, Player player)
    {
        if (obj.WeenieType == WeenieType.Creature)
            obj.Location = player.Location.InFrontOf(5f, true);
        else
        {
            var dist = Math.Max(2, obj.UseRadius ?? 2);

            obj.Location = player.Location.InFrontOf(dist);
        }

        obj.Location.LandblockId = new LandblockId(obj.Location.GetCell());
    }
}

public static class PlayerHelpers
{
    public static void PlaySound(this Player player, Sound sound, float volume = 1f) =>
        player.EnqueueBroadcast(new GameMessageSound(player.Guid, sound, volume));
}

public static class CreatureHelpers
{
    /// <summary>
    /// Percentage of current health
    /// </summary>
    public static float PercentHealth(this Creature creature) => (float)creature.Health.Current / creature.Health.MaxValue;


    /// <summary>
    /// Damage without source
    /// </summary>
    public static bool TryDamageDirect(this Creature creature, float amount, out uint damageTaken, DamageType damageType = DamageType.Undef, bool ignoreResistance = false)
    {
        damageTaken = 0;
        if (creature.IsDead || creature.Invincible) return false;

        // handle lifestone protection?
        if (creature is Player p && p.UnderLifestoneProtection)
        {
            p.HandleLifestoneProtection();
            return false;
        }

        // vital
        CreatureVital vital = damageType switch
        {
            DamageType.Stamina => creature.Stamina,
            DamageType.Mana => creature.Mana,
            _ => creature.Health
        };

        // scale by resistance?
        var resistance = ignoreResistance ? 1f : creature.GetResistanceMod(damageType, null, null);
        var damage = (uint)Math.Round(amount * resistance);

        // update vital
        damageTaken = (uint)-creature.UpdateVitalDelta(vital, (int)-damage);
        creature.DamageHistory.Add(creature, damageType, damageTaken);

        if (creature.Health.Current <= 0)
        {
            creature.OnDeath(new DamageHistoryInfo(creature), damageType, false);
            creature.Die();
        }

        return true;
    }

    public static void ScaleProperty(this WorldObject wo, PropertyInt property, float amount) => wo.SetProperty(property, (int)(amount * wo.GetProperty(property) ?? 0));
    public static void ScaleProperty(this WorldObject wo, PropertyFloat property, float amount) => wo.SetProperty(property, (double)(amount * wo.GetProperty(property) ?? 0));
    public static void ScaleProperty(this WorldObject wo, PropertyInt64 property, float amount) => wo.SetProperty(property, (long)(amount * wo.GetProperty(property) ?? 0));

    public static void ScaleAttributeBase(this Creature wo, float amount, params PropertyAttribute[] properties) =>
        Array.ForEach<PropertyAttribute>(properties, (property) =>
        {
            if (property != PropertyAttribute.Undef)
                wo.Attributes[property].StartingValue = (uint)(wo.Attributes[property].StartingValue * amount);
        });
    public static void ScaleAttributeBase(this Creature wo, float amount, params PropertyAttribute2nd[] properties) =>
        Array.ForEach<PropertyAttribute2nd>(properties, (property) =>
        {
            if (property != PropertyAttribute2nd.Undef)
                wo.Vitals[property].StartingValue = (uint)(wo.Vitals[property].StartingValue * amount);
        });

}

public static class RandomHelpers
{
    public static T GetRandom<T>(this IEnumerable<T> list) => list.GetRandomElements<T>(1).FirstOrDefault();
    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount) =>
        list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
}

public static class StringHelpers
{
    public static string Name(this CreatureVital vital) => vital switch
    {
    };
    public static string Repeat(this string input, int count)
    {
        if (string.IsNullOrEmpty(input) || count <= 1)
            return input;

        var builder = new StringBuilder(input.Length * count);

        for (var i = 0; i < count; i++) builder.Append(input);

        return builder.ToString();
    }
}

public static class EnchantmentHelpers
{
    /// <summary>
    /// Returns a random enchantment?
    /// </summary>
    public static bool TryGetRandomEnchantment(this Creature creature, out PropertiesEnchantmentRegistry random)
    {
        ICollection<PropertiesEnchantmentRegistry> value = creature.Biota.PropertiesEnchantmentRegistry;
        ReaderWriterLockSlim rwLock = creature.BiotaDatabaseLock;

        random = null;
        if (value == null)
            return false;

        rwLock.EnterReadLock();
        try
        {
            random = value.GetRandom<PropertiesEnchantmentRegistry>();
        }
        finally
        {
            rwLock.ExitReadLock();
        }

        return random is not null;
    }
}


public static class PositionHelpers
{
    //    public static Position Translate(this Position p, float distanceInFront, float radians = 0)
    //    {
    //        //Add rotation?
    //        //Quaternion.CreateFromYawPitchRoll()
    //        var pos = new Position();
    //        //pos.landblockId.Raw = p.LandblockId.Raw;
    //        pos.Rotation = p.Rotation;

    //        // Create a Quaternion representing the rotation
    //        Quaternion rotationQuaternion = Quaternion.CreateFromYawPitchRoll(radians, 0, 0);

    //        // Multiply a unit vector by distance/rotation
    //        Vector3 rotatedPosition = Vector3.Transform(Vector3.One * distanceInFront, rotationQuaternion);

    //        // Add the rotated position to the original position to obtain the translated position
    //        pos.Pos = p.Pos + rotatedPosition;

    //        return pos;
    //        //p.FindZ()
    ////        return new Position(p.LandblockId.Raw, p.PositionX + num2, p.PositionY + num3, p.PositionZ + num4, 0f, 0f, rotationZ, rotationW);
    //    }
}