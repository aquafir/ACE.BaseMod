namespace Balance;

public class Settings
{
    //Patches will involve a single formula and variable definitions
    public List<AngouriMathPatch> Formulas { get; set; } = new()
    {
        new GrantExperience(),
        //new LevelingPatches(),
        //new NetherPatches(),
        //new MovementPatches(),
    };

    //Additional settings for patches.
    //Per-patch settings was considered but would require custom JSON converters that don't seem worth it

}
