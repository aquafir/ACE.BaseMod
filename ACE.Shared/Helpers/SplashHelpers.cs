using ACE.Adapter.GDLE.Models;
using ACE.Server.Physics;
using ACE.Server.WorldObjects;
using System.Collections.Frozen;
using System.Linq;
using System.Linq.Expressions;

namespace ACE.Shared.Helpers;

public static class SplashHelper
{
    #region Objects Sorted by Distance
    /// <summary>
    /// Gets a list of PhysicsObj visible to a Player sorted by distance using their visible objects
    /// </summary>
    public static List<PhysicsObj> GetKnownCreaturesByDistance(this Player origin) => origin.GetKnownCreaturesByDistance(origin);

    //Todo: implement on per-landblock level if reference approach doesn't work.  Maybe look at spatial hashing
    /// <summary>
    /// Gets a list of PhysicsObj sorted by distance from a WorldObject using objects visible to a reference Player
    /// </summary>
    /// <param name="reference">A player with similar vision to the origin</param>
    /// <param name="origin">Center of distance comparison</param>
    /// <returns></returns>
    public static List<PhysicsObj> GetKnownCreaturesByDistance(this Player reference, WorldObject origin)
    {
        if (reference is null || reference.PhysicsObj is null)
            return new();

        var visible = reference.PhysicsObj.ObjMaint?.GetVisibleObjectsValuesWhere(o =>
            o?.WeenieObj?.WorldObject is not null &&
            o.WeenieObj.WorldObject.WeenieType == WeenieType.Creature &&    //Restrict to creature weenies here for speed?
            o.WeenieObj.WorldObject != null);

        visible.Sort((x, y) => origin.Location.SquaredDistanceTo(x.WeenieObj.WorldObject.Location)
                    .CompareTo(origin.Location.SquaredDistanceTo(y.WeenieObj.WorldObject.Location)));

        return visible;
    }
    #endregion

    #region Splash
    public static IEnumerable<Creature> GetSplashTargets(this Player reference, WorldObject origin, TargetExclusionFilter filter, float maxRange = 5.0f)
    {
        if (reference is null) return new List<Creature>();

        //Try to get a cached predicate and build one if not created (should only happen with novel combinations of filters
        if (!predicates.TryGetValue(filter, out var predicate))
            predicate = filter.GetExclusionFilterPredicate();

        return reference.GetSplashTargets(origin, predicate, maxRange);
    }

    /// <summary>
    /// Finds nearest neighbors to an origin matching a predicate and using a players known PhysicsObjs
    /// </summary>
    public static IEnumerable<Creature> GetSplashTargets(this Player reference, WorldObject origin, Func<WorldObject, Creature, bool> isFiltered, float maxRange = 5.0f)
    {
        if (origin is null || reference is null || isFiltered is null)
        {
            ModManager.Log($"Failed to get splash targets.", ModManager.LogLevel.Warn);
            yield break;
        }

        var visible = reference.GetKnownCreaturesByDistance(origin);

        foreach (var obj in visible)
        {
            //Only splash creatures?
            var creature = obj.WeenieObj.WorldObject as Creature;

            if (creature is null)
                continue;

            //Todo: think about this -- max range halts evaluation instead of skipping, but might be nice to open 
            //No more objects in range
            if (origin.GetCylinderDistance(creature) > maxRange)
                yield break;

            //Check filter conditions
            if (isFiltered(origin, creature))
                continue;

            //Found splash object
            yield return creature;
        }
    }
    #endregion

    #region Split
    public static IEnumerable<Creature> GetSplitTargets(this Player player, WorldObject target, TargetExclusionFilter filter, float maxRange = 5.0f, float splitAngle = 360)
    {
        if (player is null) return new List<Creature>();

        //Try to get a cached predicate and build one if not created (should only happen with novel combinations of filters
        if (!predicates.TryGetValue(filter, out var predicate))
            predicate = filter.GetExclusionFilterPredicate();

        return player.GetSplitTargets(target, predicate, maxRange, splitAngle);
    }

    /// <summary>
    /// Finds nearest neighbors to an origin matching a predicate and falling within an angle of the player and a target
    /// </summary>
    public static IEnumerable<Creature> GetSplitTargets(this Player origin, WorldObject target, Func<WorldObject, Creature, bool> isFiltered, float maxRange = 5.0f, float splitAngle = 360)
    {
        if (origin is null || target is null || isFiltered is null)
        {
            ModManager.Log($"Failed to get split targets.", ModManager.LogLevel.Warn);
            yield break;
        }

        var visible = origin.GetKnownCreaturesByDistance(origin);

        foreach (var obj in visible)
        {
            //Only splash creatures?
            var creature = obj.WeenieObj.WorldObject as Creature;

            if (creature is null)
                continue;

            //No more objects in range
            if (origin.GetCylinderDistance(creature) > maxRange)
                yield break;

            //Check filter conditions
            if (isFiltered(origin, creature))
                continue;

            //Filter by angle?
            var angle = origin.GetAngle(creature);
            if (Math.Abs(angle) > splitAngle / 2.0f)
                continue;

            //Found splash object
            yield return creature;
        }
    }
    #endregion

