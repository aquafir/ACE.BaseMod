namespace Raise
{
    public class Settings
    {
        public double AttrMult { get; set; } = 3292201940D;     // (Mult * L) / (MultDecay - LevelDecay * L)
        public double AttrMultDecay { get; set; } = 7.995D;
        public double AttrLevelDecay { get; set; } = 0.001D;

        public long RaitingMult { get; set; } = 15000000;       //Luminance cost for offense/defense
        public long WorldMult { get; set; } = 5000000;          //Luminance cost for World

        public uint RaiseMax { get; set; } = 1000;
        //public TimeSpan RAISE_TIME_BETWEEN_REFUND { get; set; } = TimeSpan.FromMinutes(60.0);
    }
}