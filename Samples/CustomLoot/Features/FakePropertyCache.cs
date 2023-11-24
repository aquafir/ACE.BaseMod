using CustomLoot.Enums;
using System.Net.Http.Headers;

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
            UpdateEquipmentCache(player, wo, true, true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), "RemoveItemFromEquippedItemsRatingCache", new Type[] { typeof(WorldObject) })]
    public static void PreRemoveItemFromEquippedItemsRatingCache(WorldObject wo, ref Creature __instance)
    {
        if (__instance is Player player)
            UpdateEquipmentCache(player, wo, false, true);
    } 
    #endregion


    /// <summary>
    /// Updates all cached properties
    /// </summary>
    public static void UpdateEquipmentCache(this Player player, WorldObject wo, bool equipping, bool full = false)
    {
        var floatCache = player.GetOrCreateFloatCache();

        player.SendMessage($"{(equipping ? "Equipping" : "Unequipping")} {wo.Name}: ");

        //For each type of prop
        player.SendMessage($"  =====FakeFloat ({watchedFloats.Count} watched)====");
        foreach (var prop in watchedFloats)
        {
            //Get value, add to cache if missing
            if (!floatCache.TryGetValue(prop, out var cacheValue))
            {
                floatCache.Add(prop, 0);
                cacheValue = 0;
            }

            var items = full ? player.EquippedObjects.Values : new List<WorldObject>().AddItem(wo);
            foreach (var item in items)
            {
                var itemValue = item.GetProperty(prop) ?? 0;
                cacheValue += equipping ? itemValue : -itemValue;
                player.SendMessage($"    {item.Name}.{prop}: {cacheValue:0.00} + {itemValue:0.00} -> {floatCache[prop]:0.00}");
            }

            floatCache[prop] = cacheValue;
        }

        var intCache = player.GetOrCreateIntCache();
        player.SendMessage($"  =====FakeInt ({watchedInts.Count} watched)====");
        foreach (var prop in watchedInts)
        {
            //Get value, add to cache if missing
            if (!intCache.TryGetValue(prop, out var cacheValue))
            {
                intCache.Add(prop, 0);
                cacheValue = 0;
            }

            var items = full ? player.EquippedObjects.Values : new List<WorldObject>().AddItem(wo);
            foreach (var item in items)
            {
                var itemValue = item.GetProperty(prop) ?? 0;
                cacheValue += equipping ? itemValue : -itemValue;
                player.SendMessage($"    {item.Name}.{prop}: {cacheValue} + {itemValue} -> {intCache[prop]}");
            }

            intCache[prop] = cacheValue;
        }
    }

    #region Abandoned Generic Update
    //public static void UpdateEquipmentCache<T, V>(this Player player, WorldObject wo, bool equipping, Dictionary<T, V> cache, List<T> watched, bool full = false)
    //{
    //    player.SendMessage($"{(equipping ? "Equipping" : "Unequipping")} {wo.Name}: ");

    //    //For each type of prop
    //    var valueType = typeof(V);
    //    player.SendMessage($"  ====={typeof(T).Name} ({watched.Count} watched)====");
    //    foreach (var prop in watched)
    //    {
    //        //Get value, add to cache if missing
    //        if (!cache.TryGetValue(prop, out var cacheValue))
    //        {
    //            cache.Add(prop, (V)Convert.ChangeType(0, valueType));
    //            cacheValue = (V)Convert.ChangeType(0, valueType);
    //        }

    //        var itemValue = wo.GetProperty(prop) ?? 0;
    //        cache[prop] = equipping ? cacheValue + itemValue : cacheValue - itemValue;
    //        //cache[prop] = newValue;
    //        player.SendMessage($"  {prop}: {cacheValue} + {itemValue} -> {cache[prop]}");

    //        //Full rebuild?
    //        //foreach (var item in player.EquippedObjects.Values)
    //    }
    //} 
    #endregion

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
