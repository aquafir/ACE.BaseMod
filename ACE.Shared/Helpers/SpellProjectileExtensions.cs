//namespace ACE.Shared.Helpers;

//public static class SpellProjectileExtensions
//{
//    public static ProjectileSpellType Nova = (ProjectileSpellType)9;

//    /// <summary>
//    /// Creates but doesn't launch spell projectiles
//    /// </summary>
//    public static List<SpellProjectile> PositionSpellProjectiles(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, uint lifeProjectileDamage)
//    {
//        List<SpellProjectile> __result;
//        //var spellType = SpellProjectile.GetProjectileSpellType(spell.Id);
//        var origins = source.CalculateSpellProjectileOrigins(spellType, spell, target, weapon, isWeaponSpell, fromProc, lifeProjectileDamage); //source.CalculateProjectileOrigins(spell, spellType, target);
//        var velocity = source.CalculateProjectileVelocity(spell, target, spellType, origins[0]);

//        //if (spell.Name.Contains("Acid"))
//        //    spellType = Nova;

//        //if (spellType == Nova && source is Player player)// && fromProc)
//        //{
//        //    var playerTarget = player?.selectedTarget?.TryGetWorldObject() ?? player;

//        //    //Debugger.Break();
//        //    //Add some more?
//        //    var pert = 4f;
//        //    for (var i = 0; i < 10; i++)
//        //    {
//        //        //origins.Add(origins[0] + new Vector3((float)ThreadSafeRandom.Next(0, pert), (float)ThreadSafeRandom.Next(0, pert), (float)ThreadSafeRandom.Next(0, pert)));
//        //        //origins.Add(origins[0] + new Vector3(i % 2 == 0 ? i : -i,0,0));
//        //        origins.Add(origins[0] + new Vector3(0, 0.0f, 0));
//        //    }
//        //}

//        __result = source.CreateSpellProjectiles(spell, spellType, target, weapon, isWeaponSpell, fromProc, origins, velocity, lifeProjectileDamage);
//        return __result;
//    }

//    /// <summary>
//    /// Launches a list of SpellProjectiles created with CreateSpellProjectiles and returns the ones successfully created
//    /// </summary>
//    public static List<SpellProjectile> LaunchSpellProjectiles(this WorldObject source, List<SpellProjectile> projectiles, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)
//    {
//        List<SpellProjectile> created = new();
//        foreach (var sp in projectiles)
//        {
//            if (!LandblockManager.AddObject(sp))
//            {
//                sp.Destroy();

//                continue;
//            }

//            if (sp.WorldEntryCollision)
//                continue;

//            sp.EnqueueBroadcast(new GameMessageScript(sp.Guid, PlayScript.Launch, sp.GetProjectileScriptIntensity(spellType)));

//            if (!source.IsProjectileVisible(sp))
//            {
//                sp.OnCollideEnvironment();
//                continue;
//            }
//            created.Add(sp);
//        }

//        return created;
//    }

//    /// <summary>
//    /// Creates spell projectiles without launching
//    /// </summary>
//    public static List<SpellProjectile> CreateSpellProjectiles(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)
//    {
//        var useGravity = spellType == ProjectileSpellType.Arc;

//        var strikeSpell = target != null && spellType == ProjectileSpellType.Strike;

//        var spellProjectiles = new List<SpellProjectile>();

//        //Reroute origins
//        var casterLoc = source.GetSpellCasterPosition(spell, spellType, target, weapon, isWeaponSpell, fromProc, origins, velocity, lifeProjectileDamage);
//        var targetLoc = source.GetSpellTargetPosition(spell, spellType, target, weapon, isWeaponSpell, fromProc, origins, velocity, lifeProjectileDamage);

//        for (var i = 0; i < origins.Count; i++)
//        {
//            var origin = origins[i];

//            var sp = WorldObjectFactory.CreateNewWorldObject(spell.Wcid) as SpellProjectile;

//            if (sp == null)
//            {
//                break;
//            }

//            sp.Setup(spell, spellType);
//#if REALM
//            //TODO: fix
//            //source.SetSpellRotation(i, spell, spellType, target, velocity, strikeSpell, casterLoc.Position, targetLoc.Position, origin, sp);
//#else
//            source.SetSpellRotation(i, spell, spellType, target, velocity, strikeSpell, casterLoc, targetLoc, origin, sp);
//#endif

