namespace Tower.Speedrun;
public static class FloorExtensions
{
    private static List<TowerFloor> Floors => PatchClass.Settings.Floors;
    private static Dictionary<ushort, TowerFloor> floorLookup = null;
    private static Dictionary<int, TowerFloor> floorIndexLookup = null;
    /// <summary>
    /// Called on plugin startup to populate a lookup for floors of the Tower
    /// </summary>
    public static void Init()
    {
        floorLookup = PatchClass.Settings.Floors.ToDictionary(x => x.Landblock, x => x);
        floorIndexLookup = PatchClass.Settings.Floors.ToDictionary(x => x.Index, x => x);
    }
    /// <summary>
    /// Try to find the floor a player is on
    /// </summary>
    public static bool TryGetFloor(this Player player, out TowerFloor floor) =>
        floorLookup.TryGetValue(player.CurrentLandblock.Id.Landblock, out floor);

    public static bool TryGetFloor(this Player player, int index, out TowerFloor floor) =>
        floorIndexLookup.TryGetValue(index, out floor);

    public static bool TryGetFloorByIndex(this int index, out TowerFloor floor) =>
        floorIndexLookup.TryGetValue(index, out floor);

    public static bool TryGetNextFloor(this TowerFloor current, out TowerFloor floor)
    {
        floor = null;
        return (current.Index + 1).TryGetFloorByIndex(out floor);
    }
    public static bool TryGetNextFloor(this Player player, out TowerFloor floor)
    {
        floor = null;
        if (player.TryGetFloor(out var current))
            return (floor.Index + 1).TryGetFloorByIndex(out floor);

        return false;
    }

    public static TowerFloor? GetHighestCompletedFloor(this Player player) =>
        Floors.OrderByDescending(x => x.Index).Where(x => player.GetFirstCompletion(x) != null).FirstOrDefault();

    public static float GetXpBonus(this Player player)
    {
        //Todo: decide about setting on portal
        if (!player.TryGetFloor(out var floor))
            return 1f;

        var delta = floor.Level - (player.Level ?? 1);
        if (delta == 0)
            return 1f;

        var fractionMaxBonus = delta / PatchClass.Settings.MaxXpBonusLevelRange;
        var bonus = PatchClass.Settings.MaxXpBonus * fractionMaxBonus;
        //player.SendMessage($"{delta} levels below target of {floor.Level} - {fractionMaxBonus} of max");
        return Math.Clamp(bonus, 1, PatchClass.Settings.MaxXpBonus);
    }
    public static float GetLootBonus(this Player player)
    {
        //Todo: decide about setting on portal
        if (!player.TryGetFloor(out var floor))
            return 1f;

        var delta = floor.Level - (player.Level ?? 1);
        if (delta == 0)
            return 1f;

        var fractionMaxBonus = delta / PatchClass.Settings.MaxLootBonusLevelRange;
        var bonus = PatchClass.Settings.MaxLootBonus * fractionMaxBonus;
        //player.SendMessage($"{delta} levels below target of {floor.Level} - {fractionMaxBonus} of max");
        return Math.Clamp(bonus, 1, PatchClass.Settings.MaxLootBonus);
    }
}
