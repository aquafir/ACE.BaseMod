using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Achievements.Domain;

public class Kill
{
    //Same as weenie?
    public ulong KillId { get; set; }
    //public ulong Wcid { get; set; }
    public ulong Count { get; set; }

    //public HashSet<Adventurer> Adventurers { get; set; }
    public Adventurer Adventurer { get; set; }
    public uint AdventurerId { get; set; }
}
