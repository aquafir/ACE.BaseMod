namespace Respawn
{
    public class Settings
    {
        public double RespawnTreshold { get; set; } = 2.9;   //The percent remaining alive at which a respawn is triggered
        public double Interval { get; set; } = 5;           //Interval in seconds

        [JsonIgnore]
        public readonly HashSet<uint> CREATURE_GENERATOR_IDS = new HashSet<uint> { 1154, 3951, 3953, 3955, 4219, 5086, 5485, 7923, 7924, 7925, 7926, 7932, 15274, 21120, 24129, 28282 };
    }
}