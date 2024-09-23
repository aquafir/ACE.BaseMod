using Balance.Patches;

namespace Balance;

public class Settings
{
    //Additional settings for patches.
    //Per-patch settings was considered but would require custom JSON converters
    public uint MaxLevel { get; set; } = 275;

    //Patches will involve a single formula and variable definitions
    public List<AngouriPatchSettings> Formulas { get; set; } = new()
    {
        //Default formulas in patches.  Not exposing variables/names
        new (nameof(ArmorMod)),
        new (nameof(CripplingBlowImbueMod)),
        new (nameof(CriticalStrikeImbueMod)),
        new (nameof(ElementalRendingImbue)),
        new (nameof(GrantExperience), false),
        new (nameof(HealingDifficulty)),
        new (nameof(LevelCost), false),
        new (nameof(MagicWeaponCriticalChance)),
        new (nameof(MeleeArmorRending)),
        new (nameof(MeleeAttributeDamage)),
        new (nameof(MissileArmorRending)),
        new (nameof(MissileAttributeDamage), false),
        new (nameof(NetherRating), false),
        new (nameof(PlayerAccuracyMod)),
        new (nameof(PlayerPowerMod)),
        new (nameof(WeaponCriticalChance)),
        new (nameof(SkillChance)),
        new (nameof(PlayerTakeDamage)),
        new (nameof(PlayerTakeDamageOverTime)),
    };


    public bool Verbose { get; set; } = false;
}