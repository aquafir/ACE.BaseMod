namespace SelectiveStartup;

[HarmonyPatchCategory(nameof(ServerTask.PropertyManager))]
public static class PropertyPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PropertyManager), nameof(PropertyManager.GetBool), new Type[] { typeof(string), typeof(bool), typeof(bool) })]
    public static bool PreGetBool(string key, bool fallback, bool cacheFallback, ref Property<bool> __result)
    {
        __result = new();
        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PropertyManager), nameof(PropertyManager.GetLong), new Type[] { typeof(string), typeof(long), typeof(bool) })]
    public static bool PreGetLong(string key, long fallback, bool cacheFallback, ref Property<long> __result)
    {
        __result = new();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PropertyManager), nameof(PropertyManager.GetDouble), new Type[] { typeof(string), typeof(double), typeof(bool) })]
    public static bool PreGetDouble(string key, double fallback, bool cacheFallback, ref Property<double> __result)
    {
        __result = new();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PropertyManager), nameof(PropertyManager.GetString), new Type[] { typeof(string), typeof(string), typeof(bool) })]
    public static bool PreGetString(string key, string fallback, bool cacheFallback, ref Property<string> __result)
    {
        __result = new();
        return false;
    }

}