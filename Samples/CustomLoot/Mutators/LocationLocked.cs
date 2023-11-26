namespace CustomLoot.Mutators;
internal class LocationLocked : Mutator
{
    public override bool MutatesLoot(HashSet<Mutation> mutations, TreasureDeath profile, TreasureRoll roll, WorldObject item = null)
    {
        //Relies on CorpseInfo - Check dungeon first?
        var lb = item.GetProperty(FakeBool.CorpseSpawnedDungeon);
        if (!(item.GetProperty(FakeBool.CorpseSpawnedDungeon) ?? false))
            return false;
        
        return base.MutatesLoot(mutations, profile, roll, item);
    }

    public override bool TryMutateLoot(HashSet<Mutation> mutations, TreasureDeath profile, TreasureRoll roll, WorldObject item)
    {
        //If it mutates it should have lb/dungeon
        var lb = item.GetProperty(FakeDID.CorpseLandblockId);
        item.SetProperty(FakeDID.LocationLockId, item.GetProperty(FakeDID.CorpseLandblockId) ?? 0);
        var foo = item.CurrentLandblock;
        Debugger.Break();

        return true;
    }
}
