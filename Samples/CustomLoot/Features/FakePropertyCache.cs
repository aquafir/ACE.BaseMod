namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakePropertyCache))]
public static class FakePropertyCache
{
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

    //FloatProps are doubles, ofc
    static readonly HashSet<FakeFloat> watchedFloats = new();
    static readonly Dictionary<Player, Dictionary<FakeFloat, double>> cachedFloats = new();

    /// <summary>
    /// Updates all cached properties
    /// </summary>
    public static void UpdateEquipmentCache(this Player player)
    {
        var cache = player.GetOrCreateFloatCache();

        foreach (var prop in watchedFloats)
        {
            double bonus = 0;
            foreach (var item in player.Inventory.Values)
            {
                bonus += item.GetProperty(prop) ?? 0;
            }
            player.SendMessage($"{prop} = {bonus}");
        }
    }

    public static double GetCachedFake(this Player player, FakeFloat prop)
    {
        var cache = player.GetOrCreateFloatCache();

        //Fetch or create
        if (!cache.TryGetValue(prop, out var value))
            value = player.EquipmentBonus(prop);

        //Watch property
        if(watchedFloats.Add(prop))
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

    //Todo: ask about efficiency of using linq?
    public static double EquipmentBonus(this Player player, FakeFloat prop) => player.EquippedObjects.Select(x => x.Value.GetProperty(prop)).Where(x => x.HasValue).Sum() ?? 0;
}
