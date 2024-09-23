namespace Tinkering
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
    }
}