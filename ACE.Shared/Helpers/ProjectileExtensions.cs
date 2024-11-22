namespace ACE.Shared.Helpers;

public static class ProjectileExtensions
{
    //LaunchProjectile to shoot an object
    //public WorldObject LaunchProjectile(WorldObject weapon, WorldObject ammo, WorldObject target, Vector3 origin, Quaternion orientation, Vector3 velocity)

    /// <summary>
    /// Attempts to create a projectile for the attacker
    /// </summary>
    public static bool TryGetProjectile(this Creature attacker, out WorldObject projectile)
    {
        projectile = default;

        if (attacker is null || attacker.AttackTarget is not Creature defender)
            return false;

        //Get ammo
        //var weapon = attacker.GetEquippedMissileWeapon();
        //if (weapon == null)
        //    return false;

        //var ammo = weapon.IsAmmoLauncher ? attacker.GetEquippedAmmo() : weapon;
        //if (ammo == null)
        //    return false;

        //var launcher = attacker.GetEquippedMissileLauncher();
        if (!attacker.TryGetLauncherAndAmmo(out var launcher, out var ammo))
            return false;

        projectile = WorldObjectFactory.CreateNewWorldObject(ammo.WeenieClassId);

        return projectile is not null;
    }

    /// <summary>
    /// Attempts to get missile equipment
    /// </summary>
    public static bool TryGetLauncherAndAmmo(this Creature creature, out WorldObject launcher, out WorldObject ammo)
    {
        if (creature is null || creature.GetEquippedMissileWeapon() is not WorldObject weapon)
        {
            launcher = null;
            ammo = null;
            return false;
        }

        ammo = weapon.IsAmmoLauncher ? creature.GetEquippedAmmo() : weapon;
        if (ammo is null)
        {
            launcher = null;
            ammo = null;
            return false;
        }

        launcher = creature.GetEquippedMissileLauncher();

        return true;
    }

    /// <summary>
    /// Return the aiming components of a missile from a Creature
    /// </summary>
    public static bool TryGetProjectileAiming(this Creature attacker, WorldObject attackTarget, out Vector3 origin, out System.Numerics.Quaternion orientation, out Vector3 velocity)
    {
        if (!attacker.TryGetLauncherAndAmmo(out var launcher, out var ammo))
        {
            origin = default;
            orientation = default;
            velocity = default;
            return false;
        }

        var attackHeight = attacker.ChooseAttackHeight();
        var dist = attacker.GetDistanceToTarget();
        var projectileSpeed = attacker.GetProjectileSpeed();

        // get z-angle for aim motion
        var aimVelocity = attacker.GetAimVelocity(attackTarget, projectileSpeed);

        var aimLevel = Creature.GetAimLevel(aimVelocity);

        // calculate projectile spawn pos and velocity
        var localOrigin = attacker.GetProjectileSpawnOrigin(ammo.WeenieClassId, aimLevel);

        velocity = attacker.CalculateProjectileVelocity(localOrigin, attackTarget, projectileSpeed, out origin, out orientation);

        return true;
        //return attacker.TryGetProjectileAiming(out origin, out orientation, out velocity, attackHeight, projectileSpeed, localOrigin);
    }

    //public static bool TryGetProjectileAiming(out Vector3 origin, System.Numerics.Quaternion orientation, Vector3 velocity, AttackHeight AttackHeight, float projectileSpeed = 20f, Vector3 localOrigin = default)
    //{
    //    //LaunchMissile ->
    //    //CalculateProjectileVelocity(localOrigin, AttackTarget, projectileSpeed, out Vector3 origin, out Quaternion orientation);
    //    velocity = GetProjectileVelocity(localOrigin, origin,  attackTarget, projectileSpeed, out origin, out orientation);
    //}

    /// <summary>
    /// Calculates velocity to a location instead of a target
    /// </summary>
    public static Vector3 CalculateProjectileVelocity(this Position source, Position target, float zOffset, Vector3 localOrigin, float projectileSpeed, out Vector3 origin, out Quaternion rotation, bool useGravity = true, Vector3 targetVelocity = default)
    {
#if REALM
        var crossLandblock = source.InstancedLandblock != target.InstancedLandblock;
#else
        var crossLandblock = source.Landblock != target.Landblock;
#endif

        var startPos = crossLandblock ? source.ToGlobal(false) : source.Pos;
        var endPos = crossLandblock ? target.ToGlobal(false) : target.Pos;

        var dir = Vector3.Normalize(endPos - startPos);

        var angle = Math.Atan2(-dir.X, dir.Y);

        rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)angle);

        origin = source.Pos + Vector3.Transform(localOrigin, rotation);

        startPos += Vector3.Transform(localOrigin, rotation);
        endPos.Z += zOffset; //target.Height / GetAimHeight(target);

        //Get velocity based on location and optionally gravity/leading
        //Todo: decide on whether inferring whether leading by target velocity being supplied
        var leading = !targetVelocity.Equals(Vector3.Zero);
        var velocity = GetProjectileVelocity(targetVelocity, startPos, dir, endPos, projectileSpeed, out float time, useGravity, leading);

        return velocity;
    }

    /// <summary>
    /// Calculates the velocity to launch the projectile from origin to dest, optionally accounting for gravity or target velocity
    /// </summary>
    public static Vector3 GetProjectileVelocity(Vector3 targetVelocity, Vector3 origin, Vector3 dir, Vector3 dest, float speed, out float time, bool useGravity = true, bool leading = false)
    {
        time = 0.0f;
        Vector3 s0;
        float t0;

        var gravity = useGravity ? -PhysicsGlobals.Gravity : 0.00001f;
        //var targetVelocity = target.PhysicsObj.CachedVelocity;

        if (!targetVelocity.Equals(Vector3.Zero) && leading)
        {
            // use movement quartic solver
            if (!PropertyManager.GetBool("trajectory_alt_solver").Item)
            {
                var numSolutions = Trajectory.solve_ballistic_arc(origin, speed, dest, targetVelocity, gravity, out s0, out _, out time);

                if (numSolutions > 0)
                    return s0;
            }
            else
                return Trajectory2.CalculateTrajectory(origin, dest, targetVelocity, speed, useGravity);
        }

        // use stationary solver
        if (!PropertyManager.GetBool("trajectory_alt_solver").Item)
        {
            Trajectory.solve_ballistic_arc(origin, speed, dest, gravity, out s0, out _, out t0, out _);

            time = t0;
            return s0;
        }
        else
            return Trajectory2.CalculateTrajectory(origin, dest, Vector3.Zero, speed, useGravity);
    }
}
