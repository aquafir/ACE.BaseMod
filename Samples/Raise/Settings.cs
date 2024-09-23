namespace Raise;

public class Settings
{
    /* Max level */
    public int MaxLevel { get; set; } = 9999;
    //public ulong CostPerLevel { get; set; } = 1_000_000_000u;
    public int CreditInterval { get; set; } = 10;

    public LevelCost LevelCost { get; set; } = new(1_000_000_000u, 1_000_000_000u, 0, -276, GrowthType: GrowthType.Linear);

    public RaiseSettings Raise { get; set; } = new();
    public AlternateLevelingSettings AlternateLeveling { get; set; } = new();
}

public class RaiseSettings
{
    public bool Enabled { get; set; } = true;

    /* Raise command */
    // (Multi * L) / (RaiseDecay - LevelDecay * L), L = Level
    public double RaiseMulti { get; set; } = 3_292_201_940D;
    public double RaiseDecay { get; set; } = 7.995D;
    public double LevelDecay { get; set; } = 0.001D;

    //Flat luminance costs
    //public LevelCost LuminanceRaiseCosts { get; set; } = new(1_000_000_000u, 1_000_000_000u, 0, GrowthType.Linear);

    public long RatingMulti { get; set; } = 15000000;       //Luminance cost for offense/defense
    public long WorldMult { get; set; } = 5000000;          //Luminance cost for World

    public uint RaiseMax { get; set; } = 1000;
    //Conversion of RaiseTarget to the PropertyInt key that tracks them
    public int PropertyOffset { get; set; } = 51000;
    //public TimeSpan RAISE_TIME_BETWEEN_REFUND { get; set; } = TimeSpan.FromMinutes(60.0);


    //Used if a specific cost not supplied for Raise target
    //public LevelCost DefaultRaiseCost { get; set; } = new(1_000_000_000u, 1_000_000_000u, 0, GrowthType: GrowthType.Linear);
    //public Dictionary<RaiseTarget, LevelCost> RaiseCosts { get; set; } = new()
    //{

    //    [RaiseTarget.World] = new(),
    //    [RaiseTarget.Offense] = new(),
    //    [RaiseTarget.Defense] = new(),
    //};

    //public HashSet<RaiseTarget> UsesLuminance { get; set; } = new()
    //{
    //    RaiseTarget.World,
    //    RaiseTarget.Offense,
    //    RaiseTarget.Defense,
    //};
}

