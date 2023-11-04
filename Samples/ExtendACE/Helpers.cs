using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network;
using ACE.Server.WorldObjects.Entity;
using System.Text;
using ACE.Server.WorldObjects;
using System.Diagnostics;
using static ACE.Server.WorldObjects.Player;
using ACE.Server.WorldObjects.Managers;
using ExtendACE.Creatures;
using System;
using ACE.Server.Managers;
using ACE.Server.Command.Handlers;
using static ACE.Server.Physics.Common.LandDefs;
using System.Numerics;

namespace ExtendACE;

public static class SplashHelpers
{
    /// <summary>
    /// Gets a list of PhysicsObj visible to a Player sorted by distance using their visible objects
    /// </summary>
    private static List<PhysicsObj> GetVisibleCreaturesByDistance(this Player origin) => origin.GetVisibleCreaturesByDistance(origin);
    /// <summary>
    /// Gets a list of PhysicsObj sorted by distance from a WorldObject using objects visible to a reference Player
    /// </summary>
    /// <param name="reference">A player with similar vision to the origin</param>
    /// <param name="origin">Center of distance comparison</param>
    /// <returns></returns>
    private static List<PhysicsObj> GetVisibleCreaturesByDistance(this Player reference, WorldObject origin)
    {
        var visible = reference.PhysicsObj.ObjMaint.GetVisibleObjectsValuesWhere(o =>
            o.WeenieObj.IsPlayer() ||
            o.WeenieObj.WorldObject.WeenieType == WeenieType.Creature &&    //Restrict to creature weenies here for speed?
            o.WeenieObj.WorldObject != null);

        //Include reference which wouldn't be present in its own ObjMaint?
        visible.Add(reference.PhysicsObj);

        visible.Sort((x, y) => origin.Location.SquaredDistanceTo(x.WeenieObj.WorldObject.Location)
                    .CompareTo(origin.Location.SquaredDistanceTo(y.WeenieObj.WorldObject.Location)));

        return visible;
    }

    //Todo: implement on per-landblock level if reference approach doesn't work.  Maybe look at spatial hashing
    /// <summary>
    /// Gets a list of PhysicsObj visible to a WorldObject sorted by distance
    /// </summary>
    //public static List<PhysicsObj> GetVisibleCreaturesByDistance(this WorldObject origin)
    //{
    //    var visible = origin.PhysicsObj.ObjMaint.GetVisibleObjectsValuesWhere(o =>
    //    o.WeenieObj.WorldObject.WeenieType == WeenieType.Creature &&    //Restrict to creature weenies here for speed?
    //    o.WeenieObj.WorldObject != null);

    //    visible.Sort((x, y) => origin.Location.SquaredDistanceTo(x.WeenieObj.WorldObject.Location)
    //                .CompareTo(origin.Location.SquaredDistanceTo(y.WeenieObj.WorldObject.Location)));

    //    return visible;
    //}


    /// <summary>
    /// Gets nearby creatures or players using a reference Player
    /// </summary>
    /// <returns>List of objects surrounding a WorldObject</returns>
    public static List<Creature> GetNearby(this Player reference, WorldObject origin, int maxTargets = 3, float maxRange = 5.0f, bool includePlayers = true, bool includeCreatures = true)
    {
        if (origin is null || reference is null)
        {
            ModManager.Log($"Failed to get splash targets.", ModManager.LogLevel.Warn);
            return new List<Creature>();
        }

        var visible = reference.GetVisibleCreaturesByDistance(origin);

        //var sb = new StringBuilder("\r\n");
        //foreach (var obj in visible.Where(x => x.WeenieObj.IsPlayer()))
        //{
        //    var creature = obj.WeenieObj.WorldObject as Creature;
        //    sb.AppendLine($"{creature.Name} - {origin.GetCylinderDistance(creature)}");
        //}
        //ModManager.Log(sb.ToString());
        //reference.SendMessage(sb.ToString());

        var splashTargets = new List<Creature>();
        foreach (var obj in visible)
        {
            var creature = obj.WeenieObj.WorldObject as Creature;

            //No more objects in range
            var cylDist = origin.GetCylinderDistance(creature);
            if (cylDist > maxRange)
                return splashTargets;

            //Splashing skips original target?
            if (obj.ID == origin.PhysicsObj.ID)
                continue;

            //Only splash creatures?
            bool isPlayer = creature is Player;
            if (creature is null || creature.Teleporting || creature.IsDead) continue;

            //Check for type
            if ((isPlayer && !includePlayers) || (!isPlayer && !includeCreatures)) continue;

            //Check attackable
            if (!creature.Attackable) continue;

            //Check combat pet?
            //if (creature is CombatPet && (player != null || this is CombatPet)) continue;

            //Check pk?
            //if (player != null && player.CheckPKStatusVsTarget(creature, null) != null) continue;

            //Filter by angle from origin?  Could be based on eithers orientation?
            //var angle = creature.GetAngle(origin);
            //if (Math.Abs(angle) > splashAngle / 2.0f)
            //    continue;

            //Found splash object
            splashTargets.Add(creature);

            //Stop if you've found enough targets
            if (splashTargets.Count == maxTargets)
                break;
        }
        return splashTargets;
    }
    public static List<Creature> GetNearbyCreatures(this Player reference, WorldObject origin, int maxTargets = 3, float maxRange = 5.0f) => GetNearby(reference, origin, maxTargets, maxRange, false, true);
    public static List<Creature> GetNearbyPlayers(this Player reference, WorldObject origin, int maxTargets = 3, float maxRange = 5.0f) => GetNearby(reference, origin, maxTargets, maxRange, true, false);
}

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


