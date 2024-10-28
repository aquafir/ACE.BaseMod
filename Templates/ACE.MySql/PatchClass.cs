
namespace ACE.MySql;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public static IServiceProvider ServiceProvider;

    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;

        //Set up services
        var config = ConfigManager.Config.MySql.Authentication;
        var connectionString = String.IsNullOrWhiteSpace(Settings.ConnectionString) ? 
            $"server={config.Host};port={config.Port};user={config.Username};password={config.Password};database={Settings.DbName};{config.ConnectionOptions}" :
            Settings.ConnectionString;
        
        var services = new ServiceCollection();
        services.AddPooledDbContextFactory<ModContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), builder => builder.EnableRetryOnFailure(10)));

        ServiceProvider = services.BuildServiceProvider();

        using var db = ModDatabase.CreateContext();

        //Create database if needed.  Sample does not deal with database migrations
        if (await db.Database.EnsureCreatedAsync())
            ModManager.Log($"Created database {Settings.DbName}");

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

