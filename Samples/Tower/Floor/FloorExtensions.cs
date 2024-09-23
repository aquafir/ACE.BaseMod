namespace Tower;
public static class FloorExtensions
{
    static FloorSettings Settings => PatchClass.Settings.Tower;

    private static List<TowerFloor> Floors => Settings.Floors;
    private static Dictionary<ushort, TowerFloor> floorLookup = null;
    private static Dictionary<int, TowerFloor> floorIndexLookup = null;
    /// <summary>
    /// Called on plugin startup to populate a lookup for floors of the Tower
    /// </summary>
    public static void Init()
    {
        floorLookup = Settings.Floors.ToDictionary(x => x.Landblock, x => x);
        floorIndexLookup = Settings.Floors.ToDictionary(x => x.Index, x => x);
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
        Floors.OrderByDescending(x => x.Index).Where(x => player.GetFirstCompletionTime(x) != null).FirstOrDefault();


}
