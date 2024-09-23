namespace Expansion.Mutators;

[HarmonyPatchCategory(nameof(Mutation.Enlightened))]  //Mutator categories not used, just convenience for generating enum or a placeholder
public class Enlightened : Mutator
{
    public override bool TryMutateCorpse(HashSet<Mutation> mutations, Creature creature, DamageHistoryInfo killer, Corpse corpse, WorldObject item)
    {
        var tier = creature.GetProperty(FakeInt.DifficultyTier) ?? 0;
        if (tier < 6)
            return false;

        //Get a tier
        var muts = tier - 6;

        //Mutate a number of times
        for (var i = 0; i < muts; i++)
        {
            bool success = item.WeenieType switch
            {
                WeenieType.MeleeWeapon => TryGrowItem(item, i, TreasureItemType_Orig.Weapon),
                WeenieType.MissileLauncher => TryGrowItem(item, i, TreasureItemType_Orig.Weapon),
                WeenieType.Caster => TryGrowItem(item, i, TreasureItemType_Orig.Weapon),
                WeenieType.Clothing => TryGrowItem(item, i, TreasureItemType_Orig.Armor),
                _ => false,
            };

            //Maybe?
            if (!success)
                return false;
        }

        //Set name/wield reqs, not a great way of doing this
        item.Name += $" ({muts})";
        item.SetProperty(FakeInt.ItemWieldRequirementEnlightenments, muts * 2);

        return true;
    }

    //Add prop to creatures?
    public override bool TryMutateEnterWorld(HashSet<Mutation> mutations, WorldObject item)
    {
        if (item is not Creature creature)
            return false;

        //Set creature tier?
        creature.SetProperty(FakeInt.DifficultyTier, (creature.Level ?? 0) / 50);

        return true;
    }

    //To be changed, borrowed from growth
    private static bool TryGrowItem(WorldObject item, int level, TreasureItemType_Orig itemType)
    {
        //Try to get an augment for level, checking for fixed level then pool
        Augment augment = 0;
        //if (!S.Settings.GrowthFixedLevelAugments.TryGetValue(itemType, out var levelAugments) || !levelAugments.TryGetValue(level, out augment))
        if (!S.Settings.GrowthFixedLevelAugments.TryGetValue(item.WeenieType, out var levelAugments) || !levelAugments.TryGetValue(level, out augment))
        {
            //if (!S.Settings.GrowthAugments.TryGetValue(itemType, out var augmentGroup))
            if (!S.Settings.GrowthAugments.TryGetValue(item.WeenieType, out var augmentGroup))
                return false;

            if (!S.Settings.AugmentGroups.TryGetValue(augmentGroup, out var augmentPool))
                return false;

            if (!augmentPool.TryGetRandom(out augment))
                return false;
        }

        //Apply
        if (!item.TryAugmentWith(augment))
            return false;

        return true;
    }
}
