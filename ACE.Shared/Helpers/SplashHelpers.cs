using ACE.Server.Physics;

namespace ACE.Shared.Helpers;

public static class SplashHelper
{
    /// <summary>
    /// Gets a list of PhysicsObj visible to a Player sorted by distance using their visible objects
    /// </summary>
    public static List<PhysicsObj> GetVisibleCreaturesByDistance(this Player origin) => origin.GetVisibleCreaturesByDistance(origin);

    /// <summary>
    /// Gets a list of PhysicsObj sorted by distance from a WorldObject using objects visible to a reference Player
    /// </summary>
    /// <param name="reference">A player with similar vision to the origin</param>
    /// <param name="origin">Center of distance comparison</param>
    /// <returns></returns>
    public static List<PhysicsObj> GetVisibleCreaturesByDistance(this Player reference, WorldObject origin)
    {
        var visible = reference.PhysicsObj.ObjMaint.GetVisibleObjectsValuesWhere(o =>
            o.WeenieObj.WorldObject.WeenieType == WeenieType.Creature &&    //Restrict to creature weenies here for speed?
            o.WeenieObj.WorldObject != null);

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
    //
    //    visible.Sort((x, y) => origin.Location.SquaredDistanceTo(x.WeenieObj.WorldObject.Location)
    //                .CompareTo(origin.Location.SquaredDistanceTo(y.WeenieObj.WorldObject.Location)));

    //    return visible;
    //}

    /// <summary>
    /// Gets the Performs a cleaving attack for two-handed weapons
    /// </summary>
    /// <returns>List of objects surrounding a WorldObject</returns>
    public static List<Creature> GetSplashTargets(this Player reference, WorldObject origin, int maxTargets = 3, float maxRange = 5.0f)
    {
        if (origin is null || reference is null)
        {
            ModManager.Log($"Failed to get splash targets.", ModManager.LogLevel.Warn);
            return new List<Creature>();
        }
        //List<PhysicsObj> visible;
        //// sort visible objects by ascending distance
        //if (origin is Player)
        //    visible = (origin as Player).GetVisibleCreaturesByDistance();

        //else visible = null;
        //origin.GetVisibleCreaturesByDistance();
        var visible = reference.GetVisibleCreaturesByDistance(origin);

        var splashTargets = new List<Creature>();

        foreach (var obj in visible)
        {
            //Pplashing skips original target?
            if (obj.ID == origin.PhysicsObj.ID)
                continue;

            //Only splash creatures?
            var creature = obj.WeenieObj.WorldObject as Creature;
            if (creature == null || creature.Teleporting || creature.IsDead) continue;

            //if (player != null && player.CheckPKStatusVsTarget(creature, null) != null)
            //    continue;

            if (!creature.Attackable && creature.TargetingTactic == TargetingTactic.None || creature.Teleporting)
                continue;

            //if (creature is CombatPet && (player != null || this is CombatPet))
            //    continue;

            //No objects in range
            var cylDist = origin.GetCylinderDistance(creature);
            if (cylDist > maxRange)
                return splashTargets;

            //Filter by angle?
            //var angle = creature.GetAngle(origin);
            // if (Math.Abs(angle) > splashAngle / 2.0f)
            //     continue;

            //Found splash object
            splashTargets.Add(creature);

            //Stop if you've found enough targets
            if (splashTargets.Count == maxTargets)
                break;
        }
        return splashTargets;
    }

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
