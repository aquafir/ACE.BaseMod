namespace ACE.Shared.Helpers;

/// <summary>
/// 
/// </summary>
public static class SpellProjectileExtensions
{
    public static ProjectileSpellType Nova = (ProjectileSpellType)9;

    /// <summary>
    /// Creates but doesn't launch spell projectiles
    /// </summary>
    public static List<SpellProjectile> PositionSpellProjectiles(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, uint lifeProjectileDamage)
    {
        List<SpellProjectile> __result;
        //var spellType = SpellProjectile.GetProjectileSpellType(spell.Id);
        var origins = source.CalculateSpellProjectileOrigins(spellType, spell, target, weapon, isWeaponSpell, fromProc, lifeProjectileDamage); //source.CalculateProjectileOrigins(spell, spellType, target);
        var velocity = source.CalculateProjectileVelocity(spell, target, spellType, origins[0]);

        //if (spell.Name.Contains("Acid"))
        //    spellType = Nova;

        //if (spellType == Nova && source is Player player)// && fromProc)
        //{
        //    var playerTarget = player?.selectedTarget?.TryGetWorldObject() ?? player;

        //    //Debugger.Break();
        //    //Add some more?
        //    var pert = 4f;
        //    for (var i = 0; i < 10; i++)
        //    {
        //        //origins.Add(origins[0] + new Vector3((float)ThreadSafeRandom.Next(0, pert), (float)ThreadSafeRandom.Next(0, pert), (float)ThreadSafeRandom.Next(0, pert)));
        //        //origins.Add(origins[0] + new Vector3(i % 2 == 0 ? i : -i,0,0));
        //        origins.Add(origins[0] + new Vector3(0, 0.0f, 0));
        //    }
        //}

        __result = source.CreateSpellProjectiles(spell, spellType, target, weapon, isWeaponSpell, fromProc, origins, velocity, lifeProjectileDamage);
        return __result;
    }


    /// <summary>
    /// Launches a list of SpellProjectiles created with CreateSpellProjectiles and returns the ones successfully created
    /// </summary>
    public static List<SpellProjectile> LaunchSpellProjectiles(this WorldObject source, List<SpellProjectile> projectiles, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)
    {
        List<SpellProjectile> created = new();
        foreach (var sp in projectiles)
        {
            if (!LandblockManager.AddObject(sp))
            {
                sp.Destroy();

                continue;
            }

            if (sp.WorldEntryCollision)
                continue;

            sp.EnqueueBroadcast(new GameMessageScript(sp.Guid, PlayScript.Launch, sp.GetProjectileScriptIntensity(spellType)));

            if (!source.IsProjectileVisible(sp))
            {
                sp.OnCollideEnvironment();
                continue;
            }
            created.Add(sp);
        }

        return created;
    }

