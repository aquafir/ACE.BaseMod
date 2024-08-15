## Spell Customization

`CustomSpells` contains a list of `SpellCustomization` that either create or update spells.

Default values use the required `SpellId` `Template`, and the spell that is created/updated is the required destination `Id`.  



Setting those to the same will update the template spell, and the ID can be used in place of the `SpellId` string:

```json
      "Template": "strengthSelf1",
      "Id": 9999,
```



Values that may be set (not hard to support other `SpellBase` / `Spell` values if desired):

* `string` Name
* `string` SpellWords
* `MagicSchool` School -  The magic school this spell belongs to
* `SpellCategory` Category -  Used for spell stacking, ie. Strength Self I and Strength Self VI will be the same category
* `SpellFlags` Bitfield -  bit flags for the spell
* `uint` BaseMana -  The base mana cost required for casting the spell
* `float` BaseRangeConstant - The base maximum distance for casting the spell
* `float` BaseRangeMod -  An additive multiplier to BaseRangeConstant based on caster's skill level
* `uint` Power -  The difficulty of casting the spell
* `EnchantmentTypeFlags` StatModType -  The stat modifier type, combined flags controlling a variety of things
* `uint` StatModKey -  The stat modifier key, used for lookup in the enchantment registry, with a value from a few possible enums
  * `Skill`
  * `PropertyAttribute`
  * `PropertyAttribute2nd`
  * `PropertyInt`
  * `PropertyFloat`
* `float` StatModVal -  The amount to modify a stat
* `DamageType` EType -  The damage type for this spell
* `DamageType` DamageType -  DamageType used by LifeMagic spells that specifies Health, Mana, or Stamina for the Boost type spells
* `int` BaseIntensity -  The base amount of damage for this spell
* `int` Variance -  The maximum additional daamage for this spell
* `double` Duration -  The amount of time the spell lasts for EnchantmentSpell / FellowshipEnchantmentSpells
* `double` DotDuration -  The DoT (damage over time) duration for the spell
* `PlayScript` CasterEffect -  Effect that plays on the caster for this spell (ie. for buffs, protects, etc.)
* `PlayScript` TargetEffect -  Effect that plays on the target for this spell (ie. for debuffs, vulns, etc.)
* `uint` Wcid = null -  The weenie class ID associated for this spell, ie. the projectile weenie class id



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



## Set Customization

`Sets` contains a dictionary mapping `EquipmentSet` to a list of `SetTier`, which in turn are a list of the required equipment (`NumEquipped`) to apply the set `Spells` (a list of `SpellId`).



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

