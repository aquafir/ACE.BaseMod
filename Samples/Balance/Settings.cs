namespace Balance;

public class Settings
{
    public uint MaxLevel { get; set; } = 275;
    //x = level
    public string CostPerLevelFormula { get; set; } = "1000 * x^3/2";

    //Use nether rating formula?
    public bool NetherRatingOverride { get; internal set; } = true;
    //x = regular rating, n = number of debuffs
    public string NetherRatingFormula { get; set; } = "P(100 if n=3 and x>100, x if n=3 and x>60, 60 if x>60, x)";
}