

namespace CustomLoot.Mutators;
internal class LocationLocked : Mutator
{
    //public override bool MutatesCorpse(HashSet<Mutation> mutations, Creature creature = null, DamageHistoryInfo killer = null, Corpse corpse = null, WorldObject item = null)
    //{
    //    //Relies on CorpseInfo - Check dungeon first?
    //    var lb = item.GetProperty(FakeBool.CorpseSpawnedDungeon);
    //    if (!(item.GetProperty(FakeBool.CorpseSpawnedDungeon) ?? false))
    //        return false;

    //    return base.MutatesLoot(mutations, profile, roll, item);
    //}

    public override bool MutatesGenerator(HashSet<Mutation> mutations, GeneratorProfile profile = null, WorldObject item = null)
    {
        if (profile.RegenLocationType != RegenLocationType.Treasure)
            return false;

        return base.MutatesGenerator(mutations, profile, item);
    }

    public override bool TryMutateCorpse(HashSet<Mutation> mutations, Creature creature, DamageHistoryInfo killer, Corpse corpse, WorldObject item)
    {
        return TryMutate(mutations, item, creature.CurrentLandblock);
    }

    public override bool TryMutateGenerator(HashSet<Mutation> mutations, GeneratorProfile generator, WorldObject item)
    {
        Debugger.Break();

        return TryMutate(mutations, item, generator.Generator.CurrentLandblock);
    }

    public bool TryMutate(HashSet<Mutation> mutations, WorldObject item, Landblock lb)
    {
        if (lb is null)// || !lb.IsDungeon)
            return false;

        var id = lb.Id.Raw;
        item.SetProperty(FakeDID.LocationLockId, id);

#if DEBUG
        ModManager.Log($"{item.Name} is locked to {id}");
#endif

        return true;
    }
}
