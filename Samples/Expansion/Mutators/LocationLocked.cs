namespace Expansion.Mutators;
internal class LocationLocked : Mutator
{
    //Corpse/Generators check landblock and other relevant things first to skip collection evaluation
    public override bool CanMutateGenerator(GeneratorProfile profile)
    {
        if (profile is null)
            return false;

        if (!profile.Generator.CurrentLandblock.IsDungeon)
            return false;

        //Must be a treasure
        if (!profile.RegenLocationType.HasAny(RegenLocationType.Treasure))
            return false;

        return base.CanMutateGenerator(profile);
    }
    public override bool CanMutateCorpse(DamageHistoryInfo killer, Corpse corpse, Creature creature)
    {
        if (creature is null)
            return false;

        if (!creature.CurrentLandblock.IsDungeon)
            return false;

        if (!killer.IsPlayer)
            return false;

        return base.CanMutateCorpse(killer, corpse, creature);
    }

    public override bool TryMutateCorpse(HashSet<Mutation> mutations, Creature creature, DamageHistoryInfo killer, Corpse corpse, WorldObject item)
    {
        return TryMutate(mutations, item, creature.CurrentLandblock);
    }
    public override bool TryMutateGenerator(HashSet<Mutation> mutations, GeneratorProfile generator, WorldObject item)
    {
        return TryMutate(mutations, item, generator.Generator.CurrentLandblock);
    }

    public bool TryMutate(HashSet<Mutation> mutations, WorldObject item, Landblock lb)
    {
        var id = lb.Id.Raw;
        item.SetProperty(FakeDID.LocationLockId, id);

#if DEBUG
        ModManager.Log($"{item.Name} is locked to {id}");
#endif

        return true;
    }
}