//            // set orientation
//            var dir = Vector3.Normalize(sp.Velocity);
//            sp.PhysicsObj.Position.Frame.set_vector_heading(dir);

//            sp.Location.Position.Rotation = sp.PhysicsObj.Position.Frame.Orientation;

//            sp.ProjectileSource = source;
//            sp.FromProc = fromProc;

//            // side projectiles always untargeted?
//            if (i == 0)
//                sp.ProjectileTarget = target;

//            sp.ProjectileLauncher = weapon;
//            sp.IsWeaponSpell = isWeaponSpell;

//            sp.SetProjectilePhysicsState(sp.ProjectileTarget, useGravity);
//            sp.SpawnPos = new(sp.Location);

//            sp.LifeProjectileDamage = lifeProjectileDamage;

//            //Moved to Launch
//            //if (!LandblockManager.AddObject(sp))
//            //{
//            //    sp.Destroy();
//            //    continue;
//            //}

//            //if (sp.WorldEntryCollision)
//            //    continue;

//            //sp.EnqueueBroadcast(new GameMessageScript(sp.Guid, PlayScript.Launch, sp.GetProjectileScriptIntensity(spellType)));

//            //if (!source.IsProjectileVisible(sp))
//            //{
//            //    sp.OnCollideEnvironment();
//            //    continue;
//            //}

//            spellProjectiles.Add(sp);
//        }

//        return spellProjectiles;
//    }

//    /// <summary>
//    /// Creates starting locations for each projectile for a custom spell
//    /// </summary>
//    public static List<Vector3> CalculateSpellProjectileOrigins(this WorldObject source, ProjectileSpellType spellType, Spell spell, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, uint lifeProjectileDamage)
//    {
//        var origins = new List<Vector3>();

//        var radius = source.GetProjectileRadius(spell);
//        //Console.WriteLine($"Radius: {radius}");

//        var vRadius = Vector3.One * radius;

//        var baseOffset = spell.CreateOffset;

//        var radsum = source.PhysicsObj.GetPhysicsRadius() * 2.0f + radius * 2.0f;

//        var heightOffset = source.CalculatePreOffset(spell, spellType, target);

//        if (target != null)
//        {
//            var cylDist = source.GetCylinderDistance(target);
//            //Console.WriteLine($"CylDist: {cylDist}");
//            if (cylDist < 0.6f)
//                radsum = source.PhysicsObj.GetPhysicsRadius() + radius;
//        }

//        if (spell.SpreadAngle == 360)
//            radsum *= 0.6f;

//        baseOffset.Y += radsum;

//        baseOffset += heightOffset;

//        var anglePerStep = WorldObject.GetSpreadAnglePerStep(spell);

//        // TODO: normalize data
//        var dims = new Vector3(spell._spell.DimsOriginX ?? spell.NumProjectiles, spell._spell.DimsOriginY ?? 1, spell._spell.DimsOriginZ ?? 1);

//        var i = 0;
//        for (var z = 0; z < dims.Z; z++)
//        {
//            for (var y = 0; y < dims.Y; y++)
//            {
//                var oddRow = (int)Math.Min(dims.X, spell.NumProjectiles - i) % 2 == 1;

//                for (var x = 0; x < dims.X; x++)
//                {
//                    if (i >= spell.NumProjectiles)
//                        break;

//                    var curOffset = baseOffset;

//                    if (spell.Peturbation != Vector3.Zero)
//                    {
//                        var rng = new Vector3((float)ThreadSafeRandom.Next(-1.0f, 1.0f), (float)ThreadSafeRandom.Next(-1.0f, 1.0f), (float)ThreadSafeRandom.Next(-1.0f, 1.0f));

//                        curOffset += rng * spell.Peturbation * spell.Padding;
//                    }

//                    if (!oddRow && spell.SpreadAngle == 0)
//                        curOffset.X += spell.Padding.X * 0.5f + radius;

//                    var xFactor = spell.SpreadAngle == 0 ? oddRow ? (float)Math.Ceiling(x * 0.5f) : (float)Math.Floor(x * 0.5f) : 0;

//                    var origin = curOffset + (vRadius * 2.0f + spell.Padding) * new Vector3(xFactor, y, z);

//                    if (spell.SpreadAngle == 0)
//                    {
//                        if (x % 2 == (oddRow ? 1 : 0))
//                            origin.X *= -1.0f;
//                    }
//                    else
//                    {
//                        // get the rotation matrix to apply to x
//                        var numSteps = (x + 1) / 2;
//                        if (x % 2 == 0)
//                            numSteps *= -1;

