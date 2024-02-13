namespace Expansion;

public class Odds
{
    public Dictionary<int, double> TierChance { get; set; } = new();

    /// <summary>
    /// True if 
    /// </summary>
    /// <param name="treasureDeath"></param>
    /// <returns></returns>
    public bool Roll(TreasureDeath treasureDeath) => TierChance.TryGetValue(treasureDeath.Tier, out var chance) && ThreadSafeRandom.Next(0f, 1.0f) < chance;
}