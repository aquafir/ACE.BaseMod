## Expansion

This mod changes the loot generated objects returned by `LootGenerationFactory.CreateAndMutateWcid`



### **Overview**

* **Features** are patches that enable additional functionality, such as a "reduced weight" property for a container
  * Some mutations may require a feature for the item they produce to do what it should.
* **Mutators** change loot after its generated.
  * The `Event` is a bitfield of `MutatorEvent` which decides when a mutator mutates.  
    All will require a `WorldObject` but some will have more granularity or other information available.
    * `Loot` catches all loot but misses corpse/location/etc.
    * `Corpse` modifies items in a creature corpse after `Creature.GenerateTreasure`
    * `Generator` runs after `GeneratorProfile.TreasureGenerator`
    * `EnterWorld` runs before `WorldObject.EnterWorld`
    * Could expose things like `RollMundane` / `CreateDinnerware` / etc.
  * Different sets of valid targets are available.  Succeeds if missing.  
    Available targets are:
    * `WeenieTypeTargets` is a set of `WeenieType` the WorldObject must be
    * `TreasureTargets` is a set of `TreasureItemType_Orig` 
  
  * `Odds` is the 0-1 chance of being applied to an item of a tier
    * If missing it will always succeed.
    * Tier 0 is a default if missing tier information
  * There are default sets but you can make your own using names from the relevant enums.
  * You can make multiple Mutators if you want to have different odds for different targets.
  
* Settings for Mutators/Features are currently just dumped in `Settings.json`.  



### Groups

* `CreatureType`

* `EquipmentSet` 

* `SpellId`

* `Augment` - attempt to change a WorldObject

  



### Dependencies

Some features or mutators depend on a utility `Feature`:

* ~~`CorpseInfo` adds additional information to a `Corpse`~~
  * ~~`FakeInt.CorpseLivingWCID`~~
  * ~~`FakeDID.CorpseLandblockId`~~
  * ~~`FakeBool.CorpseSpawnedDungeon`~~
* `FakePropertyCache` stores a cached version for a players `FakeProperty` bonus and updates on change.  Not strictly required but gives a significant performance boost.



### Mutations

#### ProcOnHit

* Cloak-style mutations that require the `ProcOnHit` feature to also be enabled.
* If `ProcOnSpells` refers to a valid `SpellGroup` it will be used instead of the default cloak list
*  is true the spells in `CloakSpells` will be used instead of the normal pool.
* ***Still requires you to be wearing a cloak with a proc.***  Probably could change this with a rewrite/Postfix of `SpellProjectile.DamageTarget` and `Player.TakeDamage` which make the checks.



#### Set

* Adds a set based on the `EquipmentSetGroups` corresponding to the `TreasureItemType_Orig` of the item in `ItemTypeEquipmentSets`

* By default: 
  * Armor/clothing roll armor sets  
  * Cloak/jewelry rolls cloak sets
  * Missing/Weapons roll nothing





#### Slayer

* `SlayerPower` determines the power of the corresponding tier of item.
* If `Slayers` refers to a valid `CreatureTypeGroup` it will be used instead of the full list
  * Invalid|Unknown|Wall are removed from the pool




### Features


#### EnableOnAttackForNonAetheria

* `EnableOnAttackForNonAetheria` is needed to patch ACE to check non-Aetheria for OnAttack triggers
  * TitaniumWeenie's [UniqueWeenies](https://github.com/titaniumweiner/ACEUniqueWeenies) contains compatible weenies








### Video

<details>
 <summary>Sets</summary>

https://github.com/aquafir/ACE.BaseMod/assets/83029060/1300de91-fa7f-442c-a2f1-527bc4a282f0
</details>

<details>
 <summary>OnAttack Proc</summary>
https://github.com/aquafir/ACE.BaseMod/assets/83029060/81e635c1-115a-453e-b1e3-c2efbf67d781
</details>




### Enums

* [EquipmentSet](https://github.com/ACEmulator/ACE/blob/fdfdec9f0a16bbcbb89a9120ce4f889520a51708/Source/ACE.Entity/Enum/EquipmentSet.cs#L4)
* [TreasureItemType_Orig enum](https://github.com/ACEmulator/ACE/blob/fdfdec9f0a16bbcbb89a9120ce4f889520a51708/Source/ACE.Server/Factories/Enum/TreasureItemType_Orig.cs#L4)

* [SpellId](https://github.com/ACEmulator/ACE/blob/fdfdec9f0a16bbcbb89a9120ce4f889520a51708/Source/ACE.Entity/Enum/SpellId.cs#L4)

* Sigil
* Surge





### Todo

* EquipmentCache caps
* Need to use [different collections](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/data-structures-for-parallel-programming) for parallel support?
* Mutations
  * Check mutations compatibility
  * Don't patch various MutatorHooks if they have no Mutator with that type

* 
* ~~ArrayOf for pools for constant time sampling?~~
* If ACE ever goes .NET >7 should switch to polymorphic serialization 
* Currently doesn't wipe a set/proc if one exists and a mutation isn't rolled.
* Possibly inefficient way of checking for self-targeting spells
* Possible increase in efficiency by intercepting WO creation instead of mutating after creation







## Scratch

* Convert damage to periodic damage
* Saving grace - chance of damage > hp being set to hp - 1
* Sweetspot angle, support with UB?
* Weal/woe
* Autocombat
* Flat vs. multi
* Clamp in cache or return?
* Bonus based on height / size
* Attributes / Vitals
  * CreatureVital.Base factors in Enlightenmentt
* Vendor AlternateCurrency
  * Award based on scaled difficulty
* skill trained/under
* Spells
  * Custom thoughts
    * Meteors that trigger on destroy?
    * Outdoor only / line of sight?
  * SpellProjectiles are WorldObjects so I can use props
    * WO.CreateSpellProjectiles
      * SpellProjectile.GetProjectileSpellType splits into types
      * WO.CalculateProjectileOrigins uses the type to return a Vector3 list of locations
      * WO.CalculateProjectileVelocity then calculates velocity *using the first Vector3 ??*
      * `Trajectory.solve_ballistic_arc_lateral` or `Trajectory2.CalculateTrajectory` get used to calculate path depending on gravity
    * Then launched by WO.LaunchSpellProjectiles
    * Has
      * OnCollideEnvironment
      * ProjectileImpact
      * `SpellType`
      * SpawnPos `Position`
      * `SpellProjectileInfo` ?
        * Caster/Target `Position`
        * Velocity
      * IsWeaponSpell - true when from caster
      * FromProc - to prevent retries
      * 
  * Position
    * InFrontOf
  * Spell
    * IconId
    * Category - used for family of buffs
    * SpellFlags - diff type flags
  * SpellBase
    * Lots of unused ones
    * Name
    * Desc
    * Icon
    * BaseMana
    * BaseRange
    * BaseRangeConstant
    * Power - Used to determine which spell in the catgory is the strongest.
    * MetaSpellType - subtype
    * ManaMod - additional cost per target
    * Duration
    * DisplayOrder?
* cleave
* kdtree
  * aura
  * transfer falling damage if close enough
* craft all / craft queue
* steal->alt curr
* spell ref
* refl non-spell
* rage
  * Low health / Low stam / low mana
* combo over x pow/acc/lvl
* combo same target
* shield flat/per
* landblock specific wield
* durability
* Sets.  Finds spells in `DatManager.PortalDat.SpellTable.SpellSet`
  * * Combined item levels in a sorted dictionary for `SpellSetTiers` list of spells







https://i.gyazo.com/9d12388f6e416f76c281ea3436ef57e8.mp4
