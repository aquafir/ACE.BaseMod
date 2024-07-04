//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Serialization;

//namespace ACE.Shared.Settings;



//public abstract class IMod : IHarmonyMod
//{
//    public IMod(string name)
//    {
//        Name = name;
//    }

//    #region Members
//    //If Harmony is set to debug it creates a log on Desktop
//    public const bool DEBUGGING = false;
//    //Point to your mod directory
//    public ModContainer Container => ModManager.GetModContainerByPath(ModPath);
//    public string Name { get; private set; } = "Unknown";
//    public string ModPath = Path.Combine(ModManager.ModPath, Name);

//    //IDs are used by Harmony to separate multiple patches
//    public string ID = $"com.ACE.ACEmulator.{Name}";
//    public Harmony Harmony { get; set; } = new(ID);

//    private bool disposedValue;
//    public IMod Instance { get; private set; }

//    //Reload on changes in Settings.json
//    private FileSystemWatcher _settingsWatcher;
//    private DateTime _lastChange = DateTime.Now;
//    private readonly TimeSpan _reloadInterval = TimeSpan.FromSeconds(3);

//    private IPatchClass patch;// = new();
//    public ModState State = ModState.None;
//    #endregion

//    #region Initialize / Dispose (called by ACE)
//    public void Initialize()
//    {
//        if (DEBUGGING)
//        {
//            Harmony.DEBUG = DEBUGGING;
//            ModManager.Log($"Initializing {ID}...");
//        }

//        Instance = this;

//        _settingsWatcher = new FileSystemWatcher()
//        {
//            Path = ModPath,
//            Filter = $"Settings.json",
//            EnableRaisingEvents = true,
//            NotifyFilter = NotifyFilters.LastWrite
//        };

//        _settingsWatcher.Changed += Settings_Changed;
//        _settingsWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;

//        Start();
//    }

//    //https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
//    protected virtual void Dispose(bool disposing)
//    {
//        if (!disposedValue)
//        {
//            if (disposing)
//            {
//                Stop();

//                _settingsWatcher.Changed -= Settings_Changed;
//            }

//            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
//            // TODO: set large fields to null
//            disposedValue = true;
//        }
//    }

//    public void Dispose()
//    {
//        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
//        Dispose(disposing: true);
//        GC.SuppressFinalize(this);
//    }

//    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
//    // ~Mod()
//    // {
//    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
//    //     Dispose(disposing: false);
//    // }

//    #region Dispose Async
//    //https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
//    //public async ValueTask DisposeAsync()
//    //{
//    //    await DisposeAsyncCore().ConfigureAwait(false);

//    //    Dispose(disposing: false);
//    //    #pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
//    //    GC.SuppressFinalize(this);
//    //    #pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
//    //}

//    //public async ValueTask DisposeAsyncCore()
//    //{

//    //}
//    #endregion
//    #endregion

//    #region Start / Stop (control the patches internally)
//    private void Start()
//    {
//        try
//        {
//            //Patch everything in the mod with Harmony attributes
//            Harmony.PatchAllUncategorized();

//            patch.Start();
//        }
//        catch (Exception ex)
//        {
//            ModManager.Log($"Failed to start.  Unpatching {ID}: {ex.Message}");
//            ModManager.DisableModByPath(ModPath);
//            //Dispose();
//        }
//    }

//    private void Stop()
//    {
//        // TODO: dispose managed state (managed objects)
//        patch.Shutdown();

//        //CustomCommands.Unregister();
//        Harmony.UnpatchAll(ID);
//    }
//    #endregion

//    private void Settings_Changed(object sender, FileSystemEventArgs e)
//    {
//        //Only reload if currently running
//        if (State != ModState.Running)
//            return;

//        var delta = DateTime.Now - _lastChange;
//        if (delta < _reloadInterval)
//            return;
//        _lastChange = DateTime.Now;

//        //An alternative would be to reload through the ModContainer
//        //ModManager.Log($"Settings changed, reloading after {delta.TotalSeconds} seconds...");
//        Stop();
//        Start();
//        ModManager.Log($"Settings reloaded.");
//    }
//}

//public enum ModState
//{
//    None,       // Mod instance freshly created
//    Loading,    // Patch class has been started
//    Error,      // An error has occurred (loading/saving/etc.)
//    Running     // Mod successfully started
//}


///// <summary>
///// PatchClasses are entry points for logic of a loaded Mod responsible for loading settings, applying patches, registering commands, initializing needed plugin state, and shutting down if disabled.
///// </summary>
//public abstract class IPatchClass {
//    //#region Settings
//    //const int RETRIES = 10;

//    //public static Settings Settings = new();
//    //static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
//    //private FileInfo settingsInfo = new(settingsPath);

//    //private JsonSerializerOptions _serializeOptions = new()
//    //{
//    //    WriteIndented = true,
//    //    AllowTrailingCommas = true,
//    //    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
//    //    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
//    //};

//    //private void SaveSettings()
//    //{
//    //    string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

