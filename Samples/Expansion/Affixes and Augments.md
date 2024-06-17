

### Affixes and Augments

An `Augment` describes a reversible change to a `WorldObject`:

* `TryAugment(this WorldObject wo, AugmentType type, AugmentOperation op, int key, double value, bool recordAugment = true)`
* `AugmentType`
  * Determines what sort of property is changed
  * Has an int `key` to determine which specific property of a category is changed
  * Related to `StatType` of Recipes
  * Can be anything but a `PropertyString` amongst properties
    * *Todo: Skill / SAC / player-related properties*

* `Operation`
  * Determines how the property is changed
  * Related to `MutationEffectType`
  * Operations are:
    * Assign
    * Add
    * Multiply
    * BitSet
    * BitClear

* `Value`
  * Normalized value the target property is set to
* When applied it 



An `Affix` is a collection of `Augment`

















## Combat

* Missile and Melee Actions/Messages
  * `ChangeCombatMode`
  * `HandleAttackDoneEvent`
  * `CancelAttack`



### Melee

#### Start

* Start with `TargetedMeleeAttack` GameAction
* `HandleActionTargetedMeleeAttack(ObjectGuid targetGuid, uint attackHeight, float powerLevel)`
  * Checks combat mode, suicide, busy, teleport, jumping, logout, invalid or dead target, `CanDamage` target
  * `OnAttackDone(WeenieError error)` may be called
    * Called at the very end of an attack sequence,
    * Sends action cancelled so the power / accuracy meter reset, and doesn't start refilling again
  * Adds power to `AttackQueue`
  * Increments `AttackSequence`
  * Sets `MeleeTarget`, `AttackTarget`
  * Resets `PrevMotionCommand` and `DualWieldAlternate` for dual wield
  * Schedules attack if `NextRefillTime` in the future, otherwise issues attack
    * `HandleActionTargetedMeleeAttack_Inner(WorldObject target, int attackSequence)`
      * Checks `MeleeDistance` or `StickyDistance` and `IsMeleeVisible`
        * On fail starts `MoveTo` if charge enabled or `CreateMoveToChain` if not
      * If the angle exceeds `melee_max_angle` delays to `Rotate` before `Attack`
      * `Attack(WorldObject target, int attackSequence, bool subsequent = false)`
        * Checks similar things to `TargetedMeleeAttack`
        * `DoSwingMotion(WorldObject target, out List<(float time, AttackHook attackHook)> attackFrames)`
          * Performs the player melee swing animation
          * Returns the frames for the swings and the total seconds taken
          * Animation speed
            * `GetAnimSpeed` (* 1.2 if dual wield)
          * Animation length
            * Get swing animation used for animation length and frames - `GetSwingAnimation` 
            * Length - `MotionTable.GetAnimationLength(MotionTableId, CurrentMotionState.Stance, swingAnimation, animSpeed)`
            * Frames - `MotionTable.GetAttackFrames(MotionTableId, CurrentMotionState.Stance, swingAnimation)`
            * *Create `Motion` based on animation and broadcast to player*
              * `Motion(WorldObject wo, MotionCommand motion, float speed = 1.0f)`
              * `EnqueueBroadcastMotion(motion)`
        * Gets weapon - `GetEquippedMeleeWeapon`
        * Gets attack type - `GetWeaponAttackType(weapon)`
          * `W_AttackType` or `Undefined`
        * Gets strikes - `GetNumStrikes(attackType)`
        * Swing time - `animLength / numStrikes / 1.5f`
        * Takes stamina - `GetAttackStamina(GetPowerRange());`
        * Checks procs
          * `TryProcEquippedItems(WorldObject attacker, Creature target, bool selfTarget, WorldObject weapon)`
        * For each strike add a delay/action to the chain based on the length of the frame/animation
          * Damage - `DamageTarget(Creature target, WorldObject damageSource)`
          * Checks primary target for procs
          * Checks for cleaving - `GetCleaveTarget(Creature target, WorldObject weapon)`
        * *Adds remaining delay*
        * `Attacking` set to false
        * `PowerLevel` fetched from `AttackQueue`
        * `NextRefillTime` set to `PowerLevel` * mod (1, or .8 if dual wielding) in the future
        * If `AutoRepeatAttacks` enabled and a repeat attack would be valid, another `Attack` is scheduled
          * Otherwise `OnAttackDone`

### Missile

#### Start

* Start with `TargetedMissileAttack` GameAction

