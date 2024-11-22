* Missile and Melee Actions/Messages
  * `ChangeCombatMode`
  * `HandleAttackDoneEvent`
  * `CancelAttack`



## Physical Damage

* Melee / missile damage both eventually use  `DamageEvent.CalculateDamage` to calculate/handle damage.  It provides rich information about what happened in a physical attack.
* Both use `NextRefillTime` to gate speed



### Melee Start

* Start with `TargetedMeleeAttack` GameAction
* `HandleActionTargetedMeleeAttack(ObjectGuid targetGuid, uint attackHeight, float powerLevel)`
  * Checks combat mode, suicide, busy, teleport, jumping, logout, invalid or dead target, `CanDamage` target
  * `OnAttackDone(WeenieError error)` may be called
    * Called at the very end of an attack sequence,
    * Sends action cancelled so the power / accuracy meter reset, and doesn't start refilling again
    * Sends `GameEventAttackDone` event
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

### Missile Start

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

      



### *Missile Collision*

* `Ammunition`, `WorldObject` and `Player` all have a `OnCollideObject(WorldObject target)` that detects initial collision
* Sends to `ProjectileCollisionHelper` used either for object or environment collisions
  * *`OnCollideEnvironment(WorldObject worldObject)`*
    * *For creatures, increments `MonsterProjectile_OnCollideEnvironment_Counter` which switches to melee on 3 misses*
  * `OnCollideObject(WorldObject worldObject, WorldObject target)`
    * Checks that the `ProjectileTarget` matches what was hit
    * If a player was the source damage is handled with 
      * `DamageTarget(Creature target, WorldObject damageSource)`
    * If a creature was the source
      * `DamageEvent` found with `CalculateDamage(Creature attacker, Creature defender, WorldObject damageSource)`
      * If damaging 
        * `TakeDamage(WorldObject source, DamageEvent damageEvent)`
        * Shield proficiency
        * Check `FightDirty(WorldObject target, WorldObject weapon)`
      * If evaded `OnEvade(WorldObject attacker, CombatType attackType)`
    * Attempts `TryProcEquippedItems(WorldObject attacker, Creature target, bool selfTarget, WorldObject weapon)`
  * Projectile is removed from the landblock and has physics disabled
    * `RemoveWorldObject(ObjectGuid objectId, bool adjacencyMove = false, bool fromPickup = false)`
    * PhysicsObj `set_active(bool active)`



### Damage

* `CalculateDamage` figures out damage.  It gets started/handled with
  * Melee - `DamageTarget(Creature target, WorldObject damageSource)`
  * Missile - `OnCollideObject(WorldObject worldObject, WorldObject target)`

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





## Magic

### Magic Start

* **Magic starts with a `CastTargetedSpell ` or `CastUntargetedSpell ` GameAction**

  * **A lot of overlap, with target validation if targeted.**
  * **Rolls skill, mana, sets up `MagicState`, and queues motions up to the time of casting**

* `CastTargetedSpell `

  * `HandleActionCastTargetedSpell(ObjectGuid targetGuid, uint spellId, WorldObject casterItem = null)`

  * Checks 
    * `LastCombatMode` and `CombatMode` for `CombatMode.Magic`
    * More physics if PK
    * Jumping, Busy, PKLogout, `MagicState.CanQueue`
    * Spell known - `VerifySpell(spellId, casterItem)` 
    * Null/teleporting target
  * Begins casting
    * `MagicState.OnCastStart`
      * Resets a lot of casting related things
    * `MagicState.SetWindupParams(targetGuid.Full, spellId, casterItem)`
      * Assigns `WindupParams`
    * `StartPos` set to current location
  * If not using `FastTick`
    * Queue `Rotate` to the target
    * If the target is null `MagicState.OnCastDone()`
    * Otherwise try to create spell, calling `OnCastDone()` on failure
      * `CreatePlayerSpell(target, targetCategory, spellId, casterItem)`
  * Otherwise `TurnTo_Magic(WorldObject target)` 

