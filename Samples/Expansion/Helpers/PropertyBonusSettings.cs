namespace Expansion.Helpers;
public class PropertyBonusSettings
{
    public Dictionary<FakeInt, int> MinInt { get; set; } = new();
    public Dictionary<FakeInt, int> MaxInt { get; set; } = new();
    public Dictionary<FakeFloat, double> MinFloat { get; set; } = new();
    public Dictionary<FakeFloat, double> MaxFloat { get; set; } = new()
    {
        [FakeFloat.ItemSpellSplashCooldownScale] = .9,
        [FakeFloat.ItemSpellSplitCooldownScale] = .9,
    };

}