* `HandleActionTargetedMissileAttack(ObjectGuid targetGuid, uint attackHeight, float accuracyLevel)`

  * Checks combat mode, suicide, busy, teleport, jumping, logout, invalid or dead target, `CanDamage` target

  * Gets weapon - `GetEquippedMissileWeapon`

  * Gets ammo (if any) - `GetEquippedAmmo`

  * Adds accuracy to `AttackQueue`

  * Increments `AttackSequence`

  * Sets `MissileTarget`, `AttackTarget`

  * `Rotate ` and add the amount that exceeds `NextRefillTime` as a delay to the action chain

  * `LaunchMissile(WorldObject target, int attackSequence, MotionStance stance, bool subsequent = false)`

    * Repeat some checks and gets weapon/ammo/launcher

      * Check `TargetInRange`

    * Add a `Rotate` and delay if this is a repeat / `subsequent` attack and not `IsFacing(target)`

    * `Attacking` set to true

    * `GameEventCombatCommenceAttack` sent to client

    * Get information about the projectile

      * Speed - `GetProjectileSpeed`
      * Velocity - `GetAimVelocity(WorldObject target, float projectileSpeed)`
      * Aim level based on velocity - `GetAimLevel(aimVelocity)`
      * Height origin - `GetProjectileSpawnOrigin(wcid, motion)`
      * Velocity - `CalculateProjectileVelocity(Vector3 localOrigin, WorldObject target, float projectileSpeed, out Vector3 origin, out Quaternion rotation)`
        * A `Zero` velocity sends a `MissileOutOfRange` error and ends attack

    * Add action and delay to the action chain

      * `EnqueueMotionPersist(ActionChain actionChain, MotionCommand motionCommand, float speed = 1.0f, bool useStance = true, MotionCommand? prevCommand = null, bool castGesture = false, bool half = false)`

    * Add projectile launch to chain

      * `TryProcEquippedItems`
      * Broadcast `GetLaunchMissileSound(weapon)` 
        * `EnqueueBroadcast(new GameMessageSound(Guid, sound, 1.0f));`
      * Take stamina - `GetAttackStamina(GetAccuracyRange())`
      * `LaunchProjectile(WorldObject weapon, WorldObject ammo, WorldObject target, Vector3 origin, Quaternion orientation, Vector3 velocity)`
        * Launches a projectile from player to target
        * Checks valid velocity
        * Sets projectile physics - `SetProjectilePhysicsState(proj, target, velocity)`
        * `LandblockManager.AddObject(proj)` - creates projectile
        * Projectile destroyed if it failed to be added or if it isn't visible to the source `Creature`
          * `IsProjectileVisible(proj)`
        * Plays script 
          * `proj.EnqueueBroadcast(new GameMessageScript(proj.Guid, PlayScript.Launch, 0f))`
      * Update ammo - `UpdateAmmoAfterLaunch(WorldObject ammo)`

    * Add reload time

      * `GetAnimSpeed`
        * Based on Quickness and `GetWeaponSpeed`
        * Clamped to `MinAttackSpeed` and `MaxAttackSpeed`
      * Enqueue - `EnqueueMotionPersist(actionChain, stance, MotionCommand.Reload, animSpeed)`

    * Add link time?

      * `MotionTable.GetAnimationLength(MotionTableId, stance, MotionCommand.Reload, MotionCommand.Ready)`

    * If `AutoRepeatAttacks` and eligible to repeat add delay based on `AccuracyLevel` and enqueue another `LaunchMissile`

      * Otherwise finish with `OnAttackDone`

      



#### Collision

* `ProjectileCollisionHelper` used either for object or environment collisions
  * Outside of `WorldObject` hierarchy
  * `OnCollideEnvironment(WorldObject worldObject)`
  * 
* `ProjectileCollisionHelper.OnCollideObject(WorldObject worldObject, WorldObject target)`
  * 



### Melee/Missile Damage

