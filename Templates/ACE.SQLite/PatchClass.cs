namespace ACE.SQLite;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    private const string DbName = "Mod.db";
    public static string DbPath => Path.Combine(Mod.Instance.ModPath, DbName);

    public static IServiceProvider ServiceProvider;

    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;

        //Set up services
        var services = new ServiceCollection();
        services.AddPooledDbContextFactory<ModContext>(options =>
            options.UseSqlite($"Data Source={DbPath}"));

        ServiceProvider = services.BuildServiceProvider();

        using var db = ModDatabase.CreateContext();

        //Create database if needed.  Sample does not deal with database migrations
        if (db.Database.EnsureCreated())
            ModManager.Log($"Created database at {DbPath}");

        if (db is null || !db.Database.CanConnect())
        {
            Mod.Instance.Container.Disable();
            return;
        }

        // Create
        Console.WriteLine("Inserting a new blog");
        db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
        db.SaveChanges();

        // Read
        Console.WriteLine("Querying for a blog");
        var blog = db.Blogs
            .OrderBy(b => b.BlogId)
            .First();

        // Update
        Console.WriteLine("Updating the blog and adding a post");
        blog.Url = "https://devblogs.microsoft.com/dotnet";
        blog.Posts.Add(
            new Post { Title = "Hello World", Content = "I wrote an app using EF Core!" });
        db.SaveChanges();
    }
}


