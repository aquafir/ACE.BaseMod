using ACE.Shared.Mods;

namespace ACE.Shared.Tests; 

public class SettingsContainerTests
{
    JsonSettings<Settings> SC;
    string PATH = @"M:\Downloads\Settings.json";

    [SetUp]
    public void Setup()
    {
        SC = new(PATH);
    }
    
    //[Test]
    //public async Task CreateOrLoadAsync_FailsRetries_SettingsNull()
    //{
    //    Task.Run(() => SimulateFileLockAsync(PATH, TimeSpan.FromSeconds(3)));
    //    await SC.LoadOrCreateAsync(5, 5);
    //    Assert.IsNull(SC.Settings);
    //}

    //[Test]
    //public async Task CreateOrLoadAsync_SucceedsRetries_SettingsNotNull()
    //{
    //    Task.Run(() => SimulateFileLockAsync(PATH, TimeSpan.FromSeconds(3)));
    //    await SC.LoadOrCreateAsync();
    //    Assert.IsNotNull(SC.Settings);
    //}

    //[Test]
    //public async Task CreateOrLoadAsync_WhenSuccess_SettingsNotNull()
    //{
    //    await SC.LoadOrCreateAsync();
    //    Assert.IsNotNull(SC.Settings);
    //}

    //[Test]
    //public async Task CreateOrLoadAsync_WhenSuccess_HasFileAsync()
    //{
    //    await SC.LoadOrCreateAsync();
    //    Assert.IsTrue(File.Exists(PATH));
    //}

    public static async Task SimulateFileLockAsync(string filePath, TimeSpan lockDuration)
    {
        // Open the file with exclusive access (no sharing)
        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        {
            Console.WriteLine("File is locked.");

            // Keep the file locked for the specified duration
            await Task.Delay(lockDuration);

            Console.WriteLine("File is now unlocked.");
        }
    }
}
