using ACE.Adapter.GDLE.Models;
using ACE.DatLoader.Entity;
using ACE.DatLoader.FileTypes;
using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Physics.Animation;
using ACE.Server.WorldObjects;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using MotionTable = ACE.Server.Physics.Animation.MotionTable;
using Skill = ACE.Entity.Enum.Skill;

namespace ACE.Shared.Helpers;
public static class SimulationExtensions
{
    const uint PLAYER_MOTION_TABLE = 0x09000001;

    //public static DamageEvent SimulateDamage(this Creature attacker, uint defenderWcid, WorldObject damageSource, MotionCommand? attackMotion = null, AttackHook attackHook = null)
    //{

    //}

    /// <summary>
    /// Attempts to do simulate a creature's melee attack
    /// </summary>
    public static bool TrySimulateMeleeDamage(this Creature attacker, out List<SimulatedDamageEvent> damageEvents)
    {
        damageEvents = new();
        if (attacker is null || attacker.AttackTarget is not Creature defender)
            return false;

        var motionCommand = attacker.GetCombatManeuver();
        if (motionCommand is null)
            return false;

        attacker.SimulateSwingMotion(motionCommand.Value, out var animLength, out var attackFrames);
        var numStrikes = attackFrames.Count;

        var weapon = attacker.GetEquippedWeapon();

        // handle self-procs
        //TryProcEquippedItems(this, this, true, weapon);

        for (var i = 0; i < attackFrames.Count; i++)
            damageEvents.Add(attacker.SimulateDamage(defender, weapon, motionCommand.Value, attackFrames[i].attackHook));

        return true;
    }

    public static bool TrySimulateMissileCollisionDamage(this Creature attacker, out SimulatedDamageEvent damageEvent)
    {
        //ProjectileCollisionHelper.OnCollideObject
        damageEvent = default;

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

        if (!attacker.TryGetProjectile(out var projectile))
            return false;

        damageEvent = attacker.SimulateDamage(defender, projectile);

        return true;
    }

    /// <summary>
    /// Simulates a swing for a MotionCommand from a Creature
    /// </summary>
    public static void SimulateSwingMotion(this Creature attacker, MotionCommand motionCommand, out float animLength, out List<(float time, AttackHook attackHook)> attackFrames)
    {
        var baseSpeed = attacker.GetAnimSpeed();
        var animSpeedMod = attacker.IsDualWieldAttack ? 1.2f : 1.0f;     // dual wield swing animation 20% faster
        var animSpeed = baseSpeed * animSpeedMod;

        //var motionTableId = attacker.MotionTableId;
        //var stance = attacker.CurrentMotionState.Stance;

        SimulateSwingMotion(animSpeed, motionCommand, attacker.MotionTableId, attacker.CurrentMotionState.Stance, out animLength, out attackFrames);
    }

    /// <summary>
    /// Simulates a swing for a MotionCommand
    /// </summary>
    public static void SimulateSwingMotion(float animSpeed, MotionCommand motionCommand, uint motionTableId, MotionStance stance, out float animLength, out List<(float time, AttackHook attackHook)> attackFrames)
    {
        animLength = MotionTable.GetAnimationLength(motionTableId, stance, motionCommand, animSpeed);

        attackFrames = MotionTable.GetAttackFrames(motionTableId, stance, motionCommand);

        //Handle missing attack frames?
        if (attackFrames.Count == 0)
        {
            var attackFrameParams = new AttackFrameParams(motionTableId, stance, motionCommand);

            if (!Creature.missingAttackFrames.ContainsKey(attackFrameParams))
                Creature.missingAttackFrames.TryAdd(attackFrameParams, true);

            attackFrames = Creature.defaultAttackFrames;
        }
    }

    /// <summary>
    /// Creates a 
    /// </summary>
    public static SimulatedDamageEvent SimulateDamage(this Creature attacker, Creature defender, WorldObject damageSource, MotionCommand? attackMotion = null, AttackHook attackHook = null)
    {
        var damageEvent = new SimulatedDamageEvent();

        damageEvent.AttackMotion = attackMotion;
        damageEvent.AttackHook = attackHook;
        if (damageSource == null)
            damageSource = attacker;

        damageEvent.DoSimulateDamage(attacker, defender, damageSource);

        //damageEvent.HandleLogging(attacker, defender);

        return damageEvent;
    }

