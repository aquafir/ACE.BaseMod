//namespace ACE.Shared.Settings;

///// <summary>
///// PatchClasses are entry points for logic of a loaded Mod responsible for loading settings, applying patches, registering commands, initializing needed plugin state, and shutting down if disabled.
///// </summary>
//public class BasicPatch<T> : IPatch where T : ISettings, new() 
//{
//    private BasicMod Mod;
//    public SettingsContainer<T> Settings;

//    public BasicPatch()
//    {
//    }
//    public BasicPatch(BasicMod mod, string settingsName = "Settings.json")
//    {
//        Mod = mod;
//        Settings = new JsonSettings<T>(Path.Combine(Mod.ModPath, settingsName));
//    }
//    public BasicPatch(BasicMod mod, SettingsContainer<T> settings, string settingsName = "Settings.json")
//    {
//        Mod = mod;
//        Settings = settings;
//    }

//    //protected virtual void Init(BasicMod mod, string settings = "Settings.json")
//    //{
//    //    Mod = mod;
//    //    Settings = new(Path.Combine(Mod.ModPath, settings));
//    //}

//    //protected virtual async Task RunOnStartup()
//    //{
//    //    while (true)
//    //    {
//    //        if (WorldManager.WorldStatus == WorldManager.WorldStatusState.Open)
//    //        {
//    //            break;
//    //        }
//    //        await Task.Delay(1000);
//    //    }
//    //}

//    //#region Start/Shutdown
//    public async void Start()
//    {
//        //Need to decide on async use
//        Mod.State = ModState.Loading;

//        await Settings.CreateOrLoadAsync();
        
//        LoadSettings();

//        if (Mod.State == ModState.Error)
//        {
//            ModManager.DisableModByPath(Mod.ModPath);
//            return;
//        }

//        Mod.State = ModState.Running;
//    }

//    public virtual void Shutdown()
//    {
//        //if (IMod.State == ModState.Running)
//        //    // Shut down enabled mod...

//        //    //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
//        //    //SaveSettings();

//        //    if (IMod.State == ModState.Error)
//        //        ModManager.Log($"Improper shutdown: {IMod.ModPath}", ModManager.LogLevel.Error);
//    }
//    //#endregion

//}
