namespace Tinkering.Domain;

public class Land
{
    public ulong LandId { get; set; }

    public bool Explored { get; set; }
    //public bool[,] Explorer { get; set; }

    public HashSet<Adventurer> Adventurers { get; set; }
    //public uint AdventurerId { get; set; }
}