    /// <summary>
    /// Creates spell projectiles without launching
    /// </summary>
    public static List<SpellProjectile> CreateSpellProjectiles(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)
    {
        var useGravity = spellType == ProjectileSpellType.Arc;

        var strikeSpell = target != null && spellType == ProjectileSpellType.Strike;

        var spellProjectiles = new List<SpellProjectile>();

        //Reroute origins
        var casterLoc = source.GetSpellCasterPosition(spell, spellType, target, weapon, isWeaponSpell, fromProc, origins, velocity, lifeProjectileDamage);
        var targetLoc = source.GetSpellTargetPosition(spell, spellType, target, weapon, isWeaponSpell, fromProc, origins, velocity, lifeProjectileDamage);

        for (var i = 0; i < origins.Count; i++)
        {
            var origin = origins[i];

            var sp = WorldObjectFactory.CreateNewWorldObject(spell.Wcid) as SpellProjectile;

            if (sp == null)
            {
                break;
            }

            sp.Setup(spell, spellType);
#if REALM
            source.SetSpellRotation(i, spell, spellType, target, velocity, strikeSpell, casterLoc.Position, targetLoc.Position, origin, sp);
#else
            source.SetSpellRotation(i, spell, spellType, target, velocity, strikeSpell, casterLoc, targetLoc, origin, sp);
#endif

            // set orientation
            var dir = Vector3.Normalize(sp.Velocity);
            sp.PhysicsObj.Position.Frame.set_vector_heading(dir);


            sp.Location.Position.Rotation = sp.PhysicsObj.Position.Frame.Orientation;

            sp.ProjectileSource = source;
            sp.FromProc = fromProc;

            // side projectiles always untargeted?
            if (i == 0)
                sp.ProjectileTarget = target;

            sp.ProjectileLauncher = weapon;
            sp.IsWeaponSpell = isWeaponSpell;

            sp.SetProjectilePhysicsState(sp.ProjectileTarget, useGravity);
            sp.SpawnPos = new(sp.Location);

            sp.LifeProjectileDamage = lifeProjectileDamage;

            //Moved to Launch
            //if (!LandblockManager.AddObject(sp))
            //{
            //    sp.Destroy();
            //    continue;
            //}

            //if (sp.WorldEntryCollision)
            //    continue;

            //sp.EnqueueBroadcast(new GameMessageScript(sp.Guid, PlayScript.Launch, sp.GetProjectileScriptIntensity(spellType)));

            //if (!source.IsProjectileVisible(sp))
            //{
            //    sp.OnCollideEnvironment();
            //    continue;
            //}

            spellProjectiles.Add(sp);
        }

        return spellProjectiles;
    }

    /// <summary>
    /// Creates starting locations for each projectile for a custom spell
    /// </summary>
    public static List<Vector3> CalculateSpellProjectileOrigins(this WorldObject source, ProjectileSpellType spellType, Spell spell, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, uint lifeProjectileDamage)
    {
        var origins = new List<Vector3>();

        var radius = source.GetProjectileRadius(spell);
        //Console.WriteLine($"Radius: {radius}");

        var vRadius = Vector3.One * radius;

        var baseOffset = spell.CreateOffset;

        var radsum = source.PhysicsObj.GetPhysicsRadius() * 2.0f + radius * 2.0f;

        var heightOffset = source.CalculatePreOffset(spell, spellType, target);

        if (target != null)
        {
            var cylDist = source.GetCylinderDistance(target);
            //Console.WriteLine($"CylDist: {cylDist}");
            if (cylDist < 0.6f)
                radsum = source.PhysicsObj.GetPhysicsRadius() + radius;
        }

        if (spell.SpreadAngle == 360)
            radsum *= 0.6f;

        baseOffset.Y += radsum;

        baseOffset += heightOffset;

        var anglePerStep = WorldObject.GetSpreadAnglePerStep(spell);

        // TODO: normalize data
        var dims = new Vector3(spell._spell.DimsOriginX ?? spell.NumProjectiles, spell._spell.DimsOriginY ?? 1, spell._spell.DimsOriginZ ?? 1);

        var i = 0;
        for (var z = 0; z < dims.Z; z++)
        {
            for (var y = 0; y < dims.Y; y++)
            {
                var oddRow = (int)Math.Min(dims.X, spell.NumProjectiles - i) % 2 == 1;

                for (var x = 0; x < dims.X; x++)
                {
                    if (i >= spell.NumProjectiles)
                        break;

                    var curOffset = baseOffset;

                    if (spell.Peturbation != Vector3.Zero)
                    {
                        var rng = new Vector3((float)ThreadSafeRandom.Next(-1.0f, 1.0f), (float)ThreadSafeRandom.Next(-1.0f, 1.0f), (float)ThreadSafeRandom.Next(-1.0f, 1.0f));

                        curOffset += rng * spell.Peturbation * spell.Padding;
                    }

                    if (!oddRow && spell.SpreadAngle == 0)
                        curOffset.X += spell.Padding.X * 0.5f + radius;

                    var xFactor = spell.SpreadAngle == 0 ? oddRow ? (float)Math.Ceiling(x * 0.5f) : (float)Math.Floor(x * 0.5f) : 0;

                    var origin = curOffset + (vRadius * 2.0f + spell.Padding) * new Vector3(xFactor, y, z);

                    if (spell.SpreadAngle == 0)
                    {
                        if (x % 2 == (oddRow ? 1 : 0))
                            origin.X *= -1.0f;
                    }
                    else
                    {
                        // get the rotation matrix to apply to x
                        var numSteps = (x + 1) / 2;
                        if (x % 2 == 0)
                            numSteps *= -1;

                        //Console.WriteLine($"NumSteps: {numSteps}");

                        var curAngle = anglePerStep * numSteps;
                        var rads = curAngle.ToRadians();

                        var rot = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, rads);
                        origin = Vector3.Transform(origin, rot);
                    }

                    origins.Add(origin);
                    i++;
                }

                if (i >= spell.NumProjectiles)
                    break;
            }

            if (i >= spell.NumProjectiles)
                break;
        }

