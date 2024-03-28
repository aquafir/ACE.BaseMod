namespace Expansion.Mutators;
internal class GrowthItem : Mutator
{
    public override bool TryMutateLoot(HashSet<Mutation> mutations, TreasureDeath profile, TreasureRoll roll, WorldObject item)
    {
        if (item.HasItemLevel)
            return false;

        //Only the unmutated?
        if (mutations.Count > 0)
            return false;

        //Get level range.  Validate?
        if (!S.Settings.GrowthTierLevelRange.TryGetValue(profile.Tier, out var levelRange))
            return false;

        //Item costs
        //if (!S.Settings.GrowthTierXpCost.TryGetValue(profile.Tier, out var xpCost))
        //    return false;
        var xpCost = (long)(S.Settings.GrowthXpBase * Math.Pow(S.Settings.GrowthXpScaleByTier, profile.Tier - 1));

        item.ItemXpStyle = ItemXpStyle.ScalesWithLevel;
        item.ItemTotalXp = 0;
        item.ItemMaxLevel = levelRange.Roll();
        item.ItemBaseXp = xpCost;

        //Store item tier
        item.SetProperty(FakeBool.GrowthItem, true);
        item.SetProperty(FakeInt.GrowthTier, profile.Tier);
        item.SetProperty(FakeInt.OriginalItemType, (int)roll.ItemType);

        return true;
    }
}
