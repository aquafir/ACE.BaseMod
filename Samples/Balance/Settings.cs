namespace Balance
{
    public class Settings
    {
        public int MaxLevel { get; set; } = 300;
        public ulong CostPerLevel { get; set; } = 1_000_000_000;
        public int NetherRatingCap { get; set; } = 60;      //Highest rating is 60
        public int NetherPerDebuffCap { get; set; } = 30;   //Highest rating per debuff is 30, requiring at least to hit the cap
    }
}