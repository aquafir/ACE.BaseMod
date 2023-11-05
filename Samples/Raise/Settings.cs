namespace Raise;

public class Settings
{
    /* Max level */
    public int MaxLevel { get; set; } = 9999;
    public ulong CostPerLevel { get; set; } = 1_000_000_000u;
    public int CreditInterval { get; set; } = 10;

    /* Raise command */
    // (Multi * L) / (RaiseDecay - LevelDecay * L), L = Level
    public double RaiseMulti { get; set; } = 3_292_201_940D;     
    public double RaiseDecay { get; set; } = 7.995D;
    public double LevelDecay { get; set; } = 0.001D;

    //Flat luminance costs
    public long RatingMulti { get; set; } = 15000000;       //Luminance cost for offense/defense
    public long WorldMult { get; set; } = 5000000;          //Luminance cost for World

    public uint RaiseMax { get; set; } = 1000;
    //Conversion of RaiseTarget to the PropertyInt key that tracks them
    public int PropertyOffset { get; set; } = 1000;
    //public TimeSpan RAISE_TIME_BETWEEN_REFUND { get; set; } = TimeSpan.FromMinutes(60.0);
}