namespace Expansion.Mutators;

public class Set : Mutator
{
    readonly Dictionary<TreasureItemType_Orig, EquipmentSet[]> sets = new();

    public override bool TryMutateLoot(HashSet<Mutation> mutations, TreasureDeath profile, TreasureRoll roll, WorldObject item)
    {
        //Add a set from the valid ones in the settings
        if (!sets.TryGetValue(roll.ItemType, out var setGroup))
            return false; //Remove if missing?

        if (!setGroup.TryGetRandom(out var set))
            return false;

        item.EquipmentSetId = set;

        return true;
    }

    public override void Start()
    {
        foreach (var kvp in PatchClass.Settings.ItemTypeEquipmentSets)
        {
            //Skip equipment group names that have no group
            if (!PatchClass.Settings.EquipmentSetGroups.TryGetValue(kvp.Key.ToString(), out var setGroup))
                continue;

            if (!sets.TryAdd(kvp.Key, setGroup) && PatchClass.Settings.Verbose)
                ModManager.Log($"Failed to add Set mutator for {kvp.Key}");
        }
    }
}