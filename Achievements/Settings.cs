namespace Achievements
{
    public class Settings
    {
        public Dictionary<string, Dictionary<string, uint>> Kills { get; set; } = new();
        public int Interval { get; set; } = 10;
        public int Multiplier { get; set; } = 7;
    }
}