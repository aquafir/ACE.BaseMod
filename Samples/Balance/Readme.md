## Balance

This mod lets you override the formula of a variety of balance-related features using [AngouriMath](https://am.angouri.org/docs/AngouriMath/MathS.html) ([docs](https://am.angouri.org/docs/namespaces.html) / [details](https://habr.com/en/articles/546926/)).



Available patches reside in the [`Patches` folder](https://github.com/aquafir/ACE.BaseMod/tree/master/Samples/Balance/Patches) and can be enabled individually with their `Enabled` property and adjusted with the `Formula` using variables defined in the patch:

```csharp
    {
      "PatchType": "WeaponCriticalChance",
      "Enabled": true,
      "Formula": "x + .01r"
    },
```



### Patches

* Armor
* CripplingBlowImbue
* CriticalStrikeImbue
* ElementalRendingImbue
* GrantExperience
* HealingDifficulty
* LevelCost
* MeleeArmorRending
* MeleeAttributeDamage
* MissileArmorRending
* MissileAttributeDamage
* NetherRating
* PlayerAccuracy
* PlayerPower
* PlayerTakeDamage
* PlayerTakeDamageOverTime
* SkillChance
* WeaponCriticalChance
* WeaponMagicCritFrequency