    /// <summary>
    /// Gets the time needed for an attack animation
    /// </summary>
    /// <param name="swingAnimation">Based on weapon, current stance, power bar, and attack height</param>
    /// <param name="speed">Based on weapon speed and player quickness</param>
    /// <param name="dualWield">Adds 1.2 multiplier to animation speed</param>
    /// <param name="stance"></param>
    /// <returns></returns>
    public static float GetSimulatedMeleeDelay(MotionCommand swingAnimation = MotionCommand.ThrustLow, MotionStance stance = MotionStance.SwordCombat, uint motionTableId = PLAYER_MOTION_TABLE, float speed = 1, bool dualWield = false)
    {
        //From GetAnimSpeed
        speed = Math.Clamp(speed, .5f, 2);

        if (dualWield)
            speed *= 1.2f;

        return Server.Physics.Animation.MotionTable.GetAnimationLength(motionTableId, stance, swingAnimation, speed);
    }

    public static float GetSimulatedMeleeDelay(this Creature attacker)
    {
        MotionCommand command = attacker is Player player ?
            player.GetSwingAnimation() :
            attacker.GetCombatManeuver().GetValueOrDefault();
        var stance = attacker.CurrentMotionState.Stance;

        return GetSimulatedMeleeDelay(command, stance, attacker.MotionTableId, 1, attacker.IsDualWieldAttack);
    }

    public static float SimulateSwingMotion(this Player player, WorldObject target, out List<(float time, ACE.DatLoader.Entity.AnimationHooks.AttackHook attackHook)> attackFrames)
    {
        // get the proper animation speed for this attack,
        // based on weapon speed and player quickness
        var baseSpeed = player.GetAnimSpeed();
        var animSpeedMod = player.IsDualWieldAttack ? 1.2f : 1.0f;     // dual wield swing animation 20% faster
        var animSpeed = baseSpeed * animSpeedMod;

        var swingAnimation = player.GetSwingAnimation();
        var animLength = Server.Physics.Animation.MotionTable.GetAnimationLength(player.MotionTableId, player.CurrentMotionState.Stance, swingAnimation, animSpeed);
        attackFrames = Server.Physics.Animation.MotionTable.GetAttackFrames(player.MotionTableId, player.CurrentMotionState.Stance, swingAnimation);

        // broadcast player swing animation to clients
        var motion = new Motion(player, swingAnimation, animSpeed);
        if (PropertyManager.GetBool("persist_movement").Item)
        {
            motion.Persist(player.CurrentMotionState);
        }
        motion.MotionState.TurnSpeed = 2.25f;
        //motion.MotionFlags |= MotionFlags.StickToObject;
        motion.TargetGuid = target.Guid;
        player.CurrentMotionState = motion;

        player.EnqueueBroadcastMotion(motion);

        if (player.FastTick)
            player.PhysicsObj.stick_to_object(target.Guid.Full);

        return animLength;
    }




