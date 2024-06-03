namespace Tower;

public class OfflineProgressSettings
{
    public bool Enabled { get; set; } = true;
    public TimeSpan MaxTime { get; set; } = TimeSpan.FromHours(24 * 3);
    public TimeSpan MinDisplayTime { get; set; } = TimeSpan.FromHours(0);

    public Dictionary<int, OfflineRewardTier> RewardTiers = new()
    {
        [0] = new(3077, Player.coinStackWcid, 1),
        [1] = new(25041, Player.coinStackWcid, 2),
        [2] = new(98422, Player.coinStackWcid, 3),
        [3] = new(612768, Player.coinStackWcid, 4),
        [4] = new(1202066, Player.coinStackWcid, 5),
        [5] = new(2139447, Player.coinStackWcid, 6),
        [6] = new(3540745, Player.coinStackWcid, 7),
        [7] = new(5538460, 691, 1),
        [8] = new(8281758, 691, 2),
        [9] = new(11936473, 691, 3),
        [10] = new(16685104, 691, 4),
        [11] = new(22726818, 691, 5),
        [12] = new(22726818, 691, 6),
    };

    //public double LootPerHour, TimeSpan MinTime, 
}
public record OfflineRewardTier(double XpPerHour, uint LootWcid, double LootPerHour);