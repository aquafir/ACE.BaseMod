namespace Raise;

public class AlternateLevelingSettings
{
    public bool Enabled { get; set; } = true;
    //PropertyInt - tracks ranks/levels of a property
    public int LevelPropertyStart { get; set; } = 20800;
    //PropertyInt64 - Tracks total amount spent on a property
    public int SpentPropertyStart { get; set; } = 20800;

    //Use ACE values where available instead of immediately using 
    public bool PreferStandard { get; set; } = true;
    //Instead of treating the level after the last standard value, treat it as 0 ranks
    public bool OffsetByLastStandard { get; set; } = true;

    public LevelCost Attribute { get; set; } = new(10000000, 2, 5000000, GrowthType: GrowthType.Polynomial);
    public LevelCost Vital { get; set; } = new(1000000000, 1000000000, 0, GrowthType: GrowthType.Linear);
    public LevelCost Trained { get; set; } = new();
    public LevelCost Specialized { get; set; } = new(100, 1.5);
}