    //Actualization takes simulated events and makes them real
    /// <summary>
    /// Handles multiple DamageEvents caused by one action like a melee attack which may only proc up to one time
    /// </summary>
    public static void ActualizeDamageEvents(this Creature attacker, Creature defender, List<SimulatedDamageEvent> damageEvents)
    {
        bool rollProc = true;

        //todo: think about this
        //Multiple DamageEvents are created by multi-hit melee attacks, but only the first should proc
        foreach (var dmgEvent in damageEvents)
            attacker.ActualizeDamageEvent(defender, dmgEvent, ref rollProc);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void ActualizeDamageEvent(this Creature attacker, Creature defender, SimulatedDamageEvent damageEvent, ref bool rollProc)
    {
        if (!damageEvent.HasDamage) //|| defender is null
            //target.OnEvade(this, CombatType.Melee);
            return;

        //Based on MeleeAttack

        //if (defender != null)
        if (defender is Player player)
        {
            // this is a player taking damage
            player.TakeDamage(attacker, damageEvent);

            //Skip proficiency?
            //if (damageEvent.ShieldMod != 1.0f)
            //{
            //    var shieldSkill = defender.GetCreatureSkill(Skill.Shield);
            //    Proficiency.OnSuccessUse(defender, shieldSkill, shieldSkill.Current); // ?
            //}

            // handle Dirty Fighting
            if (attacker.GetCreatureSkill(Skill.DirtyFighting).AdvancementClass >= SkillAdvancementClass.Trained)
                attacker.FightDirty(defender, damageEvent.Weapon);
        }
        else //if (combatPet != null || targetPet != null || Faction1Bits != null || target.Faction1Bits != null || PotentialFoe(target))
        {
            // combat pet inflicting or receiving damage?
            defender.TakeDamage(attacker, damageEvent.DamageType, damageEvent.Damage);

            //EmitSplatter(target, damageEvent.Damage);

            // handle Dirty Fighting
            if (attacker.GetCreatureSkill(Skill.DirtyFighting).AdvancementClass >= SkillAdvancementClass.Trained)
                attacker.FightDirty(defender, damageEvent.Weapon);
        }

        // handle target procs
        if (rollProc)
        {
            attacker.TryProcEquippedItems(attacker, defender, false, damageEvent.Weapon);
            rollProc = false;
        }
    }
}

public class SimulatedDamageEvent : DamageEvent
{
    /// <summary>
    /// Simulates damage for missile and melee
    /// </summary>
    public void DoSimulateDamage(Creature attacker, Creature defender, WorldObject damageSource)
    {
        var playerAttacker = attacker as Player;
        var playerDefender = defender as Player;

        var pkBattle = playerAttacker != null && playerDefender != null;

        SetupSimulation(attacker, defender, damageSource);

        if (IsInvulnerable(defender, playerDefender))
            return;

        CheckEvasion(attacker, defender);
        if (Evaded)
            return;

        SetBaseDamage(attacker, playerAttacker);

        CheckDamageType(attacker, damageSource);
        if (GeneralFailure) return;

        // get damage modifiers
        SetDamageModifiers(attacker, defender);
        // ratings
        SetRatings(attacker, defender, pkBattle);

        // damage before mitigation
        DamageBeforeMitigation = BaseDamage * AttributeMod * PowerMod * SlayerMod * DamageRatingMod;

        // critical hit?
        var attackSkill = attacker.GetCreatureSkill(attacker.GetCurrentWeaponSkill());
        CheckCritical(attacker, defender, playerAttacker, playerDefender, pkBattle, attackSkill);

        // armor rending and cleaving
        SetArmor(attacker, playerDefender, attackSkill);

        // get resistance modifiers
        SetResistance(attacker, defender, playerDefender, attackSkill);
        SetCritResistance(defender);
        SetPKResistance(defender, pkBattle);

        // get shield modifier
        ShieldMod = defender.GetShieldMod(attacker, DamageType, Weapon);

        // calculate final output damage
        Damage = DamageBeforeMitigation * ArmorMod * ShieldMod * ResistanceMod * DamageResistanceRatingMod;

        DamageMitigated = DamageBeforeMitigation - Damage;
    }

    private void CheckDamageType(Creature attacker, WorldObject damageSource)
    {
        if (DamageType == DamageType.Undef)
        {
            if ((attacker?.Guid.IsPlayer() ?? false) || (damageSource?.Guid.IsPlayer() ?? false))
            {
                //log.Error($"DamageEvent.DoCalculateDamage({attacker?.Name} ({attacker?.Guid}), {defender?.Name} ({defender?.Guid}), {damageSource?.Name} ({damageSource?.Guid})) - DamageType == DamageType.Undef");
                GeneralFailure = true;
            }
        }
    }

    private void SetPKResistance(Creature defender, bool pkBattle)
    {
        if (pkBattle)
        {
            PkDamageResistanceMod = Creature.GetNegativeRatingMod(defender.GetPKDamageResistRating());

            DamageResistanceRatingMod = Creature.AdditiveCombine(DamageResistanceRatingMod, PkDamageResistanceMod);
        }
    }