//                        //Console.WriteLine($"NumSteps: {numSteps}");

//                        var curAngle = anglePerStep * numSteps;
//                        var rads = curAngle.ToRadians();

//                        var rot = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, rads);
//                        origin = Vector3.Transform(origin, rot);
//                    }

//                    origins.Add(origin);
//                    i++;
//                }

//                if (i >= spell.NumProjectiles)
//                    break;
//            }

//            if (i >= spell.NumProjectiles)
//                break;
//        }

//        /*foreach (var origin in origins)
//            Console.WriteLine(origin);*/

//        return origins;
//    }

//    /// <summary>
//    /// *****UNUSED*****
//    /// Decomposed method to calculates the damage for a spell projectile
//    /// Used by war magic, void magic, and life magic projectiles
//    /// </summary>
//    public static float? CalculateDamage(this SpellProjectile projectile, Spell Spell, WorldObject source, Creature target, bool criticalHit, bool critDefended, bool overpower)
//    {
//        var sourcePlayer = source as Player;
//        var targetPlayer = target as Player;

//        if (source == null || !target.IsAlive || targetPlayer != null && targetPlayer.Invincible)
//            return null;

//        // check lifestone protection
//        if (targetPlayer != null && targetPlayer.UnderLifestoneProtection)
//        {
//            if (sourcePlayer != null)
//                sourcePlayer.Session.Network.EnqueueSend(new GameMessageSystemChat($"The Lifestone's magic protects {targetPlayer.Name} from the attack!", ChatMessageType.Magic));

//            targetPlayer.HandleLifestoneProtection();
//            return null;
//        }

//        var critDamageBonus = 0.0f;
//        var weaponCritDamageMod = 1.0f;
//        var weaponResistanceMod = 1.0f;
//        var resistanceMod = 1.0f;

//        // life magic
//        var lifeMagicDamage = 0.0f;

//        // war/void magic
//        var baseDamage = 0;
//        var skillBonus = 0.0f;
//        var finalDamage = 0.0f;

//        var resistanceType = Creature.GetResistanceType(Spell.DamageType);

//        var sourceCreature = source as Creature;
//        if (sourceCreature?.Overpower != null)
//            overpower = Creature.GetOverpower(sourceCreature, target);

//        var weapon = projectile.ProjectileLauncher;

//        var resistSource = projectile.IsWeaponSpell ? weapon : source;

//        var resisted = source.TryResistSpell(target, Spell, resistSource, true);
//        if (resisted && !overpower)
//            return null;

//        CreatureSkill attackSkill = null;
//        if (sourceCreature != null)
//            attackSkill = sourceCreature.GetCreatureSkill(Spell.School);

//        // critical hit
//        var criticalChance = SpellProjectile.GetWeaponMagicCritFrequency(weapon, sourceCreature, attackSkill, target);

//        if (ThreadSafeRandom.Next(0.0f, 1.0f) < criticalChance)
//        {
//            if (targetPlayer != null && targetPlayer.AugmentationCriticalDefense > 0)
//            {
//                var criticalDefenseMod = sourcePlayer != null ? 0.05f : 0.25f;
//                var criticalDefenseChance = targetPlayer.AugmentationCriticalDefense * criticalDefenseMod;

//                if (criticalDefenseChance > ThreadSafeRandom.Next(0.0f, 1.0f))
//                    critDefended = true;
//            }

//            if (!critDefended)
//                criticalHit = true;
//        }

//        var absorbMod = projectile.GetAbsorbMod(target);

//        bool isPVP = sourcePlayer != null && targetPlayer != null;

//        //http://acpedia.org/wiki/Announcements_-_2014/01_-_Forces_of_Nature - Aegis is 72% effective in PvP
//        if (isPVP && (target.CombatMode == CombatMode.Melee || target.CombatMode == CombatMode.Missile))
//        {
//            absorbMod = 1 - absorbMod;
//            absorbMod *= 0.72f;
//            absorbMod = 1 - absorbMod;
//        }

//        if (isPVP && Spell.IsHarmful)
//            Player.UpdatePKTimers(sourcePlayer, targetPlayer);

//        var elementalDamageMod = SpellProjectile.GetCasterElementalDamageModifier(weapon, sourceCreature, target, Spell.DamageType);

//        // Possible 2x + damage bonus for the slayer property
//        var slayerMod = SpellProjectile.GetWeaponCreatureSlayerModifier(weapon, sourceCreature, target);

