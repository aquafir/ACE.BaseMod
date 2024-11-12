namespace Expansion.Mutators;
public class TowerLocked : Mutator
{
    public override bool CheckMutates(WorldObject item, int tier = 0)
    {
        return item.WeenieType switch
        {
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

    public override bool TryMutateEnterInventory(HashSet<Mutation> mutations, WorldObject item, Player player)
    {
        item.Bonded = BondedStatus.Bonded;
        item.Attuned = AttunedStatus.Attuned;
        return true;
    }
}