* `CastUntargetedSpell`

  * `HandleActionMagicCastUnTargetedSpell(uint spellId)`
    * Same as targeted spells, but no checks for target and no rotation
    * Ends with `CreatePlayerSpell(spellId)`
    * On fail `OnCastDone`

* `CreatePlayerSpell`

  * `ValidateSpell(spellId, casterItem != null)`
  * *`VerifySpellTarget(spell, target)` if targeted*
  * Find `caster`, whether player or item
  * Find if `isWeaponSpell` with `IsWeaponSpell(uint spellId, WorldObject casterItem)`
  * Find `magicSkill` with either
    * `GetCreatureSkill(MagicSchool skill)`
    * `ItemSpellcraft`

  * *`(WorldObject target, TargetCategory targetCategory, Spell spell, WorldObject casterItem, uint magicSkill)` if targeted*
  * `GetCastingPreCheckStatus(Spell spell, uint magicSkill, bool isWeaponSpell)`
    * Defaults to a fail - `CastingPreCheckStatus.CastFailed`
    * Weapon spells automatically succeed
    * If skill is within 50 of `Power` / difficulty, succeed on a successful roll with
      * `SkillCheck.GetMagicSkillChance(int skill, int difficulty)`

    * ***Switching between War / Void has an additional check***

  * `CalculateManaUsage(CastingPreCheckStatus castingPreCheckStatus, Spell spell, WorldObject target, WorldObject casterItem, out uint manaUsed)`
    * Check mana use, defaulting to 5 on a failed precheck
    * Mana used found with `CalculateManaUsage(Creature caster, Spell spell, WorldObject target = null)`
      * *Todo*.  Uses spell's `BaseMana` and differentiates between item/player cast

    * Fails on player/item mana insufficient
    * Grant proficiency on use

  * `DoSpellWords(Spell spell, bool isWeaponSpell)` broadcasts spell words
  * `DoWindupGestures(Spell spell, bool isWeaponSpell, ActionChain castChain)`
    * Queue windup motions
      * `SpellFlags.FastCast` skips
      * *FastTick - todo*
      * Each gesture in the spell formula's list queued - `spell.Formula.WindupGestures` 
        * `EnqueueMotionMagic(ActionChain actionChain, MotionCommand motionCommand, float speed = 1.0f)`
          * Finds animation length 
            * `Physics.Animation.MotionTable.GetAnimationLength(MotionTableId, MotionStance.Magic, motionCommand, speed)`
          * Players broadcast their motion
            * `EnqueueBroadcastMotion(Motion motion, float? maxRange = null, bool? applyPhysics = null)`
          * Add animation length to action chain as delay

  * `DoCastGesture(Spell spell, WorldObject casterItem, ActionChain castChain)`
    * `MagicState.CastGesture` comes from formula - `spell.Formula.CastGesture`
      * Unless it's a caster item with `UseUserAnimation`

    * Fails it not casting - `MagicState.IsCasting`
    * `MagicState.CastGestureStartTime` set to the present
    * *FastTick - todo*
    * `EnqueueMotionMagic(ActionChain actionChain, MotionCommand motionCommand, float speed = 1.0f)`

  * `SetCastParams(Spell spell, WorldObject casterItem, uint magicSkill, uint manaUsed, WorldObject target, Player.CastingPreCheckStatus status)`
    * Sets `CastSpellParams` of `MagicState`

  * `DoCastSpell(MagicState _state, bool checkAngle = true)`
    * *Added to action chain to finish up, if not FastTick*






#### DoCastSpell

* Targeted and untargeted both route through here on successful checks for skill / mana / target and motions

* `DoCastSpell(Spell spell, WorldObject casterItem, uint magicSkill, uint manaUsed, WorldObject target, CastingPreCheckStatus castingPreCheckStatus, bool checkAngle = true)`

* If targeted

  * Check range, valid target
  * If  a second `Rotate` is needed queue that and call back into `DoCastSpell` when done

