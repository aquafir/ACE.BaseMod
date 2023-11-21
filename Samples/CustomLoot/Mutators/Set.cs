namespace CustomLoot.Mutators;

internal static class Set
{
    public static void HandleSetMutation(TreasureDeath treasureDeath, TreasureRoll treasureRoll, WorldObject __result)
    {
        //Missing or empty set doesn't roll
        if (!PatchClass.Settings.CustomSets.TryGetValue(treasureRoll.ItemType, out var setList) || setList.Count == 0)
            return;

        //Add a set
        __result.RollEquipmentSet(treasureRoll);
    }
}