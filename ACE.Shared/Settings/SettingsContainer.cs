using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Shared.Settings;
public abstract class SettingsContainer<T> where T : ISettings, new()
{
    const int RETRIES = 10;

    protected FileSystemWatcher _fileWatcher;
    protected string SettingsPath;
    protected FileInfo SettingsInfo;

    public T Settings { get; set; } = new();

    public SettingsContainer(string filePath)
    {
        this.SettingsPath = filePath;
        SettingsInfo = new(filePath);

        _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(filePath))
        {
            //Path =
            Filter = Path.GetFileName(filePath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            EnableRaisingEvents = true,
        };

        _fileWatcher.Changed += OnSettingsChanged;
        _fileWatcher.EnableRaisingEvents = true;
    }

    protected virtual async void OnSettingsChanged(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"Reloaded settings: {SettingsPath}");
        await CreateOrLoadAsync();
    }

    public abstract Task CreateAsync();
    public abstract Task LoadAsync(string contents);
    public virtual async Task CreateOrLoadAsync()
    {
        if (!SettingsInfo.Exists)
        {
            ModManager.Log($"Creating {SettingsInfo}...");
            await CreateAsync();
        }
        else
            ModManager.Log($"Loading settings from {SettingsPath}...");

        try
        {
            if (!SettingsInfo.RetryRead(out string contents, RETRIES))
            {
                Mod.State = ModState.Error;
                return;
            }
            else
            {
                await LoadAsync(contents);
                Console.WriteLine($"Loaded settings: {SettingsPath}");
            }
        }
        catch (Exception ex)
        {
                    ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
                    Mod.State = ModState.Error;
                    return;
        }
    }



    private void SaveSettings(string contents)
    {
        //string jsonString = JsonSerializer.Serialize(SettingsFile, _serializeOptions);

        if (!SettingsInfo.RetryWrite(contents, RETRIES))
        {
            //ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
            //Mod.State = ModState.Error;
        }
    }
}