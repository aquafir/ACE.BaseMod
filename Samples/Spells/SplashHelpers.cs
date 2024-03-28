using ACE.Server.Physics;

namespace Spells
{
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
    }
}
