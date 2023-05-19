using System.Runtime.CompilerServices;

namespace Balance.Patches;

public class AngouriPatchSettings
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
            PatchType.GrantExperience => new GrantExperience(),
            PatchType.LevelCost => new LevelCost(),
            PatchType.MeleeAttributeDamage => new MeleeAttributeDamage(),
            PatchType.MissileAttributeDamage => new MissileAttributeDamage(),
            PatchType.NetherRating => new NetherRating(),
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
}