//        // life magic projectiles: ie., martyr's hecatomb
//        if (Spell.MetaSpellType == ACE.Entity.Enum.SpellType.LifeProjectile)
//        {
//            lifeMagicDamage = projectile.LifeProjectileDamage * Spell.DamageRatio;

//            // could life magic projectiles crit?
//            // if so, did they use the same 1.5x formula as war magic, instead of 2.0x?
//            if (criticalHit)
//            {
//                // verify: CriticalMultiplier only applied to the additional crit damage,
//                // whereas CD/CDR applied to the total damage (base damage + additional crit damage)
//                weaponCritDamageMod = SpellProjectile.GetWeaponCritDamageMod(weapon, sourceCreature, attackSkill, target);

//                critDamageBonus = lifeMagicDamage * 0.5f * weaponCritDamageMod;
//            }

//            weaponResistanceMod = SpellProjectile.GetWeaponResistanceModifier(weapon, sourceCreature, attackSkill, Spell.DamageType);

//            // if attacker/weapon has IgnoreMagicResist directly, do not transfer to spell projectile
//            // only pass if SpellProjectile has it directly, such as 2637 - Invoking Aun Tanua

//            resistanceMod = (float)Math.Max(0.0f, target.GetResistanceMod(resistanceType, projectile, null, weaponResistanceMod));

//            finalDamage = (lifeMagicDamage + critDamageBonus) * elementalDamageMod * slayerMod * resistanceMod * absorbMod;
//        }
//        // war/void magic projectiles
//        else
//        {
//            if (criticalHit)
//            {
//                // Original:
//                // http://acpedia.org/wiki/Announcements_-_2002/08_-_Atonement#Letter_to_the_Players

//                // Critical Strikes: In addition to the skill-based damage bonus, each projectile spell has a 2% chance of causing a critical hit on the target and doing increased damage.
//                // A magical critical hit is similar in some respects to melee critical hits (although the damage calculation is handled differently).
//                // While a melee critical hit automatically does twice the maximum damage of the weapon, a magical critical hit will do an additional half the minimum damage of the spell.
//                // For instance, a magical critical hit from a level 7 spell, which does 110-180 points of damage, would add an additional 55 points of damage to the spell.

//                // Later updated for PvE only:

//                // http://acpedia.org/wiki/Announcements_-_2004/07_-_Treaties_in_Stone#Letter_to_the_Players

//                // Currently when a War Magic spell scores a critical hit, it adds a multiple of the base damage of the spell to a normal damage roll.
//                // Starting in July, War Magic critical hits will instead add a multiple of the maximum damage of the spell.
//                // No more crits that do less damage than non-crits!

//                if (isPVP) // PvP: 50% of the MIN damage added to normal damage roll
//                    critDamageBonus = Spell.MinDamage * 0.5f;
//                else   // PvE: 50% of the MAX damage added to normal damage roll
//                    critDamageBonus = Spell.MaxDamage * 0.5f;

//                // verify: CriticalMultiplier only applied to the additional crit damage,
//                // whereas CD/CDR applied to the total damage (base damage + additional crit damage)
//                weaponCritDamageMod = SpellProjectile.GetWeaponCritDamageMod(weapon, sourceCreature, attackSkill, target);

//                critDamageBonus *= weaponCritDamageMod;
//            }

//            /* War Magic skill-based damage bonus
//             * http://acpedia.org/wiki/Announcements_-_2002/08_-_Atonement#Letter_to_the_Players
//             */
//            if (sourcePlayer != null)
//            {
//                var magicSkill = sourcePlayer.GetCreatureSkill(Spell.School).Current;

//                if (magicSkill > Spell.Power)
//                {
//                    var percentageBonus = (magicSkill - Spell.Power) / 1000.0f;

//                    skillBonus = Spell.MinDamage * percentageBonus;
//                }
//            }
//            baseDamage = ThreadSafeRandom.Next(Spell.MinDamage, Spell.MaxDamage);

//            weaponResistanceMod = SpellProjectile.GetWeaponResistanceModifier(weapon, sourceCreature, attackSkill, Spell.DamageType);

//            // if attacker/weapon has IgnoreMagicResist directly, do not transfer to spell projectile
//            // only pass if SpellProjectile has it directly, such as 2637 - Invoking Aun Tanua

//            resistanceMod = (float)Math.Max(0.0f, target.GetResistanceMod(resistanceType, projectile, null, weaponResistanceMod));

