namespace ACE.Shared.Mods;

public static class FeatureHelpers
{
    /// <summary>
    /// Registers all commands contained in a collection
    /// </summary>
    public static void RegisterCommands<T>(this BasicMod mod, IEnumerable<T> features) //where T : Enum
    {
        var commandRegex = String.Join("|", features.Select(x => x.ToString()));
        mod.Container.RegisterCommandCategory(commandRegex);
    }
    /// <summary>
    /// Removes all commands contained in a collection
    /// </summary>
    public static void UnregisterCommands<T>(this BasicMod mod, IEnumerable<T> features) //where T : Enum
    {
        var commandRegex = String.Join("|", features.Select(x => x.ToString()));
        mod.Container.UnregisterCommandCategory(commandRegex);
    }

    /// <summary>
    /// Patches all categories contained in a collection
    /// </summary>
    public static void RegisterPatchCategories<T>(this BasicMod mod, IEnumerable<T> features) //where T : Enum
    {
        var assembly = mod.Container.ModAssembly;

        foreach (var feature in features)
            mod.Harmony.PatchCategory(assembly, feature.ToString());
    }
    /// <summary>
    /// Unpatches all categories contained in a collection
    /// </summary>
    public static void UnregisterPatchCategories<T>(this BasicMod mod, IEnumerable<T> features) //where T : Enum
    {
        var assembly = mod.Container.ModAssembly;

        foreach (var feature in features)
            mod.Harmony.UnpatchCategory(assembly, feature.ToString());
    }

    ///// <summary>
    ///// Registers all commands contained in a collection
    ///// </summary>
    //public static void RegisterCommandsTower(this BasicMod mod, params object[] features)
    //{
    //    var commandRegex = String.Join("|", features.ToString());
    //    mod.Container.RegisterCommandCategory(commandRegex);
    //}
    ///// <summary>
    ///// Removes all commands contained in a collection
    ///// </summary>
    //public static void UnregisterCommandsTower(this BasicMod mod, params object[] features)
    //{
    //    var commandRegex = String.Join("|", features);
    //    mod.Container.UnregisterCommandCategory(commandRegex);
    //}

    ///// <summary>
    ///// Patches all categories contained in a collection
    ///// </summary>
    //public static void RegisterPatchCategoriesTower(this BasicMod mod, params object[] features)
    //{
    //    foreach (var feature in features)
    //        mod.Harmony.PatchCategory(feature.ToString());
    //}
    ///// <summary>
    ///// Unpatches all categories contained in a collection
    ///// </summary>
    //public static void UnregisterPatchCategoriesTower(this BasicMod mod, params object[] features)
    //{
    //    foreach (var feature in features)
    //        mod.Harmony.UnpatchCategory(feature.ToString());
    //}
}
