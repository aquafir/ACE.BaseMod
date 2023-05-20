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

        //new (PatchType.ArmorMod),
        //new (PatchType.CripplingBlowImbueMod),
        //new (PatchType.CriticalStrikeImbueMod),
        //new (PatchType.ElementalRendingImbue),
        //new (PatchType.GrantExperience, false),
        //new (PatchType.HealingDifficulty),
        //new (PatchType.LevelCost, false),
        //new (PatchType.MagicWeaponCriticalChance),
        //new (PatchType.MeleeArmorRending),
        //new (PatchType.MeleeAttributeDamage),
        //new (PatchType.MissileArmorRending),
        //new (PatchType.MissileAttributeDamage, false),
        //new (PatchType.NetherRating, false),
        //new (PatchType.PlayerAccuracyMod),
        //new (PatchType.PlayerPowerMod),
        //new (PatchType.WeaponCriticalChance),
    };
}