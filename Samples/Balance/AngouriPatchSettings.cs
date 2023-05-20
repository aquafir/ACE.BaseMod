using System.Runtime.CompilerServices;

namespace Balance;

public partial class AngouriPatchSettings
{
    public PatchType Type { get; set; }
    public bool Enabled { get; set; } = true;
    public string Formula { get; set; }

    public AngouriPatchSettings() { }

    public AngouriPatchSettings(PatchType type, bool enabled = true, string formula = "")
    {
        Type = type;
        Enabled = enabled;
        Formula = formula;
    }

    public AngouriMathPatch CreatePatch()
    {
        AngouriMathPatch patch = Type switch
        {
            PatchType.ArmorMod => new ArmorMod(),
            PatchType.CripplingBlowImbueMod => new CripplingBlowImbueMod(),
            PatchType.CriticalStrikeImbueMod => new CriticalStrikeImbueMod(),
            PatchType.ElementalRendingImbue => new ElementalRendingImbue(),
            PatchType.GrantExperience => new GrantExperience(),
            PatchType.HealingDifficulty => new HealingDifficulty(),
            PatchType.LevelCost => new LevelCost(),
            PatchType.MagicWeaponCriticalChance => new MagicWeaponCriticalChance(),
            PatchType.MeleeArmorRending => new MeleeArmorRending(),
            PatchType.MeleeAttributeDamage => new MeleeAttributeDamage(),
            PatchType.MissileArmorRending => new MissileArmorRending(),
            PatchType.MissileAttributeDamage => new MissileAttributeDamage(),
            PatchType.NetherRating => new NetherRating(),
            PatchType.PlayerAccuracyMod => new PlayerAccuracyMod(),
            PatchType.PlayerPowerMod => new PlayerPowerMod(),
            PatchType.WeaponCriticalChance => new WeaponCriticalChance(),
            _ => throw new NotImplementedException(),
        };

        //If the formula is missing from the settings use the default of the patch, otherwise set the patch's formula to the settings value
        if (string.IsNullOrWhiteSpace(Formula))
            Formula = patch.Formula;
        else
            patch.Formula = Formula;

        return patch;
    }
}

public enum PatchType
{
    GrantExperience,
    LevelCost,
    MeleeAttributeDamage,
    MissileAttributeDamage,
    NetherRating,
    PlayerAccuracyMod,
    ArmorMod,
    MissileArmorRending,
    MeleeArmorRending,
    CripplingBlowImbueMod,
    CriticalStrikeImbueMod,
    ElementalRendingImbue,
    MagicWeaponCriticalChance,
    WeaponCriticalChance,
    PlayerPowerMod,
    HealingDifficulty,
}