//            if (sourcePlayer != null && targetPlayer != null && Spell.DamageType == DamageType.Nether)
//            {
//                // for direct damage from void spells in pvp,
//                // apply void_pvp_modifier *on top of* the player's natural resistance to nether

//                // this supposedly brings the direct damage from void spells in pvp closer to retail
//                resistanceMod *= (float)PropertyManager.GetDouble("void_pvp_modifier").Item;
//            }

//            finalDamage = baseDamage + critDamageBonus + skillBonus;

//            finalDamage *= elementalDamageMod * slayerMod * resistanceMod * absorbMod;
//        }

//        // show debug info
//        //if (sourceCreature != null && sourceCreature.DebugDamage.HasFlag(Creature.DebugDamageType.Attacker))
//        //{
//        //    projectile.ShowInfo(sourceCreature, Spell, attackSkill, criticalChance, criticalHit, critDefended, overpower, weaponCritDamageMod, skillBonus, baseDamage, critDamageBonus, elementalDamageMod, slayerMod, weaponResistanceMod, resistanceMod, absorbMod, projectile.LifeProjectileDamage, lifeMagicDamage, finalDamage);
//        //}
//        //if (target.DebugDamage.HasFlag(Creature.DebugDamageType.Defender))
//        //{
//        //    projectile.ShowInfo(target, Spell, attackSkill, criticalChance, criticalHit, critDefended, overpower, weaponCritDamageMod, skillBonus, baseDamage, critDamageBonus, elementalDamageMod, slayerMod, weaponResistanceMod, resistanceMod, absorbMod, projectile.LifeProjectileDamage, lifeMagicDamage, finalDamage);
//        //}
//        return finalDamage;
//    }

//    private static Position? GetSpellCasterPosition(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)

//    {
//        //Ring spells center on the selected target of the player if available
//        if (spellType == ProjectileSpellType.Ring)
//        {
//            if (source is Player player)
//            {
//                if (player.selectedTarget is not null && player.selectedTarget?.TryGetWorldObject() is Creature creature)
//                    return creature?.PhysicsObj.Position.ACEPosition(source.Location.Instance);

//                //Just to show use a position relative to player?
//                return player?.PhysicsObj.Position.ACEPosition(source.Location.Instance).InFrontOf(5);
//            }
//        }

//        return source?.PhysicsObj.Position.ACEPosition(source.Location.Instance);
//    }

//    private static Position? GetSpellTargetPosition(this WorldObject source, Spell spell, ProjectileSpellType spellType, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage)
//    {
//        return target?.PhysicsObj.Position.ACEPosition(source.Location.Instance);
//    }

//    private static Quaternion SetSpellRotation(this WorldObject source, int i, Spell spell, ProjectileSpellType spellType, WorldObject target, Vector3 velocity, bool strikeSpell, Position? casterLoc, Position? targetLoc, Vector3 origin, SpellProjectile? sp)
//    {
//        var rotate = casterLoc.Rotation;
//        if (target != null)
//        {
//            var qDir = source.PhysicsObj.Position.GetOffset(target.PhysicsObj.Position);
//            rotate = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.Atan2(-qDir.X, qDir.Y));
//        }

//        //sp.Location = strikeSpell ? new Position(targetLoc) : new Position(casterLoc);
//        //sp.Location.Pos += Vector3.Transform(origin, strikeSpell ? rotate * WorldObject.OneEighty : rotate);

//        //sp.PhysicsObj.Velocity = velocity;

//        //Special handling
//        //if (spellType == CustomSpellProjectiles.Nova)
//        //{
//        //    var n = Vector3.Normalize(origin);
//        //    var angle = i * Math.PI / 5; //36 degrees
//        //    if (source is Player player)
//        //        player.SendMessage($"{i} @ {angle / Math.PI * 180} degrees");
//        //    var q = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)angle);
//        //    q = Quaternion.CreateFromYawPitchRoll((float)angle, (float)angle, 0);
//        //    sp.PhysicsObj.Velocity = Vector3.Transform(velocity, q);
//        //}

//        if (spell.SpreadAngle > 0)
//        {
//            var n = Vector3.Normalize(origin);
//            var angle = Math.Atan2(-n.X, n.Y);
//            var q = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)angle);
//            sp.PhysicsObj.Velocity = Vector3.Transform(velocity, q);
//        }
//        return rotate;
//    }
//}