//    //    if (!settingsInfo.RetryWrite(jsonString, RETRIES))
//    //    {
//    //        ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
//    //        Mod.State = ModState.Error;
//    //    }
//    //}

//    //private void LoadSettings()
//    //{
//    //    if (!settingsInfo.Exists)
//    //    {
//    //        ModManager.Log($"Creating {settingsInfo}...");
//    //        SaveSettings();
//    //    }
//    //    else
//    //        ModManager.Log($"Loading settings from {settingsPath}...");

//    //    if (!settingsInfo.RetryRead(out string jsonString, RETRIES))
//    //    {
//    //        Mod.State = ModState.Error;
//    //        return;
//    //    }

//    //    try
//    //    {
//    //        Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
//    //    }
//    //    catch (Exception)
//    //    {
//    //        ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
//    //        Mod.State = ModState.Error;
//    //        return;
//    //    }
//    //}
//    //#endregion

//    //#region Start/Shutdown
//    //public void Start()
//    //{
//    //    //Need to decide on async use
//    //    Mod.State = ModState.Loading;
//    //    LoadSettings();

//    //    if (Mod.State == ModState.Error)
//    //    {
//    //        ModManager.DisableModByPath(Mod.ModPath);
//    //        return;
//    //    }

//    //    SetupFeatures();

//    //    BankExtensions.Init();
//    //    FloorExtensions.Init();
//    //    Mod.State = ModState.Running;
//    //}

//    public virtual void Shutdown()
//    {
//        if (IMod.State == ModState.Running)
//            // Shut down enabled mod...

//            //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
//            //SaveSettings();

//            if (IMod.State == ModState.Error)
//                ModManager.Log($"Improper shutdown: {IMod.ModPath}", ModManager.LogLevel.Error);
//    }
//    //#endregion

//}

//public interface ISettings
//{
//    Task CreateAsync(string filePath);
//    Task CreateOrLoadAsync(string filePath);
//}

//public abstract class GenericSettings<T> : ISettings where T : class, new()
//{
//    private FileSystemWatcher _fileWatcher;
//    public T Data { get; set; } = new T();

//    public GenericSettings(string filePath)
//    {
//        _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(filePath))
//        {
//            Filter = Path.GetFileName(filePath),
//            NotifyFilter = NotifyFilters.LastWrite
//        };

//        _fileWatcher.Changed += OnChanged;
//        _fileWatcher.EnableRaisingEvents = true;
//    }

//    private async void OnChanged(object sender, FileSystemEventArgs e)
//    {
//        await CreateOrLoadAsync(e.FullPath);
//    }

//    public abstract Task CreateAsync(string filePath);
//    public abstract Task CreateOrLoadAsync(string filePath);
//}



//public class JsonSettings<T> : ISettings where T : class, new()
//{
//    public T Data { get; set; } = new T();

//    public async Task CreateAsync(string filePath)
//    {
//        var json = JsonSerializer.Serialize(Data);
//        await File.WriteAllTextAsync(filePath, json);
//    }

//    public async Task CreateOrLoadAsync(string filePath)
//    {
//        if (File.Exists(filePath))
//        {
//            var json = await File.ReadAllTextAsync(filePath);
//            Data = JsonSerializer.Deserialize<T>(json);
//        }
//    }
//}


//public class UserSettings
//{
//    public string Username { get; set; }
//    public string Email { get; set; }
//}

//public class ApplicationSettings
//{
//    public UserSettings User { get; set; } = new UserSettings();
//    public string ApplicationName { get; set; }
//    public string Version { get; set; }
//}

//public class XmlSettings<T> : ISettings where T : class, new()
//{
//    public T Data { get; set; } = new T();

//    public async Task CreateAsync(string filePath)
//    {
//        var serializer = new XmlSerializer(typeof(T));
//        using var stream = new FileStream(filePath, FileMode.Create);
//        serializer.Serialize(stream, Data);
//        await Task.CompletedTask;
//    }

//    public async Task CreateOrLoadAsync(string filePath)
//    {
//        if (File.Exists(filePath))
//        {
//            var serializer = new XmlSerializer(typeof(T));
//            using var stream = new FileStream(filePath, FileMode.Open);
//            Data = (T)serializer.Deserialize(stream);
//        }
//        await Task.CompletedTask;
//    }
//}

////public static async Task Main(string[] args)
////{
////    // Using Settings for UserSettings
////    var userSettings = new Settings<UserSettings>();
////    userSettings.Data.Username = "JohnDoe";
////    userSettings.Data.Email = "john.doe@example.com";
////    await userSettings.SaveAsync("userSettings.json");
////    await userSettings.LoadAsync("userSettings.json");

////    // Using Settings for ApplicationSettings
////    var appSettings = new Settings<ApplicationSettings>();
////    appSettings.Data.ApplicationName = "MyApp";
////    appSettings.Data.Version = "1.0.0";
////    appSettings.Data.User.Username = "JaneDoe";
////    appSettings.Data.User.Email = "jane.doe@example.com";
////    await appSettings.SaveAsync("appSettings.json");
////    await appSettings.LoadAsync("appSettings.json");
////}