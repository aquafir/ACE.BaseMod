using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoot.Mutators;
internal class DungeonLocked : Mutator
{
    //!!Relies on CorpseInfo
    public override bool TryMutate(TreasureDeath profile, TreasureRoll roll, HashSet<Mutation> mutations, WorldObject item)
    {
        
    }
}
