namespace Tower;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{

    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;
        ModC.RegisterCommands(Settings.Features);
        ModC.RegisterPatchCategories(Settings.Features);
        
        BankExtensions.Init();
        FloorExtensions.Init();
    } 

    public override void Stop()
    {
        base.Stop();

        if (Settings is null)
            return;

        ModC.UnregisterCommands(Settings.Features);
        Mod.Instance.Harmony.UnpatchAll();
    }

    /// <summary>
    /// Adds additional features to ACE that may be needed by custom loot
    /// </summary>
    //private  void SetupFeatures()
    //{
    //    //Add enabled Feature patches
    //    foreach (var feature in Settings.Features)
    //    {
    //        Mod.Harmony.PatchCategory(feature.ToString());

    //        if (Settings.Verbose)
    //            ModManager.Log($"Enabled feature: {feature}");
    //    }

    //    //Add commands of enabled features
    //    var commandRegex = String.Join("|", Settings.Features);
    //    Mod.Container.RegisterCommandCategory(commandRegex);
    //}
}