    private void SetCritResistance(Creature defender)
    {
        if (IsCritical)
        {
            CriticalDamageResistanceRatingMod = Creature.GetNegativeRatingMod(defender.GetCritDamageResistRating());

            DamageResistanceRatingMod = Creature.AdditiveCombine(DamageResistanceRatingBaseMod, CriticalDamageResistanceRatingMod);
        }
    }

    private void SetResistance(Creature attacker, Creature defender, Player? playerDefender, CreatureSkill attackSkill)
    {
        WeaponResistanceMod = WorldObject.GetWeaponResistanceModifier(Weapon, attacker, attackSkill, DamageType);

        if (playerDefender != null)
        {
            ResistanceMod = playerDefender.GetResistanceMod(DamageType, Attacker, Weapon, WeaponResistanceMod);
        }
        else
        {
            var resistanceType = Creature.GetResistanceType(DamageType);
            ResistanceMod = (float)Math.Max(0.0f, defender.GetResistanceMod(resistanceType, Attacker, Weapon, WeaponResistanceMod));
        }

        // damage resistance rating
        DamageResistanceRatingMod = DamageResistanceRatingBaseMod = defender.GetDamageResistRatingMod(CombatType);
    }

    private void SetArmor(Creature attacker, Player? playerDefender, CreatureSkill attackSkill)
    {
        var armorRendingMod = 1.0f;
        if (Weapon != null && Weapon.HasImbuedEffect(ImbuedEffectType.ArmorRending))
            armorRendingMod = WorldObject.GetArmorRendingMod(attackSkill);

        var armorCleavingMod = attacker.GetArmorCleavingMod(Weapon);

        var ignoreArmorMod = Math.Min(armorRendingMod, armorCleavingMod);

        // get body part / armor pieces / armor modifier
        if (playerDefender != null)
        {
            // select random body part @ current attack height
            GetBodyPart(AttackHeight);

            // get player armor pieces
            Armor = attacker.GetArmorLayers(playerDefender, BodyPart);

            // get armor modifiers
            ArmorMod = attacker.GetArmorMod(playerDefender, DamageType, Armor, Weapon, ignoreArmorMod);
        }
        else
        {
            //Randomize quadrant
            Quadrant = Enum.GetValues<Quadrant>().Random();//GetQuadrant(Defender, Attacker, AttackHeight, DamageSource);

            // select random body part @ current attack height
            GetBodyPart(Defender, Quadrant);
            if (Evaded)
                return;

            Armor = CreaturePart.GetArmorLayers(PropertiesBodyPart.Key);

            // get target armor
            ArmorMod = CreaturePart.GetArmorMod(DamageType, Armor, Attacker, Weapon, ignoreArmorMod);
        }

        if (Weapon != null && Weapon.HasImbuedEffect(ImbuedEffectType.IgnoreAllArmor))
            ArmorMod = 1.0f;
    }

    private void CheckCritical(Creature attacker, Creature defender, Player? playerAttacker, Player? playerDefender, bool pkBattle, CreatureSkill attackSkill)
    {
        CriticalChance = WorldObject.GetWeaponCriticalChance(Weapon, attacker, attackSkill, defender);

        // https://asheron.fandom.com/wiki/Announcements_-_2002/08_-_Atonement
        // It should be noted that any time a character is logging off, PK or not, all physical attacks against them become automatically critical.
        // (Note that spells do not share this behavior.) We hope this will stress the need to log off in a safe place.

        if (playerDefender != null && (playerDefender.IsLoggingOut || playerDefender.PKLogout))
            CriticalChance = 1.0f;

        if (CriticalChance > ThreadSafeRandom.Next(0.0f, 1.0f))
        {
            if (playerDefender != null && playerDefender.AugmentationCriticalDefense > 0)
            {
                var criticalDefenseMod = playerAttacker != null ? 0.05f : 0.25f;
                var criticalDefenseChance = playerDefender.AugmentationCriticalDefense * criticalDefenseMod;

                if (criticalDefenseChance > ThreadSafeRandom.Next(0.0f, 1.0f))
                    CriticalDefended = true;
            }

            if (!CriticalDefended)
            {
                IsCritical = true;

                // verify: CriticalMultiplier only applied to the additional crit damage,
                // whereas CD/CDR applied to the total damage (base damage + additional crit damage)
                CriticalDamageMod = 1.0f + WorldObject.GetWeaponCritDamageMod(Weapon, attacker, attackSkill, defender);

                CriticalDamageRatingMod = Creature.GetPositiveRatingMod(attacker.GetCritDamageRating());

                // recklessness excluded from crits
                RecklessnessMod = 1.0f;
                DamageRatingMod = Creature.AdditiveCombine(DamageRatingBaseMod, CriticalDamageRatingMod, SneakAttackMod, HeritageMod);

                if (pkBattle)
                    DamageRatingMod = Creature.AdditiveCombine(DamageRatingMod, PkDamageMod);

                DamageBeforeMitigation = BaseDamageMod.MaxDamage * AttributeMod * PowerMod * SlayerMod * DamageRatingMod * CriticalDamageMod;
            }
        }
    }

