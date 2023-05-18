namespace Balance;

public class Settings
{
    //Patches will involve a single formula and variable definitions
    public Dictionary<PatchType, AngouriPatchSettings> Formulas { get; set; } = new()
    {
        //Hate doing it this way.  Custom property names with nameof weren't being picked up.
        [PatchType.GrantExperience] = new (),
        [PatchType.NetherRating] = new (),
        [PatchType.MeleeAttributeDamage] = new (),
        [PatchType.MissileAttributeDamage] = new (),
        //["Level Cost"] = new LevelCost(),
        //new MovementPatches(),
    };

    //Additional settings for patches.
    //Per-patch settings was considered but would require custom JSON converters that don't seem worth it
    public uint MaxLevel { get; set; } = 275;
}
