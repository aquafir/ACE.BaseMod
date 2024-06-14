namespace Expansion.Features;

[CommandCategory(nameof(Feature.ItemLevelUpGrowth))]
[HarmonyPatchCategory(nameof(Feature.ItemLevelUpGrowth))]
public static class ItemLevelUpGrowth
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.OnItemLevelUp), new Type[] { typeof(WorldObject), typeof(int) })]
    public static void PreOnItemLevelUp(WorldObject item, int prevItemLevel, ref Player __instance)
    {
        //Skip items not made as growth items
        if (item.GetProperty(FakeBool.GrowthItem) is null)  //Will this ever by false?
            return;


        //Todo: WO->Original type
        var storedType = item.GetProperty(FakeInt.OriginalItemType);
        if (storedType is null) return;
        var itemType = (TreasureItemType_Orig)storedType;


        for (int level = prevItemLevel + 1; level <= item.ItemLevel; level++)
        {
            if (!item.TryGrowItem(level, itemType, __instance))
            {
                //Quit early?
                __instance.SendMessage($"Failed to apply Augment to {item.Name} for level {level}");
            }
        }

        //Return true to execute original
        return;
    }

    private static bool TryGrowItem(this WorldObject item, int level, TreasureItemType_Orig itemType, Player player)
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

        player.SendMessage($"Growing {item.Name} with {augment} for level {level}/{item.ItemMaxLevel}");
        return true;
    }
}
