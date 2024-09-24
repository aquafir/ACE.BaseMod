namespace ACE.Shared.Mods;
public abstract class SettingsContainer<T> where T : class?, new()
{
    const int RETRIES = 10;

    protected FileSystemWatcher _fileWatcher;
    protected string SettingsPath;
    protected FileInfo SettingsInfo;

    public event EventHandler SettingsChanged;

    public T Settings { get; set; }

    private DateTime _lastChange = DateTime.MinValue;
    private readonly TimeSpan DEBOUNCE_INTERVAL = TimeSpan.FromMilliseconds(500);

    public SettingsContainer(string filePath)
    {
        this.SettingsPath = filePath;
        SettingsInfo = new(filePath);

        _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(filePath))
        {
            //Path =
            Filter = Path.GetFileName(filePath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
            EnableRaisingEvents = true,
        };
        
        _fileWatcher.Changed += OnSettingsChanged;
        _fileWatcher.EnableRaisingEvents = true;
    }

    protected virtual async void OnSettingsChanged(object sender, FileSystemEventArgs e)
    {
        //Gate reload events, was getting doubled
        var lapsed = DateTime.Now - _lastChange;
        if (lapsed < DEBOUNCE_INTERVAL)
            return;

        _lastChange = DateTime.Now;

        //Todo: think about better ways of doing this
        //When the settings file is changed reload and raise the event
        var result = await LoadOrCreateAsync();
        
        if(result)
            Console.WriteLine($"Reloaded settings: {SettingsPath}");

        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public virtual async Task<bool> LoadOrCreateAsync(int delay = 2000, int retries = 5) 
    {
        int attempt = 0;

        //Retry up to a number of attempts
        while (attempt < retries)
        {
            try
            {
                //Return successfully loaded settings
                if (await LoadSettingsAsync() is T settings)
                {
                    Settings = settings;
                    return true;
                }
                else
                {
                    //Otherwise try to save new settings, returning null on failure
                    Settings = new();

                    var success = await SaveSettingsAsync(Settings);
                    if (!success)
                        Settings = null;

                    return Settings is null;

                }
            }
            catch (IOException ex)
            {
                if (attempt++ >= retries)
                {
                    // If max retries reached, return failure
                    ModManager.Log($"{Path.GetFileName(SettingsPath)} failed after {attempt} attempts: {ex.Message}", ModManager.LogLevel.Error);
                    return false;
                }

                // Wait before retrying
                await Task.Delay(delay);
            }
            catch (Exception ex)
            {
                //Don't retry on non-IO exceptions
                ModManager.Log($"{Path.GetFileName(SettingsPath)} failed: {ex.Message}", ModManager.LogLevel.Error);
                return false;
            }
        }

        // Should never reach here
        Settings = null;
        return false;
    }

    protected abstract Task<T> LoadSettingsAsync();
    protected abstract Task<bool> SaveSettingsAsync(T settings);
}