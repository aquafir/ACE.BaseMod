namespace ACE.CatMod;

public class Mod : IHarmonyMod
{
    #region Members
    //If Harmony is set to debug it creates a log on Desktop
    public const bool DEBUGGING = false;
    //Point to your mod directory
    public static ModContainer Container => ModManager.GetModContainerByPath(Mod.ModPath);
    public static string ModPath = Path.Combine(ModManager.ModPath, "ACE.CatMod");

    //IDs are used by Harmony to separate multiple patches
    const string ID = "com.ACE.ACEmulator.ACE.CatMod";
    public static Harmony Harmony { get; set; } = new(ID);

    private bool disposedValue;
    public static Mod Instance { get; private set; }

    //Reload on changes in Settings.json
    private FileSystemWatcher _settingsWatcher;
    private DateTime _lastChange = DateTime.Now;
    private readonly TimeSpan _reloadInterval = TimeSpan.FromSeconds(3);

    private PatchClass patch = new();
    public static ModState State = ModState.None;
    #endregion

    #region Initialize / Dispose (called by ACE)
    public void Initialize()
    {
        if (DEBUGGING)
        {
            Harmony.DEBUG = DEBUGGING;
            ModManager.Log($"Initializing {ID}...");
        }

        Instance = this;

        _settingsWatcher = new FileSystemWatcher()
        {
            Path = ModPath,
            Filter = $"Settings.json",
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.LastWrite
        };

        _settingsWatcher.Changed += Settings_Changed;
        _settingsWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;

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

                _settingsWatcher.Changed -= Settings_Changed;
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

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Mod()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    #region Dispose Async
    //https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
    //public async ValueTask DisposeAsync()
    //{
    //    await DisposeAsyncCore().ConfigureAwait(false);

    //    Dispose(disposing: false);
    //    #pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    //    GC.SuppressFinalize(this);
    //    #pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    //}

    //public async ValueTask DisposeAsyncCore()
    //{

    //}
    #endregion
    #endregion

    #region Start / Stop (control the patches internally)
    private void Start()
    {
        try
        {
            //Patch everything in the mod with Harmony attributes
            Harmony.PatchAllUncategorized();

            patch.Start();
        }
        catch (Exception ex)
        {
            ModManager.Log($"Failed to start.  Unpatching {ID}: {ex.Message}");
            ModManager.DisableModByPath(ModPath);
            //Dispose();
        }
    }

    private void Stop()
    {
        // TODO: dispose managed state (managed objects)
        patch.Shutdown();

        //CustomCommands.Unregister();
        Harmony.UnpatchAll(ID);
    }
    #endregion

    private void Settings_Changed(object sender, FileSystemEventArgs e)
    {
        //Only reload if currently running
        if (State != ModState.Running)
            return;

        var delta = DateTime.Now - _lastChange;
        if (delta < _reloadInterval)
            return;
        _lastChange = DateTime.Now;

        //An alternative would be to reload through the ModContainer
        //ModManager.Log($"Settings changed, reloading after {delta.TotalSeconds} seconds...");
        Stop();
        Start();
        ModManager.Log($"Settings reloaded.");
    }
}

public enum ModState
{
    None,       // Mod instance freshly created
    Loading,    // Patch class has been started
    Error,      // An error has occurred (loading/saving/etc.)
    Running     // Mod successfully started
}
