namespace Respawn
{
    public class Settings
    {
        public double RespawnTreshold { get; set; } = 0.2;  //The percent remaining alive at which a respawn is triggered
        public double Interval { get; set; } = 10;          //Interval in seconds to check landblocks
        public bool ExceedMax { get; set; } = false;        //If true you will be able to spawn more than the max of a generator.  Fully respawns LB
        public bool DetailedDump { get; set; } = true;      //Displays lists of creatures and their counts in a lb instead of just count with /left
        public bool RewardLastKill { get; set; } = true;    //Give a bonus to the Player with the last kill when a landblock respawns
        public long RewardAmount { get; set; } = 100000000;
        public bool SpamPlayer { get; set; } = true;        //Display remaining on kill

        [JsonIgnore]
        public readonly HashSet<uint> CREATURE_GENERATOR_IDS = new HashSet<uint> { 1154, 3951, 3953, 3955, 4219, 5086, 5485, 7923, 7924, 7925, 7926, 7932, 15274, 21120, 24129, 28282 };
        //WeenieClassId == 3666 is for Placeholder generators, but that isn't in the list of IDs so it doesn't matter
    }
}