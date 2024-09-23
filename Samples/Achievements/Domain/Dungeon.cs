namespace Tinkering.Domain;

public class Dungeon
{
    public uint DungeonId { get; set; }

    public byte[] Snapshot { get; set; }

    public Adventurer Adventurer { get; set; }
    public uint AdventurerId { get; set; }
}
