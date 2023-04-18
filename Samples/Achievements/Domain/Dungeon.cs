using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Achievements.Domain;

public class Dungeon
{
    public uint DungeonId { get; set; }

    public byte[] Snapshot { get; set; }

    public Adventurer Adventurer { get; set; }
    public uint AdventurerId { get; set; }
}
