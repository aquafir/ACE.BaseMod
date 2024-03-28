namespace Achievements.Domain;

public class Adventurer
{
    //Mirror of ACE.  Todo: think about issues that come from this like changing ACE IDs
    public uint AdventurerId { get; set; }
    //ACE access of player.  Make adventurer identical?
    //public string Name { get; set; }
    //public uint AceId { get; set; }

    public HashSet<Kill> Kills { get; set; }
    public HashSet<Land> Lands { get; set; } = new();
    //public List<Land> Lands { get; set; } = new();

    //public HashSet<Dungeon> Dungeons { get; set; }
    //public List<Combat> Combats { get; set; }
}
