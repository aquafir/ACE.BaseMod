## Spell Customization

`CustomSpells` contains a list of `SpellCustomization` that either create new spells or update existing spells.

Default values come from the required `SpellId` `Template`, and the spell that is created/updated is the `Id`, defaulting to updating the template.  



Setting those to the same will update the template spell.  You can also use a number ID in place of a `SpellId`:

```json
      "Template": "strengthSelf1",
      "Id": 9999,
```



The below renames "Strength Self I", lowers it to 5 before updating the `Template` with those changed values when assigning to the same `Id`.  It then creates a new spell (testable with `/castspell 9999`) with a different name, buffed amount, and changes its `Category` so it doesn't overlap with other buffs .

```json
  "CustomSpells": [
    {
      "Template": "strengthSelf1",
      "Id": "strengthSelf1",
      "Name": "Lesser Strength I",
      "StatModVal": 5
    },
    {
      "Template": "strengthSelf1",
      "Id": 9999,
      "Name": "Boosted Strength I",
      "Category": 987,
      "StatModVal": 111
    }
  ],
```



### Spreadsheet

If `AutoloadSpreadsheet` is true, a valid Excel spreadsheet set with `Spreadsheet` will be applied when starting.  This may be done manually with the `/loadspells` command.



You can copy/edit/download a [template](https://docs.google.com/spreadsheets/d/1Ya_oDlCZ-AJwV4qcXsZ3m15NqgSGOgtLlPAPuIn8bj0/edit?usp=sharing) (basic data validation included) with `File→Download→Microsoft Excel (.xlsx)` and place that file in your mod directory.

* Columns can be hidden
  * They may also be removed or re-ordered, but it may be more difficult to copy a spell from a spell dump directly if you do.
* `/spelldump` will create `Dump.xlsx` for all existing `SpellId` for reference, 
  * Or you can [use this](https://docs.google.com/spreadsheets/d/1fdRatNizjyBEsSwFQKKYqfouksQVrc96Vs56O_l8I0U/edit?usp=sharing)
* All cells except the `Template` are optional



### Values

Values that may be set (in a rough grouping by category):

* **Text**

  * `string` Name
  * `string` SpellWords

* **Meta**

  * `MagicSchool` School -  The magic school this spell belongs to
  * `SpellCategory` Category -  Used for spell stacking, ie. Strength Self I and Strength Self VI will be the same category
  * `SpellFlags` Bitfield -  bit flags for the spell
  * `SpellType` MetaSpellType - A subtype for the spell

* **Casting Requirements**

  * `uint` BaseMana -  The base mana cost required for casting the spell
  * `float` BaseRangeConstant - The base maximum distance for casting the spell
  * `float` BaseRangeMod -  An additive multiplier to BaseRangeConstant based on caster's skill level
  * `uint` Power -  The difficulty of casting the spell
  * `ItemType` NonComponentTargetType - The allowed target types for this spell.  *Probably not be used in client.*
  * `List<uint>` Formula - *Values correspond to the SpellComponentsTable*

* **Enchantments**

  * `EnchantmentTypeFlags` StatModType -  The stat modifier type, combined flags controlling a variety of things
  * `uint` StatModKey -  The stat modifier key, used for lookup in the enchantment registry, with a value from a few possible enums
    * `Skill`
    * `PropertyAttribute`
    * `PropertyAttribute2nd`
    * `PropertyInt`
    * `PropertyFloat`

  * `float` StatModVal -  The amount to modify a stat
  * `double` Duration -  The amount of time the spell lasts for EnchantmentSpell / FellowshipEnchantmentSpells
  * `double` DotDuration -  The DoT (damage over time) duration for the spell

* **Projectiles**

  * `DamageType` EType -  The damage type for this spell
  * `DamageType` DamageType -  DamageType used by LifeMagic spells that specifies Health, Mana, or Stamina for the Boost type spells
  * `int` BaseIntensity -  The base amount of damage for this spell
  * `int` Variance -  The maximum additional damage for this spell
  * `float` SpreadAngle - The total angle for multi-projectile spells (90 degrees for 3-5 projectiles, or 360 degrees for ring spells)
  * `bool` NonTracking - If this is on then projectile spells won't lead a target
  * `int` NumProjectiles - The total # of projectiles launched for this spell
  * `Vector3` CreateOffset - The offset to apply to the spawn position
  * `Vector3` Padding - The minimum amount of padding to ensure for the spell to spawn
  * `Vector3` Peturbation - The maximum variation for spawn position

* **Appearance**

  * `PlayScript` CasterEffect -  Effect that plays on the caster for this spell (ie. for buffs, protects, etc.)
  * `PlayScript` TargetEffect -  Effect that plays on the target for this spell (ie. for debuffs, vulns, etc.)
  * `uint` Wcid -  ***This must be a `SpellProjectile` or it will cause a crash!!*** The weenie class ID associated for this spell, ie. the projectile weenie class id

* **Drains / Transfers / Boosts**

  * `float` DrainPercentage - The amount of source vital to drain for a life spell
  * `float` DamageRatio - The percentage of DrainPercentage to damage a target for life projectiles
  * `int` Boost - The minimum amount of vital boost from a life spell
  * `int` BoostVariance - `Boost` + `BoostVariance` = the maximum amount of vital boost from a life spell

  * `PropertyAttribute2nd` Source - The source vital for a life spell
  * `PropertyAttribute2nd` Destination - The destination vital for a life spell
  * `float` Proportion - The propotion of source vital to transfer to destination vital
  * `float` LossPercent - The percent of source vital loss for a life magic transfer spell
  * `int` TransferCap - The maximum amount of vital transferred by a life magic spell

  * `TransferFlags` TransferFlags - Indicates the source and destination for life magic transfer spells

* **Portals**

  * `double` PortalLifetime - The duration for PortalSummon_SpellType
  * `int` Link - For SpellType.PortalSummon spells, Link is set to either 1 for LinkedPortalOneDID or 2 for LinkedPortalTwoDID
  * `Position` Position - A destination location for a spell

* **Dispel**

  * `int` MinPower - The minimum spell power to dispel (unused?)
  * `int` MaxPower - The maximum spell power to dispel
  * `MagicSchool` DispelSchool - The magic school to dispel, or undefined for all schools
  * `DispelType` Align - The type of spells to dispel (0 = all spells, 1 = positive, 2 = negative)
  * `int` Number - The maximum # of spells to dispel
  * `float` NumberVariance - `Number` * `NumberVariance` = the minimum # of spells to dispel

* *Many unused properties ignored*

  


## Set Customization

`Sets` contains a dictionary mapping `EquipmentSet` to a list of `SetTier`, which in turn are a list of the required equipment (`NumEquipped`) to apply the set `Spells` (a list of `SpellId`).



`/listset` is added as a command to list the set tiers/spells of the last appraised item.



The below updates the `Adepts` `EquipmentSet` to have two tiers (@ 3/4 equipped) with the shown spells.  It also creates a fake set that can be used with the key "999".

```json
  "Sets": {
    "adepts": [
      {
        "NumEquipped": 3,
        "Spells": [
          "harmSelf3",
          "acidProtectionSelf3"
        ]
      },
      {
        "NumEquipped": 4,
        "Spells": [
          "harmSelf5",
          "acidProtectionSelf5"
        ]
      }
    ],
    "999": [
      {
        "NumEquipped": 1,
        "Spells": [
          "fireProtectionSelf6"
        ]
      },
      {
        "NumEquipped": 2,
        "Spells": [
          "fireProtectionSelf7"
        ]
      }
    ]
  }
```



## Commands

* `/listset` lists the set spells (if any) of the last appraised object
* `/spelldump` is a console-only command that exports a spreadsheet with all available `SpellCustomization` as rows to `Dump.xlsx`
* `/loadspells` manually loads and applies the custom spell spreadsheet
