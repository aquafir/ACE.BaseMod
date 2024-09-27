namespace ACE.Shared.Mods;
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

    public List<IPatch> Patches { get; set; }

    //IDs are used by Harmony to separate multiple patches
    public string ID { get; set; }
    public string Name { get; set; }

    public ModState State { get; set; } = ModState.None;
    private bool disposedValue;

    public BasicMod() { }
    public BasicMod(string name, params IPatch[] patch) => Setup(name, patch);
    #endregion

    protected virtual void Setup(string name, params IPatch[] patches)
    {
        Name = name;
        ID = $"com.ACE.ACEmulator.{Name}";
        ModPath = Path.Combine(ModManager.ModPath, Name);
        Harmony = new(ID);
        Instance = this;
        Patches = patches.ToList();
    }

    #region Initialize / Dispose (called by ACE)
    public virtual void Initialize()
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
            Harmony.PatchAllUncategorized(Container.ModAssembly);
            foreach (var patch in Patches)
                patch.Init();
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
            foreach (var patch in Patches)
                patch?.Dispose();
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
    /// <summary>
    /// Mod instance freshly created
    /// </summary>
    None,
    /// <summary>
    /// Patch class has been started
    /// </summary>
    Loading,
    /// <summary>
    /// An error has occurred (loading/saving/etc.)
    /// </summary>
    Error,
    /// <summary>
    /// Mod successfully started
    /// </summary>
    Running
}
