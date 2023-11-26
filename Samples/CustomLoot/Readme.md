## CustomLoot

This mod changes the loot generated objects returned by `LootGenerationFactory.CreateAndMutateWcid`



### **Overview**

* **Features** are patches that enable additional functionality, such as a "reduced weight" property for a container
  * Some mutations may require a feature for the item they produce to do what it should.
* **Mutators** change loot after its generated.
  * `Targets` is a set of `TreasureItemType_Orig` the mutation applies to
  * `Odds` is the 0-1 chance of being applied to an item of a tier
  * There are default sets of both but you can make your own.
  * You can make multiple Mutators if you want to have different odds for different targets.
* Settings for Mutators/Features are currently just dumped in `Settings.json`.  



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

* Check mutations compatibility
* ~~ArrayOf for pools for constant time sampling?~~
* If ACE ever goes .NET >7 should switch to polymorphic serialization 
* Currently doesn't wipe a set/proc if one exists and a mutation isn't rolled.
* Possibly inefficient way of checking for self-targeting spells
* Possible increase in efficiency by intercepting WO creation instead of mutating after creation







## Scratch

* skill trained/under
* import spell split
* cleave
* aura + kdtree
* steal->alt curr
* spell ref
* refl non-spell
* rage
* combo over x pow/acc/lvl
* combo same target
* shield flat/per
* landblock specific wield
* durability
* Sets.  Finds spells in `DatManager.PortalDat.SpellTable.SpellSet`
  * * Combined item levels in a sorted dictionary for `SpellSetTiers` list of spells