    private void SetDamageModifiers(Creature attacker, Creature defender)
    {
        PowerMod = attacker.GetPowerMod(Weapon);
        AttributeMod = attacker.GetAttributeMod(Weapon);
        SlayerMod = WorldObject.GetWeaponCreatureSlayerModifier(Weapon, attacker, defender);
    }

    private void SetRatings(Creature attacker, Creature defender, bool pkBattle, float sneakMod = 1f)
    {
        DamageRatingBaseMod = Creature.GetPositiveRatingMod(attacker.GetDamageRating());
        RecklessnessMod = Creature.GetRecklessnessMod(attacker, defender);

        //Todo: decide how to simulate sneak mod without requiring WO
        SneakAttackMod = sneakMod;  //attacker.GetSneakAttackMod(defender);
        HeritageMod = attacker.GetHeritageBonus(Weapon) ? 1.05f : 1.0f;

        DamageRatingMod = Creature.AdditiveCombine(DamageRatingBaseMod, RecklessnessMod, SneakAttackMod, HeritageMod);

        if (pkBattle)
        {
            PkDamageMod = Creature.GetPositiveRatingMod(attacker.GetPKDamageRating());
            DamageRatingMod = Creature.AdditiveCombine(DamageRatingMod, PkDamageMod);
        }
    }

    private void SetBaseDamage(Creature attacker, Player? playerAttacker)
    {
        // get base damage
        if (playerAttacker != null)
            GetBaseDamage(playerAttacker);
        else
            GetBaseDamage(attacker, AttackMotion ?? MotionCommand.Invalid, AttackHook);
    }

    private void CheckEvasion(Creature attacker, Creature defender)
    {
        // overpower
        if (attacker.Overpower != null)
            Overpower = Creature.GetOverpower(attacker, defender);

        // evasion chance
        if (!Overpower)
        {
            EvasionChance = GetEvadeChance(attacker, defender);
            if (EvasionChance > ThreadSafeRandom.Next(0.0f, 1.0f))
            {
                Evaded = true;
            }
        }
    }

    private void SetupSimulation(Creature attacker, Creature defender, WorldObject damageSource)
    {
        //Default damage
        Damage = 0;

        Attacker = attacker;
        Defender = defender;

        CombatType = damageSource.ProjectileSource == null ? CombatType.Melee : CombatType.Missile;

        DamageSource = damageSource;

        Weapon = damageSource.ProjectileSource == null ? attacker.GetEquippedMeleeWeapon() : (damageSource.ProjectileLauncher ?? damageSource.ProjectileAmmo);

        AttackType = attacker.AttackType;
        AttackHeight = attacker.AttackHeight ?? AttackHeight.Medium;
    }

    private bool IsInvulnerable(Creature defender, Player? playerDefender)
    {
        if (playerDefender != null && playerDefender.UnderLifestoneProtection)
        {
            LifestoneProtection = true;
            playerDefender.HandleLifestoneProtection();
            return true;
        }

        if (defender.Invincible)
            return true;

        return false;
    }
}
