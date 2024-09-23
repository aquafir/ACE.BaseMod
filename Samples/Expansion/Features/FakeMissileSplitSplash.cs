namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeMissileSplitSplash))]
[HarmonyPatchCategory(nameof(Feature.FakeMissileSplitSplash))]
internal class FakeMissileSplitSplash
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.LaunchMissile), new Type[] { typeof(WorldObject), typeof(int), typeof(MotionStance), typeof(bool) })]
    public static void PreLaunchMissile(WorldObject target, int attackSequence, MotionStance stance, bool subsequent, ref Player __instance)
    {
        //Monster has its own version of LaunchMissile
        //Check weapon
        var weapon = __instance.GetEquippedMissileWeapon();
        if (weapon is null || target is not Creature creature) return;

        //Debugger.Break();
        TrySplashMissile(__instance, creature, weapon);
        TrySplitMissile(__instance, creature, weapon);
    }

    private static bool TrySplashMissile(Player player, Creature target, WorldObject weapon)
    {
        //Todo: allow without ammo / fix redundancy?
        var ammo = weapon.IsAmmoLauncher ? player.GetEquippedAmmo() : weapon;
        var projectileSpeed = player.GetProjectileSpeed();

        //Check if the missile splits
        var splashCount = weapon.GetProperty(FakeInt.ItemMissileSplashCount) ?? 0; //player.GetCachedFake(FakeInt.ItemMissileSplitCount);
        if (splashCount < 1)
            return false;

        //Gate by cooldown
        var time = player.GetProperty(FakeFloat.TimestampLastMissileSplash) ?? 0;
        var current = Time.GetUnixTime();
        var delta = current - time;

        var interval = weapon.GetProperty(FakeFloat.ItemMissileSplashCooldown) ?? 1;
        //player.SendMessage($"Splash Interval: {delta}");
        if (delta < interval)
            return false;

        //Radius, range, etc.
        var radius = weapon.GetProperty(FakeFloat.ItemMissileSplashRadius) ?? 4;
        //var targets = player.GetSplashTargets(target, (float)radius).Where(x => x is not Player).Take(splashCount).ToList();
        var targets = player.GetSplashTargets(target, TargetExclusionFilter.OnlyVisibleCreature, (float)radius).Take(splashCount).ToList();
        //player.SendMessage($"Radius: {radius} - {targets.Count}");

        if (targets.Count < 1)
            return false;

        //Success, set last timestamp
        player.SetProperty(FakeFloat.TimestampLastMissileSplash, Time.GetUnixTime());

        //Splash in a radius
        //player.SendMessage($"Splashing {targets.Count} times: {String.Join("\n", targets)}");
        //LaunchProjectile(launcher, ammo, target, origin, orientation, velocity);
        foreach (var t in targets)
        {
            var aimVelocity = player.GetAimVelocity(t, projectileSpeed);
            var aimLevel = Creature.GetAimLevel(aimVelocity);
            var localOrigin = player.GetProjectileSpawnOrigin(ammo.WeenieClassId, aimLevel);
            var velocity = player.CalculateProjectileVelocity(localOrigin, t, projectileSpeed, out Vector3 origin, out Quaternion orientation);

            player.LaunchProjectile(weapon, ammo, t, origin, orientation, velocity);
            player.UpdateAmmoAfterLaunch(ammo);

            player.LaunchProjectile(weapon, ammo, t, origin, orientation, velocity);
        }

        return true;
    }

    //Cleave-style
    private static bool TrySplitMissile(Player player, Creature target, WorldObject weapon)
    {
        //Todo: allow without ammo?
        var ammo = weapon.IsAmmoLauncher ? player.GetEquippedAmmo() : weapon;
        var projectileSpeed = player.GetProjectileSpeed();

        //Check if the missile splits
        var splitCount = weapon.GetProperty(FakeInt.ItemMissileSplitCount) ?? 0; //player.GetCachedFake(FakeInt.ItemMissileSplitCount);
        if (splitCount < 1)
            return false;

        //Gate by cooldown
        var time = player.GetProperty(FakeFloat.TimestampLastMissileSplit) ?? 0;
        var current = Time.GetUnixTime();
        var delta = current - time;

        //Scale an interval?  Default to .1
        var interval = weapon.GetProperty(FakeFloat.ItemMissileSplitCooldown) ?? 1;
        if (delta < interval)
            return false;

        //Radius, range, etc.
        var angle = weapon.GetProperty(FakeFloat.ItemMissileSplitAngle) ?? 45;
        var targets = player.GetSplitTargets(target, TargetExclusionFilter.OnlyVisibleCreature, player.GetMaxMissileRange(), (float)angle).Take(splitCount).ToList();

        if (targets.Count < 1)
            return false;

        //Success, set last timestamp
        player.SetProperty(FakeFloat.TimestampLastMissileSplit, Time.GetUnixTime());

        //Split the missile
        //player.SendMessage($"Splitting {targets.Count} times: {String.Join("\n", targets.Select(x => x.Name))}");
        foreach (var t in targets)
        {
            // target procs don't happen for cleaving
            var aimVelocity = player.GetAimVelocity(t, projectileSpeed);
            var aimLevel = Creature.GetAimLevel(aimVelocity);
            var localOrigin = player.GetProjectileSpawnOrigin(ammo.WeenieClassId, aimLevel);
            var velocity = player.CalculateProjectileVelocity(localOrigin, t, projectileSpeed, out Vector3 origin, out Quaternion orientation);

            player.LaunchProjectile(weapon, ammo, t, origin, orientation, velocity);
            player.UpdateAmmoAfterLaunch(ammo);
        }

        return true;
    }
}
