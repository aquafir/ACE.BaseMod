﻿namespace ACE.Shared.Helpers;
public static class SimulationExtensions
{
    //public static DamageEvent SimulateDamage(this Creature attacker, uint defenderWcid, WorldObject damageSource, MotionCommand? attackMotion = null, AttackHook attackHook = null)
    //{

    //}


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
        if(Evaded)
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