* `DamageTarget(Creature target, WorldObject damageSource)`
  * `DamageEvent.CalculateDamage(this, target, damageSource)` 
    * Sets `AttackMotion` and `AttackHook` and `attacker` before passing to `DoCalculateDamage`
    * `float DoCalculateDamage(Creature attacker, Creature defender, WorldObject damageSource)`
      * Finds `Attacker`, `Defender`, `Weapon`, `DamageSource`, `AttackType`
      * `CombatType` based on presence of `ProjectileSource`
      * Checks for invulnerability
      * bool `Overpower` - `Creature.GetOverpower(attacker, defender)`
      * Check evasion
        * `Overpower` skips
        * Odds - `GetEvadeChance(attacker, defender)`
        * Rolls and sets `Evaded` / returns 0 on evasion
      * Get base damage, depends on if a player or creature
        * `GetBaseDamage(Player attacker)`
        * `GetBaseDamage(Creature attacker, MotionCommand motionCommand, AttackHook attackHook)`
      * Get modifiers
        * Power - `GetPowerMod(Weapon)`
        * Attribute - `GetAttributeMod(Weapon)`
        * Slayer - `WorldObject.GetWeaponCreatureSlayerModifier(Weapon, attacker, defender);`
      * Get ratings
        * DR - `Creature.GetPositiveRatingMod(attacker.GetDamageRating())`
        * Reckless - `Creature.GetRecklessnessMod(attacker, defender)`
        * Sneak - `attacker.GetSneakAttackMod(defender)`
        * Heritage - `attacker.GetHeritageBonus(Weapon) ? 1.05f : 1.0f`
        * *PK mod* - `Creature.GetPositiveRatingMod(attacker.GetPKDamageRating())`
        * `DamageRatingMod` is the additive combination
          * `Creature.AdditiveCombine(DamageRatingBaseMod, RecklessnessMod, SneakAttackMod, HeritageMod)`
      * Calculate `DamageBeforeMitigation`
        * `BaseDamage * AttributeMod * PowerMod * SlayerMod * DamageRatingMod`
      * Critical handling
        * Chance based on attack skill
          * Skill = `attacker.GetCreatureSkill(attacker.GetCurrentWeaponSkill())`
          * `WorldObject.GetWeaponCriticalChance(Weapon, attacker, attackSkill, defender)`
        * On crit players have a chance to defend
          * CDM = .05 or .25 if attacker not a `Player`
          * Chance = `AugmentationCriticalDefense` * (.05 or )
        * Otherwise factor in crit damage
          * Mod = `1.0f + WorldObject.GetWeaponCritDamageMod(Weapon, attacker, attackSkill, defender)`
          * `CriticalMultiplier` applied only to additional damage, while `CD` and `CDR`  applied to total `DamageBeforeMitigation`
            * CD = `1.0f + WorldObject.GetWeaponCritDamageMod(Weapon, attacker, attackSkill, defender)`
            * CDR = `Creature.GetPositiveRatingMod(attacker.GetCritDamageRating())`
          * Remove reckless from `DamageRatingMod`
            * `Creature.AdditiveCombine(DamageRatingBaseMod, CriticalDamageRatingMod, SneakAttackMod, HeritageMod)`
          * Damage = `BaseDamageMod.MaxDamage * AttributeMod * PowerMod * SlayerMod * DamageRatingMod * CriticalDamageMod`
      * Armor rending / cleaving takes the min between the two
        * `WorldObject.GetArmorRendingMod(attackSkill)`
        * `attacker.GetArmorCleavingMod(Weapon)`
        * `ignoreArmorMod = Math.Min(armorRendingMod, armorCleavingMod)`
      * Get armor/armor mod
        * `GetArmorMod` returns the percent of damage absorbed by layered armor + clothing
          * *Todo*
        * Players
          * Armor - `attacker.GetArmorLayers(playerDefender, BodyPart)`
          * AM - `attacker.GetArmorMod(playerDefender, DamageType, Armor, Weapon, ignoreArmorMod)`
        * Creatures
          * `GetQuadrant`
          * `GetBodyPart(quadrant)`
          * Armor - `CreaturePart.GetArmorLayers(PropertiesBodyPart.Key)`
          * AM - `GetArmorMod(DamageType damageType, List<WorldObject> armorLayers, Creature attacker, WorldObject weapon, float armorRendingMod = 1.0f)`
            * `GetEffectiveArmorVsType`
            * *Todo*
        * `IgnoreAllArmor` imbue sets to 1
      * Get resistances
        * WeaponResistanceMod - `WorldObject.GetWeaponResistanceModifier(Weapon, attacker, attackSkill, DamageType)`
        * ResistanceMod
          * Player - `playerDefender.GetResistanceMod(DamageType, Attacker, Weapon, WeaponResistanceMod)`
          * Creature
            * Max of 0 and `Creature.GetResistanceType(DamageType)` of `GetResistanceMod`
        * Damage resistance
          * Base - `defender.GetDamageResistRatingMod(CombatType)`
          * When crit
            * AdditiveCombine with `Creature.GetNegativeRatingMod(defender.GetCritDamageResistRating())`
          * *Todo Handle PK*
        * Shield - `defender.GetShieldMod(attacker, DamageType, Weapon)`
      * Finish damage calculations
        * Damage = DamageBeforeMitigation * ArmorMod * ShieldMod * ResistanceMod * DamageResistanceRatingMod
        * DamageMitigated = DamageBeforeMitigation - Damage;





### Magic

* Events
  * `CastTargetedSpell`
  * `CastUntargetedSpell`
* Life
  * Drain
  * Harm
  * Martyr
* War / Void
  * SpellProjectile
* Void
  * DoT







#### SpellProjectile



##### Collision

* `SpellProjectile` either collides with object or environment
  * `OnCollideEnvironment`
  * `OnCollideObject(WorldObject target)`