* Check for `IsDead`

* `DoCastSpell_Inner(Spell spell, WorldObject casterItem, uint manaUsed, WorldObject target, CastingPreCheckStatus castingPreCheckStatus, bool finishCast = true)`

  * Consumes mana from player or item

  * Burn comps - `TryBurnComponents(Spell spell)`

  * Check for movement disrupting casting

    * Distance from `StartPos` 

  * Check pk - `CheckPKStatusVsTarget(target, spell)`

    * *Projectiles created even on fail?*

  * On success, `CreatePlayerSpell(WorldObject target, Spell spell, bool isWeaponSpell)`

    * If a fellowship spell, for each player in `GetFellowshipTargets()`
    * If harmful, `TryProcEquippedItems(WorldObject attacker, Creature target, bool selfTarget, WorldObject weapon)`

  * `FinishCast` unless coming from `FailCast`

    * Returns to Ready stance
    * *Todo - FastTick*
    * 1 second recoil/delay
    * `HandleCastQueue()` to process queue if enabled

    

#### CreatePlayerSpell

* Untargeted / Targeted both have a `CreatePlayerSpell` which after success/motions ends up here

* `CreatePlayerSpell(WorldObject target, Spell spell, bool isWeaponSpell)`

  * Routes based on spell type
    * `MagicSchool.ItemEnchantment`
      * `TryCastItemEnchantment_WithRedirects(Spell spell, WorldObject target, WorldObject itemCaster = null)`
    * Non-projectile checks
      * `OnAttackMonster(targetCreature)` 
        * *Target wakes up / retaliates*
      * Check resist
        * `TryResistSpell(WorldObject target, Spell spell, WorldObject itemCaster = null, bool projectileHit = false)`
      * Check immunity
    * `HandleCastSpell(Spell spell, WorldObject target, WorldObject itemCaster = null, WorldObject weapon = null, bool isWeaponSpell = false, bool fromProc = false, bool equip = false)`
      * Further routes spell based on information
    * Non-projectiles also:
      * `TryProcEquippedItems` and update PK timers if harmful
      * Gain proficiency - `Proficiency.OnSuccessUse`

  



### HandleCastSpell

* Creates a spell based on `MetaSpellType`
* Resolves `targetCreature`
* Plays caster/target effects unless there was an error
  * `DoSpellEffects(Spell spell, WorldObject caster, WorldObject target, bool projectileHit = false)`
* Routes on `spell.MetaSpellType`
  * Enchantments
    * `CreateEnchantment(WorldObject target, WorldObject caster, WorldObject weapon, Spell spell, bool equip = false, bool fromProc = false, bool isWeaponSpell = false)`
  * Boosts (typically heal/harm)
    * `HandleCastSpell_Boost(Spell spell, Creature targetCreature)`
  * Transfers (e.g., Stam to Mana)
    * `HandleCastSpell_Transfer(Spell spell, Creature targetCreature)`
  * Portal Link/Recall/Summon/Sending/FellowSending
    * `HandleCastSpell_PortalLink(Spell spell, WorldObject target)`
    * `HandleCastSpell_PortalRecall(Spell spell, Creature targetCreature)`
    * `HandleCastSpell_PortalSummon(Spell spell, Creature targetCreature, WorldObject itemCaster)`
    * `HandleCastSpell_FellowPortalSending(Spell spell, Creature targetCreature, WorldObject itemCaster)`
  * Dispels
    * `HandleCastSpell_Dispel(Spell spell, WorldObject target)`
  * Projectiles - Projectile / LifeProjectile / EnchantmentProjectile
    * `HandleCastSpell_Projectile(Spell spell, WorldObject target, WorldObject itemCaster, WorldObject weapon, bool isWeaponSpell, bool fromProc)`





#### Portal / Dispel

*Todo, not of interest*

#### Enchantment



#### Boost

