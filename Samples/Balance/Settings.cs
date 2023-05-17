namespace Balance;

public class Settings
{
    //Patches will involve a single formula and variable definitions
    public Dictionary<string, AngouriMathPatch> Formulas { get; set; } = new()
    {
        //Hate doing it this way.  Custom property names with nameof weren't being picked up.
        ["Grant Experience"] = new GrantExperience(),
        ["Nether Rating"] = new NetherRating(),
        ["Melee Attribute Damage"] = new MeleeAttributeDamage(),
        ["Missile Attribute Damage"] = new MissileAttributeDamage(),
        //["Level Cost"] = new LevelCost(),
        //new MovementPatches(),
    };

    //Additional settings for patches.
    //Per-patch settings was considered but would require custom JSON converters that don't seem worth it
    public uint MaxLevel { get; set; } = 275;
}