public static class CreatureExHelpers
{
    public static CreatureEx Create(this Creatures.CreatureType type, Biota biota) => type switch
    {
        //Creatures.CreatureType.Accurate => new Accurate(biota),
        //Creatures.CreatureType.Avenger => new Avenger(biota),
        //Creatures.CreatureType.Bard => new Bard(biota),
        //Creatures.CreatureType.Boss => new Boss(biota),
        //Creatures.CreatureType.Berserker => new Berserker(biota),
        //Creatures.CreatureType.Comboer => new Comboer(biota),
        //Creatures.CreatureType.Drainer => new Drainer(biota),
        //Creatures.CreatureType.Duelist => new Duelist(biota),
        //Creatures.CreatureType.Evader => new Evader(biota),
        //Creatures.CreatureType.Exploding => new Exploder(biota),
        //Creatures.CreatureType.Healer => new Healer(biota),
        //Creatures.CreatureType.Launcher => new Launcher(biota),
        //Creatures.CreatureType.Merging => new Merger(biota),
        //Creatures.CreatureType.Necromancer => new Necromancer(biota),
        //Creatures.CreatureType.Poisoner => new Poisoner(biota),
        //Creatures.CreatureType.Puppeteer => new Puppeteer(biota),
        //Creatures.CreatureType.Reaper => new Reaper(biota),
        //Creatures.CreatureType.Rogue => new Rogue(biota),
        //Creatures.CreatureType.Runner => new Runner(biota),
        //Creatures.CreatureType.Shielded => new Shielded(biota),
        //Creatures.CreatureType.SpellBreaker => new SpellBreaker(biota),
        //Creatures.CreatureType.SpellThief => new SpellThief(biota),
        //Creatures.CreatureType.Splitter => new Splitter(biota),
        //Creatures.CreatureType.Stomper => new Stomper(biota),
        //Creatures.CreatureType.Stunner => new Stunner(biota),
        //Creatures.CreatureType.Suppresser => new Suppresser(biota),
        //Creatures.CreatureType.Tank => new Tank(biota),
        //Creatures.CreatureType.Vampire => new Vampire(biota),
        //Creatures.CreatureType.Warder => new Warder(biota),
        _ => new CreatureEx(biota),             // throw new NotImplementedException(),
    };
    public static CreatureEx Create(this Creatures.CreatureType type, Weenie weenie, ObjectGuid guid) => type switch
    {
        Creatures.CreatureType.Accurate => new Accurate(weenie, guid),
        //Creatures.CreatureType.Avenger => new Avenger(weenie, guid),
        //Creatures.CreatureType.Bard => new Bard(weenie, guid),
        Creatures.CreatureType.Boss => new Boss(weenie, guid),
        Creatures.CreatureType.Berserker => new Berserker(weenie, guid),
        Creatures.CreatureType.Comboer => new Comboer(weenie, guid),
        Creatures.CreatureType.Drainer => new Drainer(weenie, guid),
        Creatures.CreatureType.Duelist => new Duelist(weenie, guid),
        Creatures.CreatureType.Evader => new Evader(weenie, guid),
        Creatures.CreatureType.Exploding => new Exploder(weenie, guid),
        Creatures.CreatureType.Healer => new Creatures.Healer(weenie, guid),
        //Creatures.CreatureType.Merging => new Merger(weenie, guid),
        //Creatures.CreatureType.Necromancer => new Necromancer(weenie, guid),
        //Creatures.CreatureType.Poisoner => new Poisoner(weenie, guid),
        Creatures.CreatureType.Puppeteer => new Puppeteer(weenie, guid),
        //Creatures.CreatureType.Reaper => new Reaper(weenie, guid),
        Creatures.CreatureType.Rogue => new Rogue(weenie, guid),
        //Creatures.CreatureType.Runner => new Runner(weenie, guid),
        Creatures.CreatureType.Shielded => new Shielded(weenie, guid),
        Creatures.CreatureType.SpellBreaker => new SpellBreaker(weenie, guid),
        Creatures.CreatureType.SpellThief => new SpellThief(weenie, guid),
        //Creatures.CreatureType.Splitter => new Splitter(weenie, guid),
        //Creatures.CreatureType.Stomper => new Stomper(weenie, guid),
        Creatures.CreatureType.Stunner => new Stunner(weenie, guid),
        //Creatures.CreatureType.Suppresser => new Suppresser(weenie, guid),
        Creatures.CreatureType.Tank => new Tank(weenie, guid),
        Creatures.CreatureType.Vampire => new Vampire(weenie, guid),
        Creatures.CreatureType.Warder => new Warder(weenie, guid),
        _ => new Stunner(weenie, guid),      //throw new NotImplementedException(),
    };
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