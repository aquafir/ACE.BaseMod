namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakePropertyCache))]
public static class FakePropertyCache
{
    //Todo: add cleanup of players (logoff?) and props?
    #region Patches / Triggers of cache update
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), "AddItemToEquippedItemsRatingCache", new Type[] { typeof(WorldObject) })]
    public static void PreAddItemToEquippedItemsRatingCache(WorldObject wo, ref Creature __instance)
    {
        if (__instance is Player player)
            UpdateEquipmentCache(player);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), "RemoveItemFromEquippedItemsRatingCache", new Type[] { typeof(WorldObject) })]
    public static void PreRemoveItemFromEquippedItemsRatingCache(WorldObject wo, ref Creature __instance)
    {
        if (__instance is Player player)
            UpdateEquipmentCache(player);
    } 
    #endregion

    /// <summary>
    /// Updates all cached properties
    /// </summary>
    public static void UpdateEquipmentCache(this Player player)
    {
        var cache = player.GetOrCreateFloatCache();

        foreach (var prop in watchedFloats)
        {
            double bonus = 0;
            foreach (var item in player.EquippedObjects.Values)
            {
                bonus += item.GetProperty(prop) ?? 0;
                player.SendMessage($"{item.Name}: {prop}->{bonus}");
            }
            player.SendMessage($"{prop} = {bonus}");
        }
    }

    //FloatProps are doubles, ofc
    #region Float Cache/Helpers
    static readonly HashSet<FakeFloat> watchedFloats = new();
    static readonly Dictionary<Player, Dictionary<FakeFloat, double>> cachedFloats = new();
    public static double GetCachedFake(this Player player, FakeFloat prop)
    {
        var cache = player.GetOrCreateFloatCache();

        //Fetch or create
        if (!cache.TryGetValue(prop, out var value))
            value = player.EquipmentBonus(prop);

        //Watch property
        if (watchedFloats.Add(prop))
            player.SendMessage($"Added {prop} to cache: {value}");

        return value;
    }
    private static Dictionary<FakeFloat, double> GetOrCreateFloatCache(this Player player)
    {
        if (!cachedFloats.TryGetValue(player, out var cache))
        {
            cache = new();
            cachedFloats.Add(player, cache);

            player.SendMessage($"Created cache.");
        }

        return cache;
    }
    public static double EquipmentBonus(this Player player, FakeFloat prop) => player.EquippedObjects.Select(x => x.Value.GetProperty(prop)).Where(x => x.HasValue).Sum() ?? 0;
    #endregion
    #region Int Cache/Helpers
    static readonly HashSet<FakeInt> watchedInts = new();
    static readonly Dictionary<Player, Dictionary<FakeInt, int>> cachedInts = new();
    public static int GetCachedFake(this Player player, FakeInt prop)
    {
        var cache = player.GetOrCreateIntCache();

        //Fetch or create
        if (!cache.TryGetValue(prop, out var value))
            value = player.EquipmentBonus(prop);

        //Watch property
        if (watchedInts.Add(prop))
            player.SendMessage($"Added {prop} to cache: {value}");

        return value;
    }
    private static Dictionary<FakeInt, int> GetOrCreateIntCache(this Player player)
    {
        if (!cachedInts.TryGetValue(player, out var cache))
        {
            cache = new();
            cachedInts.Add(player, cache);

            player.SendMessage($"Created cache.");
        }

        return cache;
    }
    public static int EquipmentBonus(this Player player, FakeInt prop) => player.EquippedObjects.Select(x => x.Value.GetProperty(prop)).Where(x => x.HasValue).Sum() ?? 0;
    #endregion
    //#region Int64 Cache/Helpers
    //static readonly HashSet<FakeInt64> watchedInt64s = new();
    //static readonly Dictionary<Player, Dictionary<FakeInt64, long>> cachedInt64s = new();
    //public static long GetCachedFake(this Player player, FakeInt64 prop)
    //{
    //    var cache = player.GetOrCreateInt64Cache();

    //    //Fetch or create
    //    if (!cache.TryGetValue(prop, out var value))
    //        value = player.EquipmentBonus(prop);

    //    //Watch property
    //    if (watchedInt64s.Add(prop))
    //        player.SendMessage($"Added {prop} to cache: {value}");

    //    return value;
    //}
    //private static Dictionary<FakeInt64, long> GetOrCreateInt64Cache(this Player player)
    //{
    //    if (!cachedInt64s.TryGetValue(player, out var cache))
    //    {
    //        cache = new();
    //        cachedInt64s.Add(player, cache);

    //        player.SendMessage($"Created cache.");
    //    }

    //    return cache;
    //}
    //public static long EquipmentBonus(this Player player, FakeInt64 prop) => player.EquippedObjects.Select(x => x.Value.GetProperty(prop)).Where(x => x.HasValue).Sum() ?? 0;
    //#endregion

}
