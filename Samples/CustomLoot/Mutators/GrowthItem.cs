namespace CustomLoot.Mutators;
internal class GrowthItem : Mutator
{
    public override bool TryMutate(TreasureDeath profile, TreasureRoll roll, HashSet<Mutation> mutations, WorldObject item)
    {
        if (item.HasItemLevel)
            return false;

        //Only the unmutated?
        if (mutations.Count > 0)
            return false;

        //if (item.ItemType != ItemType.Weapon)
        //    return false;

//        item.Name = "Test";
        //Todo: more interesting things with item levels
        item.ItemMaxLevel = 50 * profile.Tier;
        item.ItemBaseXp = (int)(Math.Pow(2, profile.Tier) * 1_000_000); 
        item.ItemXpStyle = ItemXpStyle.ScalesWithLevel;
        item.ItemTotalXp = 0;

        //Store item tier
        item.SetProperty(FakeBool.GrowthItem, true);
        item.SetProperty(FakeInt.GrowthTier, profile.Tier);

        return true;
    }
}