        /*foreach (var origin in origins)
            Console.WriteLine(origin);*/

        return origins;
    }

#if REALM
    private static InstancedPosition? GetSpellCasterPosition(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)
#else
    private static Position? GetSpellCasterPosition(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)
#endif
    {
        //Ring spells center on the selected target of the player if available
        if (spellType == ProjectileSpellType.Ring)
        {
            if (source is Player player)
            {
                if (player.selectedTarget is not null && player.selectedTarget?.TryGetWorldObject() is Creature creature)
                    return creature?.PhysicsObj.Position.ACEPosition(source.Location.Instance);

                //Just to show use a position relative to player?
                return player?.PhysicsObj.Position.ACEPosition(source.Location.Instance).InFrontOf(5);
            }
        }

        return source?.PhysicsObj.Position.ACEPosition(source.Location.Instance);
    }
#if REALM
    private static InstancedPosition? GetSpellTargetPosition(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)
#else
    private static Position? GetSpellTargetPosition(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)
#endif
    {
        return target?.PhysicsObj.Position.ACEPosition(source.Location.Instance);
    }

    private static Quaternion SetSpellRotation(this WorldObject source, int i, Spell spell, ProjectileSpellType spellType, WorldObject target, Vector3 velocity, bool strikeSpell, Position? casterLoc, Position? targetLoc, Vector3 origin, SpellProjectile? sp)
    {
        var rotate = casterLoc.Rotation;
        if (target != null)
        {
            var qDir = source.PhysicsObj.Position.GetOffset(target.PhysicsObj.Position);
            rotate = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.Atan2(-qDir.X, qDir.Y));
        }

        //sp.Location = strikeSpell ? new Position(targetLoc) : new Position(casterLoc);
        //sp.Location.Pos += Vector3.Transform(origin, strikeSpell ? rotate * WorldObject.OneEighty : rotate);

        //sp.PhysicsObj.Velocity = velocity;

        //Special handling
        //if (spellType == CustomSpellProjectiles.Nova)
        //{
        //    var n = Vector3.Normalize(origin);
        //    var angle = i * Math.PI / 5; //36 degrees
        //    if (source is Player player)
        //        player.SendMessage($"{i} @ {angle / Math.PI * 180} degrees");
        //    var q = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)angle);
        //    q = Quaternion.CreateFromYawPitchRoll((float)angle, (float)angle, 0);
        //    sp.PhysicsObj.Velocity = Vector3.Transform(velocity, q);
        //}

        if (spell.SpreadAngle > 0)
        {
            var n = Vector3.Normalize(origin);
            var angle = Math.Atan2(-n.X, n.Y);
            var q = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)angle);
            sp.PhysicsObj.Velocity = Vector3.Transform(velocity, q);
        }
        return rotate;
    }
}