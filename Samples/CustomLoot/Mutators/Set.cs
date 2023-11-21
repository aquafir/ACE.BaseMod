namespace CustomLoot.Mutators;

public class Set : Mutator
{
    public override bool TryMutate(TreasureDeath treasureDeath, TreasureRoll treasureRoll, HashSet<Mutation> mutations, WorldObject item)
    {
        //Add a set from the valid ones in the settings
        item.RollEquipmentSet(treasureRoll);

        return true;
    }
}