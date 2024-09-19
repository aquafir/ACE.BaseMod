
namespace ACE.Shared.Settings;
public class BasicMod : IHarmonyMod
{
    #region Members
    //If Harmony is set to debug it creates a log on Desktop
    public const bool DEBUGGING = false;
    public Harmony Harmony { get; set; }

    //Point to your mod directory
    public string ModPath { get; set; }
    public ModContainer Container => ModManager.GetModContainerByPath(ModPath);

    //Global access to mod
    public static BasicMod Instance { get; set; }

    public IPatch Patch { get; set; }

    //IDs are used by Harmony to separate multiple patches
    public string ID { get; set; }
    public string Name { get; set; }

    public ModState State { get; set; } = ModState.None;
    private bool disposedValue;

    public BasicMod() { }
    public BasicMod(string name, IPatch patch)
    {
        Setup(name, patch);
    }
    #endregion

    public void Setup(string name, IPatch patch)
    {
        Name = name;
        ID = $"com.ACE.ACEmulator.{Name}";
        ModPath = Path.Combine(ModManager.ModPath, Name);
        Harmony = new(ID);
        Instance = this;
        Patch = patch;
    }

    #region Initialize / Dispose (called by ACE)
    public void Initialize()
    {
        if (DEBUGGING)
        {
            Harmony.DEBUG = DEBUGGING;
            ModManager.Log($"Initializing {ID}...");
        }

        Start();
    }

    //https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Stop();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Start / Stop (control the patches internally)
    public virtual void Start()
    {
        try
        {
            Harmony.PatchAllUncategorized();
            Patch.Start();
        }
        catch (Exception ex)
        {
            ModManager.Log($"Failed to start.  Unpatching {ID}: {ex.Message}");
            ModManager.DisableModByPath(ModPath);
        }
    }

    public virtual void Stop()
    {
        try
        {
            Patch.Shutdown();
            Harmony.UnpatchAll(ID);
        }
        catch (Exception ex)
        {
            ModManager.Log($"Failed to shutdown.  Unpatching {ID}: {ex.Message}");
            ModManager.DisableModByPath(ModPath);
        }
    }
    #endregion
}

public enum ModState
{
    None,       // Mod instance freshly created
    Loading,    // Patch class has been started
    Error,      // An error has occurred (loading/saving/etc.)
    Running     // Mod successfully started
}







//public class Mod : BasicMod//() : BasicMod("MyMod", new BasicPatch<Settings>(this))
//{
//    public Mod() : base()
//    {
//        var patch = new BasicPatch<Settings>(this);

//        Setup("MyName", patch);
//    }
//}

//public class Settings : JsonSettings, ISettings
//{
//    public Task CreateAsync()
//    {
//        throw new NotImplementedException();
//    }

//    public Task CreateOrLoadAsync()
//    {
//        throw new NotImplementedException();
//    }
//}