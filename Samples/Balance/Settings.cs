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
        new (PatchType.GrantExperience, false),
        new (PatchType.NetherRating, false),
        new (PatchType.MeleeAttributeDamage),
        new (PatchType.MissileAttributeDamage, false),
        new (PatchType.LevelCost, false)
        //new MovementPatches(),
    };
}
