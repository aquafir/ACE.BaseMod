namespace Expansion.Mutators;
internal class IronmanLocked : Mutator
{
    //Apply to corpses with an Ironman killer
    public override bool CanMutateCorpse(DamageHistoryInfo killer, Corpse corpse, Creature creature)
    {
        if (killer.TryGetPetOwnerOrAttacker() is Player player && (player.GetProperty(FakeBool.Ironman) ?? false))
            return true;

        return false;
    }

    //Add Ironman
    public override bool TryMutateCorpse(HashSet<Mutation> mutations, Creature creature, DamageHistoryInfo killer, Corpse corpse, WorldObject item)
    {
        item.SetProperty(FakeBool.Ironman, true);

        return true;
    }
}