* Gets an amount to try to boost
  * Random between `MinBoost` and `MaxBoost`
    * Or `spell.Boost`, if lower/higher
  * Multiply by resistance
    * `GetResistanceMod(ResistanceType resistance, WorldObject attacker = null, WorldObject weapon = null, float weaponResistanceMod = 1.0f)`
  * `UpdateVitalDelta` corresponding to the DamageType
  * Harms (health, negative boost)
    * Roll `target` Cloak procs
    * Add to `DamageHistory`
    * *Adds a short delay to sequence things?*
    * Checks for death with `HandleBoostTransferDeath(Creature caster, Creature target)`

#### Transfer

* Find source and destination of transfer
* Drains are when `TransferFlags` is `TargetSource | CasterDestination`
  * If draining the `boostMod` is the target's resistance to the damage type
  * `GetResistanceMod(ResistanceType resistance, WorldObject attacker = null, WorldObject weapon = null, float weaponResistanceMod = 1.0f)`
  * Health drains 
    * Add to `DamageHistory` (`OnHeal` for source) and check for `Cloak` procs
    * *Delay slightly for sequencing*
    * Checks for death with `HandleBoostTransferDeath(Creature caster, Creature target)`
* Find vital change in the source from the spell's `Proportion` and the `Current` vital
  * `srcVitalChange = (uint)Math.Round(transferSource.GetCreatureVital(spell.Source).Current * spell.Proportion * drainMod);`
* Find vital change in destination
  * Start with source change and factor in spell's `LossPercent` and, if a drain, the `boostMod`
    * `destVitalChange = (uint)Math.Round(srcVitalChange * (1.0f - spell.LossPercent) * boostMod)`
  * Find a max change
    * Min of the spell's `TransferCap` and the amount of the vital missing in the destination
  * If the max `destVitalChange` is capped, scale the source and destination changes to the ratio
* `UpdateVitalDelta` for the source/destination values, corresponding to the DamageType







*Example:*

* *Drain Health 1 - Drains  25% of the target's Health and gives 200% of it to the caster.*
* *`LossPercent` = -1, `Proportion` = 0.25, `TransferCap` = 60*
* *Missing 40 health, draining 500 health, no resistance*
  * *Source = 125*
  * *Dest = 250 (and boost?)*
  * *Cap is 40, lower than transfer cap*
  * *Ratio drained = 40/250 = 16%*
  * *Source = 125*.16 = 20*



#### Projectile

* `HandleCastSpell_Projectile(Spell spell, WorldObject target, WorldObject itemCaster, WorldObject weapon, bool isWeaponSpell, bool fromProc)`
* Possible types are: 
  * `Projectile`
  * `LifeProjectile`
  * `EnchantmentProjectile`
* 
* 





##### 

### SpellProjectile

#### Creation/Setup





####  Collision

* `SpellProjectile` either collides with object or environment
  * `OnCollideEnvironment`
  * `OnCollideObject(WorldObject target)`









## Pets

* `PetDevice`
  * `SummonCreature(Player player, uint wcid)`
    * `Pet.Init(Player player, PetDevice petDevice)`
      * Checks `HandleCurrentActivePet(Player player)`
        * Based on setting uses replace / retail
        * `HandleCurrentActivePet_Replace(Player player)` 
          * Destroys pet and replaces (if different wcid?)
        * `HandleCurrentActivePet_Retail(player)`
          * Fails if already summoned
        * True if player's `CurrentActivePet` is null
      * If null / false returned



### CurrentActivePet







## Skills

* `HandleActionTrainSkill(Skill skill, int creditsSpent)`
* 

* 









## Appraisal

* `GameEventType.IdentifyObjectResponse` builds an `AppraiseInfo`
  * Either empty or for a player
* `AppraiseInfo` handles calculating and sending all object appraisal info
* 









## Creatures

* `Attack` in `Monster_Combat` routes based on `CombatType`
  * `MeleeAttack`
  * `MissileAttack`
  * `MagicAttack`
