namespace ACE.Shared.Mods;

public static class FeatureHelpers
{
    public static void RegisterFeatureEnumCommands<T>(this BasicMod mod, IEnumerable<T> features) where T : Enum
    {
        var commandRegex = String.Join("|", features);
        mod.Container.RegisterCommandCategory(commandRegex);
    }
    public static void UnregisterFeatureEnumCommands<T>(this BasicMod mod, IEnumerable<T> features) where T : Enum
    {
        var commandRegex = String.Join("|", features);
        mod.Container.UnregisterCommandCategory(commandRegex);
    }

    public static void RegisterFeatureEnumPatchCategories<T>(this BasicMod mod, IEnumerable<T> features) where T : Enum
    {
        foreach (var feature in features)
            mod.Harmony.PatchCategory(feature.ToString());
    }
    public static void UnregisterFeatureEnumPatchCategories<T>(this BasicMod mod, IEnumerable<T> features) where T : Enum
    {
        foreach (var feature in features)
            mod.Harmony.UnpatchCategory(feature.ToString());
    }
}
