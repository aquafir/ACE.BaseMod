using ACE.Server.Command.Handlers;

namespace Expansion.Features;

//[CommandCategory(nameof(Feature.FakePropertyCache))]
[HarmonyPatchCategory(nameof(Feature.FakePropertyCache))]
public static class FakePropertyCache
{
    //Todo: add cleanup of players (logoff?) and props?
    #region Patches / Triggers of cache update
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.AddItemToEquippedItemsRatingCache), new Type[] { typeof(WorldObject) })]
    public static void PreAddItemToEquippedItemsRatingCache(WorldObject wo, ref Creature __instance)
    {
        if (__instance is Player player)
        {
            player.UpdateEquipmentCache();
            //UpdateItem(player, wo, true);
            //player.SendMessage($"\nEquipped {wo.Name}:\n{wo.DumpItem()}"); 
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.RemoveItemFromEquippedItemsRatingCache), new Type[] { typeof(WorldObject) })]
    public static void PreRemoveItemFromEquippedItemsRatingCache(WorldObject wo, ref Creature __instance)
    {
        if (__instance is Player player)
        {
            player.UpdateEquipmentCache();
            //UpdateItem(player, wo, true);
            //player.SendMessage($"\nUnequipped {wo.Name}:\n{wo.DumpItem()}"); 
        }
    }
    #endregion

    /// <summary>
    /// Updates all cached properties
    /// </summary>
    public static void UpdateEquipmentCache(this Player player)
    {
        //player.SendMessage($"Rebuilding equipment cache.");

        //Clear caches to rebuild instead of just 
        var floatCache = player.GetOrCreateFloatCache();
        floatCache.Clear();

        var intCache = player.GetOrCreateIntCache();
        intCache.Clear();

        foreach (var item in player.EquippedObjects.Values)
            player.UpdateItem(item);

        //Clamp after or during?
        foreach (var watched in watchedFloats)
        {
            if (S.Settings.BonusCaps.MinFloat.TryGetValue(watched, out var min) &&
                floatCache.TryGetValue(watched, out var value))
            {
                //player.SendMessage($"min {watched} from {value} to {min}");
                floatCache[watched] = Math.Max(value, min);
            }

            if (S.Settings.BonusCaps.MaxFloat.TryGetValue(watched, out var max) &&
                floatCache.TryGetValue(watched, out value))
            {
                //player.SendMessage($"Max {watched} from {value} to {max}");
                floatCache[watched] = Math.Min(value, max);
            }
        }
        foreach (var watched in watchedInts)
        {
            if (S.Settings.BonusCaps.MinInt.TryGetValue(watched, out var min) &&
                intCache.TryGetValue(watched, out var value))
                intCache[watched] = Math.Max(value, min);

            if (S.Settings.BonusCaps.MaxInt.TryGetValue(watched, out var max) &&
                intCache.TryGetValue(watched, out value))
                intCache[watched] = Math.Min(value, max);
        }
    }
    //todo
    public static void UpdateItem(this Player player, WorldObject item, bool equipping = true)
    {
        var floatCache = player.GetOrCreateFloatCache();

        // player.SendMessage($"{(equipping ? "Equipping" : "Unequipping")} {item.Name}: ");

        //For each type of prop
        //player.SendMessage($"  =====FakeFloat ({watchedFloats.Count} watched)====");
        foreach (var prop in watchedFloats)
        {
            //Get value, add to cache if missing
            if (!floatCache.TryGetValue(prop, out var cacheValue))
            {
                floatCache.Add(prop, 0);
                cacheValue = 0;
            }

            var itemValue = item.GetProperty(prop) ?? 0;
            cacheValue += equipping ? itemValue : -itemValue;
            // player.SendMessage($"    {item.Name}.{prop}: {cacheValue:0.00} + {itemValue:0.00} -> {floatCache[prop]:0.00}");

            floatCache[prop] = cacheValue;
        }

        var intCache = player.GetOrCreateIntCache();
        //player.SendMessage($"  =====FakeInt ({watchedInts.Count} watched)====");
        foreach (var prop in watchedInts)
        {
            //Get value, add to cache if missing
            if (!intCache.TryGetValue(prop, out var cacheValue))
            {
                intCache.Add(prop, 0);
                cacheValue = 0;
            }

            var itemValue = item.GetProperty(prop) ?? 0;
            cacheValue += equipping ? itemValue : -itemValue;
            //  player.SendMessage($"    {item.Name}.{prop}: {cacheValue:0.00} + {itemValue:0.00} -> {intCache[prop]:0.00}");

            intCache[prop] = cacheValue;
        }
    }

    //FloatProps are doubles, ofc
    #region Float Cache/Helpers
    static readonly HashSet<FakeFloat> watchedFloats = new();
    static readonly Dictionary<Player, Dictionary<FakeFloat, double>> cachedFloats = new();
    public static double GetCachedFake(this Player player, FakeFloat prop)
    {
        //var watch = Stopwatch.StartNew();
        var cache = player.GetOrCreateFloatCache();

        //Fetch or create
        if (!cache.TryGetValue(prop, out var value))
        {
            //value = player.EquipmentBonus(prop);
            //Update and try again
            player.UpdateEquipmentCache();
            value = cache.TryGetValue(prop, out value) ? value : 0;
        }

        //Watch property
        if (watchedFloats.Add(prop))
        { }
        //player.SendMessage($"Added {prop} to cache: {value}");

        //watch.Stop();
        //player.SendMessage($"Fetched in {watch.ElapsedTicks} ticks / {watch.ElapsedMilliseconds} ms.");
        return value;
    }
    private static Dictionary<FakeFloat, double> GetOrCreateFloatCache(this Player player)
    {
        if (!cachedFloats.TryGetValue(player, out var cache))
        {
            cache = new();
            cachedFloats.Add(player, cache);

            //player.SendMessage($"Created cache.");
        }

        return cache;
    }
    //public static double EquipmentBonus(this Player player, FakeFloat prop) => player.EquippedObjects.Select(x => x.Value.GetProperty(prop)).Where(x => x.HasValue).Sum() ?? 0;
    #endregion
    #region Int Cache/Helpers
    static readonly HashSet<FakeInt> watchedInts = new();
    static readonly Dictionary<Player, Dictionary<FakeInt, int>> cachedInts = new();
    public static int GetCachedFake(this Player player, FakeInt prop)
    {
        var cache = player.GetOrCreateIntCache();

        //Fetch or create
        if (!cache.TryGetValue(prop, out var value))
        {
            //value = player.EquipmentBonus(prop);
            //Update and try again
            player.UpdateEquipmentCache();
            value = cache.TryGetValue(prop, out value) ? value : 0;
        }

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

            //player.SendMessage($"Created cache.");
        }

        return cache;
    }
    //public static int EquipmentBonus(this Player player, FakeInt prop) => player.EquippedObjects.Select(x => x.Value.GetProperty(prop)).Where(x => x.HasValue).Sum() ?? 0;
    #endregion

    #region Commands / Dumps -- /ecXX
    [CommandHandler("ecdw", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void WatchedDump(Session session, params string[] parameters)
    {
        var player = session.Player;

        var dump = $"\nFloats: {string.Join(", ", watchedFloats)}\nInts: {string.Join(", ", watchedInts)}";

        player.SendMessage(dump);
    }
    [CommandHandler("ecdc", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void PlayerCacheDump(Session session, params string[] parameters)
    {
        var player = session.Player;
        player.SendMessage(player.DumpPlayerCache());
    }
    [CommandHandler("ecde", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void PlayerEquipmentDump(Session session, params string[] parameters)
    {
        var player = session.Player;
        var dump = player.DumpPlayerItems();
        player.SendMessage(dump);
    }
    [CommandHandler("ecdi", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void ItemDump(Session session, params string[] parameters)
    {
        var player = session.Player;
        var target = CommandHandlerHelper.GetLastAppraisedObject(session);

        if (target is null)
            return;

        player.SendMessage(target.DumpItem());
    }
    #endregion

    public static string DumpPlayerItems(this Player player)
    {
        var sb = new StringBuilder();
        foreach (var item in player.EquippedObjects.Values)
        {
            var dump = item.DumpItem();

            if (dump.Length > 0)
            {
                sb.Append($"============={item.Name}=============\n{dump}\n");
            }
        }
        return sb.ToString();
    }
    public static string DumpItem(this WorldObject item)
    {
        var sb = new StringBuilder();
        sb.Append(item.DumpItemFloats());
        sb.Append(item.DumpItemInts());

        return sb.ToString();
    }
    public static string DumpItemFloats(this WorldObject item)
    {
        if (watchedFloats.Count == 0) return "";

        int count = 0;
        var sb = new StringBuilder($"======={typeof(FakeFloat).Name} ({watchedFloats.Count} watched)=======\n");
        foreach (var prop in watchedFloats)
        {
            var value = item.GetProperty(prop) ?? 0;
            if (value == 0) continue;

            count++;
            sb.Append($"  {prop} = {value:0.00}\n");
        }
        if (count == 0) return "";

        return sb.ToString();
    }
    public static string DumpItemInts(this WorldObject item)
    {
        if (watchedInts.Count == 0) return "";

        int count = 0;
        var sb = new StringBuilder($"======={typeof(FakeInt).Name} ({watchedInts.Count} watched)=======\n");
        foreach (var prop in watchedInts)
        {
            var value = item.GetProperty(prop) ?? 0;
            if (value == 0) continue;

            count++;
            sb.Append($"  {prop} = {value:0.00}\n");
        }
        if (count == 0) return "";

        return sb.ToString();
    }
    public static string DumpPlayerCache(this Player player)
    {
        var floatCache = player.GetOrCreateFloatCache();
        var intCache = player.GetOrCreateIntCache();
        //For each prop in a list, get each value in a dictionary
        var dump = new StringBuilder();
        dump.Append(floatCache.DumpPlayerPropertyCache());
        dump.Append(intCache.DumpPlayerPropertyCache());

        return dump.ToString();
    }
    //Only one I could make generic with the GetProperty requirement
    public static string DumpPlayerPropertyCache<T, E>(this Dictionary<T, E> cache)
    {
        if (cache.Count == 0) return "";

        var sb = new StringBuilder($"======={typeof(T).Name} ({cache.Count} watched)=======\n");
        foreach (var kvp in cache)
        {
            if (Convert.ToUInt32(kvp.Value) != 0)
                //Conditional formatting?
                sb.Append($"  {kvp.Key.ToString()} = {kvp.Value:0.00}\n");
        }
        return sb.ToString();
    }
}
