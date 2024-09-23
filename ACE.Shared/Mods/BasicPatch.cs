using static ACE.Server.Mods.ModManager;
using static ACE.Server.WorldObjects.Creature;

namespace ACE.Shared.Mods;

/// <summary>
/// PatchClasses are entry points for logic of a loaded Mod responsible for loading settings, applying patches, registering commands, initializing needed plugin state, and shutting down if disabled.
/// </summary>
public class BasicPatch<T> : IPatch where T : class?, new()
{
    private bool disposedValue;
    protected BasicMod Mod;
    protected SettingsContainer<T> SettingsContainer;
    protected T Settings => SettingsContainer.Settings;
    protected string SettingsName;
    protected string SettingsPath => Path.Combine(Mod.ModPath, SettingsName);


    //public BasicPatch() { }
    public BasicPatch(BasicMod mod, string settingsName = "Settings.json")
    {
        Mod = mod;
        SettingsName = settingsName;
    }
    
    /// <summary>
    /// Loads settings and calls OnStartSuccess / OnWorldOpen if there are no errors
    /// </summary>
    public virtual async void Start()
    {
        //Need to decide on async use
        Mod.State = ModState.Loading;

        if (!await SettingsContainer.LoadOrCreateAsync())
        {
            Mod.State = ModState.Error;
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        Mod.State = ModState.Running;

        await OnStartSuccess();

        while (WorldManager.WorldStatus != WorldManager.WorldStatusState.Open)
            await Task.Delay(1000);
        await OnWorldOpen();
    }

    public virtual async void Stop()
    {
        //if (Mod.State == ModState.Running)
        //Do Things

        //if (Mod.State == ModState.Error)
        //    ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }

    /// <summary>
    /// Runs immediately on 
    /// </summary>
    public virtual async Task OnStartSuccess() { }

    /// <summary>
    /// Runs when the WorldStatus is Open
    /// </summary>
    public virtual async Task OnWorldOpen() { }

    protected virtual void SettingsChanged(object? sender, EventArgs e)
    {
        //Only reload if currently running
        if (Mod.State != ModState.Running)
            return;

        Task.Run(Stop).Wait();

        if (Settings is not null)
        {
            Task.Run(Start).Wait();
            ModManager.Log($"Settings reloaded.");
        }
    }

    #region Init / Dispose
    public virtual void Init()
    {
        SettingsContainer = new JsonSettings<T>(SettingsPath);
        SettingsContainer.SettingsChanged += SettingsChanged;
        Start();
    }

    public virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                SettingsContainer.SettingsChanged -= SettingsChanged;
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
}