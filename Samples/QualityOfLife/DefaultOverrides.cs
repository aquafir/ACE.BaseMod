using ACE.Entity.Enum.Properties;

namespace QualityOfLife;

[HarmonyPatchCategory(nameof(DefaultOverrides))]
internal static class DefaultOverrides
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateProperty), new Type[] { typeof(WorldObject), typeof(PropertyInt), typeof(int?), typeof(bool) })]
    public static void PreUpdateProperty(WorldObject obj, PropertyInt prop, int? value, bool broadcast, ref Player __instance)
    {
        if (value is null || __instance.GetProperty(prop) is not null || !PatchClass.Settings.IntDefaults.TryGetValue(prop, out var defaultValue))
            return;

        value = defaultValue;

    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateProperty), new Type[] { typeof(WorldObject), typeof(PropertyBool), typeof(bool?), typeof(bool) })]
    public static void PreUpdateProperty(WorldObject obj, PropertyBool prop, bool? value, bool broadcast, ref Player __instance)
    {
        if (value is null || __instance.GetProperty(prop) is not null || !PatchClass.Settings.BoolDefaults.TryGetValue(prop, out var defaultValue))
            return;

        value = defaultValue;

    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateProperty), new Type[] { typeof(WorldObject), typeof(PropertyFloat), typeof(double?), typeof(bool) })]
    public static void PreUpdateProperty(WorldObject obj, PropertyFloat prop, double? value, bool broadcast, ref Player __instance)
    {
        if (value is null || __instance.GetProperty(prop) is not null || !PatchClass.Settings.FloatDefaults.TryGetValue(prop, out var defaultValue))
            return;

        value = defaultValue;

    }

    #region Unused Properties
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), nameof(Player.UpdateProperty), new Type[] { typeof(WorldObject), typeof(PropertyInstanceId), typeof(uint?), typeof(bool) })]
    //public static bool PreUpdateProperty(WorldObject obj, PropertyInstanceId prop, uint? value, bool broadcast, ref Player __instance)
    //{
    //    //Return false to override
    //    //return false;

    //    //Return true to execute original
    //    return true;
    //}

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), nameof(Player.UpdateProperty), new Type[] { typeof(WorldObject), typeof(PropertyString), typeof(string), typeof(bool) })]
    //public static bool PreUpdateProperty(WorldObject obj, PropertyString prop, string value, bool broadcast, ref Player __instance)
    //{
    //    //Return false to override
    //    //return false;

    //    //Return true to execute original
    //    return true;
    //}

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), nameof(Player.UpdateProperty), new Type[] { typeof(WorldObject), typeof(PropertyDataId), typeof(uint?), typeof(bool) })]
    //public static bool PreUpdateProperty(WorldObject obj, PropertyDataId prop, uint? value, bool broadcast, ref Player __instance)
    //{
    //    //Return false to override
    //    //return false;

    //    //Return true to execute original
    //    return true;
    //} 
    #endregion

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateProperty), new Type[] { typeof(WorldObject), typeof(PropertyInt64), typeof(long?), typeof(bool) })]
    public static void PreUpdateProperty(WorldObject obj, PropertyInt64 prop, ref long? value, bool broadcast, ref Player __instance)
    {
        if (value is null || __instance.GetProperty(prop) is not null || !PatchClass.Settings.Int64Defaults.TryGetValue(prop, out var defaultValue))
            return;

        value = defaultValue;
    }
}