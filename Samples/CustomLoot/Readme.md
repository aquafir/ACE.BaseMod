## CustomLoot
 
This mod changes the loot generated objects returned by `LootGenerationFactory.CreateAndMutateWcid`


### Slayer

* `SlayerChance` is a 0-1 chance of adding a random Slayer property to a weapon.
  * `SlayerPower` determines the power of the corresponding tier of item.



### Cloak-style Mutation

* For each of armor/jewelry/clothing, `CloakMutationChance` is a 0-1 chance of adding Cloak properties
  * If the `TreasureItemType_Orig` is missing that item type doesn't attempt a cloak mutation
  * If `UseCustomCloakSpellProcs` is true the spells in `CloakSpells` will be used instead of the normal pool.
* `EnableOnHitForNonCloak` is needed to patch ACE to check non-Cloaks for OnHit triggers
  * ***This requires you to be wearing a cloak with a proc.***  Probably could change this with a rewrite/Postfix of `SpellProjectile.DamageTarget` and `Player.TakeDamage` which make the above checks.



### Set Mutation

* For each of armor/jewelry/clothing/weapon (or added `TreasureItemType_Orig`), `CloakMutationChance` is a 0-1 chance of adding Cloak properties

* If the `TreasureItemType_Orig` is missing it won't attempt a set mutation.
* `CustomSets` is a `TreasureItemType_Orig` with a list of set IDs that it can roll.  By default: 
  * Armor/clothing roll armor sets  
  * Cloak/jewelry rolls cloak sets
  * Weapons (or missing) roll nothing


### Aetheria

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



### Notes

* Per-tier chances would be a good candidate for future customization
* Currently doesn't wipe a set/proc if one exists and a mutation isn't rolled.
* Possibly inefficient way of checking for self-targeting spells
* Possible increase in efficiency by intercepting WO creation instead of mutating after creation



### Levels?

* Needs all 3
  * ItemMaxLevel - max level
  * ItemBaseXp - xp per level
  * ItemXpStyle
    * Undef, Fixed, ScalesWithLevel, FixedPlusBase
* `Player.OnItemLevelUp`
  * Removes/adds spells using `GetSpellSet`
  * Finds spells in `DatManager.PortalDat.SpellTable.SpellSet`
    * Combined item levels in a sorted dictionary for `SpellSetTiers` list of spells