    #region Filters/Expression/Funcs
    /// <summary>
    /// Predicates created for each exclusion filter flag or composite flag
    /// </summary>
    static FrozenDictionary<TargetExclusionFilter, Func<WorldObject, Creature, bool>> predicates =
        Enum.GetValues<TargetExclusionFilter>().ToFrozenDictionary(x => x, x => x.GetExclusionFilterPredicate());

    /// <summary>
    /// Creates a filter that excludes targets from an origin's search for nearest neighbors
    /// </summary>
    public static Func<WorldObject, Creature, bool> GetExclusionFilterPredicate(this TargetExclusionFilter filter)
    {
        //Get filtering that is always required (null checks)
        Expression<Func<WorldObject, Creature, bool>> combinedExpression = TargetExclusionFilter.None.GetPredicateExpression();

        //Get the simple flags representing a single criterion being checked
        var flags = filter.GetIndividualFlags();

        //Combine them
        foreach (var flag in flags)
            combinedExpression = CombineWithOr(combinedExpression, flag.GetPredicateExpression());

        //Return the combined function
        return combinedExpression.Compile();
    }

    /// <summary>
    /// Returns an expression for the atomic criterion of the filter
    /// </summary>
    public static Expression<Func<WorldObject, Creature, bool>> GetPredicateExpression(this TargetExclusionFilter filter)
    {
        Func<WorldObject, Creature, bool> predicate = filter switch
        {
            TargetExclusionFilter.None => (origin, target) => target is null,
            TargetExclusionFilter.Creature => (origin, target) => target is Creature && target is not Player,
            TargetExclusionFilter.Player => (origin, target) => target is Player,
            TargetExclusionFilter.Original => (origin, target) => origin.PhysicsObj.ID == target.PhysicsObj.ID,
            TargetExclusionFilter.Dead => (origin, target) => target.IsDead,
            TargetExclusionFilter.Teleporting => (origin, target) => target.Teleporting,
            TargetExclusionFilter.NPC => (origin, target) => (!target.Attackable && target.TargetingTactic == TargetingTactic.None),
            TargetExclusionFilter.LineOfSight => (origin, target) => origin.IsDirectVisible(target),
            TargetExclusionFilter.OutOfSight => (origin, target) => !origin.IsDirectVisible(target),
            _ => (origin, target) => target is null,
        };

        return (origin, target) => predicate(origin, target);
    }

    /// <summary>
    /// Returns an expression combining two filtering functions
    /// </summary>
    public static Expression<Func<WorldObject, Creature, bool>> CombineWithOr(
    Expression<Func<WorldObject, Creature, bool>> expr1,
    Expression<Func<WorldObject, Creature, bool>> expr2)
    {
        var parameter1 = Expression.Parameter(typeof(WorldObject), "origin");
        var parameter2 = Expression.Parameter(typeof(Creature), "target");

        //var body = Expression.AndAlso(
        var body = Expression.OrElse(
            Expression.Invoke(expr1, parameter1, parameter2),
            Expression.Invoke(expr2, parameter1, parameter2)
        );

        return Expression.Lambda<Func<WorldObject, Creature, bool>>(body, parameter1, parameter2);
    }
    #endregion

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

        var visible = reference.GetKnownCreaturesByDistance(origin);

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


/// <summary>
/// Exclusion filters for common WorldObject target selection
/// </summary>
[Flags]
public enum TargetExclusionFilter
{
    /// <summary>
    /// No filtering
    /// </summary>
    None = 0x0000,
    /// <summary>
    /// Ignores Creatures
    /// </summary>
    Creature = 0x0001,
    /// <summary>
    /// Ignores players
    /// </summary>
    Player = 0x0002,
    /// <summary>
    /// Check for the origin being the same as the target
    /// </summary>
    Original = 0x0004,
    /// <summary>
    /// Excludes dead creatures
    /// </summary>
    Dead = 0x0008,
    /// <summary>
    /// Excludes teleporting creatures
    /// </summary>
    Teleporting = 0x0010,
    /// <summary>
    /// Excludes NPCs
    /// </summary>
    NPC = 0x0020,
    /// <summary>
    /// Ignores WorldObjects in line-of-sight
    /// </summary>
    LineOfSight = 0x1000,
    /// <summary>
    /// Ignores WorldObjects out of line-of-sight
    /// </summary>
    OutOfSight = 0x2000,


    /// <summary>
    /// Ignores the things typically not of interest
    /// </summary>
    Standard = Original | Dead | Teleporting | NPC,
    /// <summary>
    /// Excludes all but visible creatures
    /// </summary>
    OnlyCreature = Standard | Player,
    /// <summary>
    /// Excludes all but visible creatures
    /// </summary>
    OnlyVisibleCreature = OnlyCreature | OutOfSight,
    /// <summary>
    /// Excludes all but players
    /// </summary>
    OnlyPlayer = Standard | Creature,
    /// <summary>
    /// Excludes all but visible players
    /// </summary>
    OnlyVisiblePlayer = OnlyPlayer | OutOfSight,
}
