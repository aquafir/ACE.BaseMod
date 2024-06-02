namespace Expansion.Mutators;
internal class TowerLocked : Mutator
{
    //Apply to corpses with an Ironman killer
    //public override bool CanMutateCorpse(DamageHistoryInfo killer, Corpse corpse, Creature creature)
    //{
    //    if (killer.TryGetPetOwnerOrAttacker() is Player player && (player.GetProperty(FakeBool.Ironman) ?? false))
    //        return true;

    //    return false;
    //}

    ////Add Ironman
    //public override bool TryMutateCorpse(HashSet<Mutation> mutations, Creature creature, DamageHistoryInfo killer, Corpse corpse, WorldObject item)
    //{
    //    item.SetProperty(FakeBool.Ironman, true);

    //    return true;
    //}

    public override bool CheckMutates(WorldObject item, int tier = 0)
    {
        return item.WeenieType switch  {
            WeenieType.Ammunition => true,
            WeenieType.Caster => true,
            WeenieType.Clothing => true,
            WeenieType.MeleeWeapon => true,
            WeenieType.Missile => true,
            WeenieType.MissileLauncher => true,
            WeenieType.PetDevice => true,
            WeenieType.Scroll => true,
            WeenieType.SpellComponent => true,
            //WeenieType.Scroll => true,
            //WeenieType.Scroll => true,
            _ => false,
            };
    }

    public override bool TryMutateEnterWorld(HashSet<Mutation> mutations, WorldObject item)
    {
        item.Bonded = BondedStatus.Bonded;
        item.Attuned = AttunedStatus.Attuned;
        return true;
    }
}
