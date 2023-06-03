namespace ACE.Web;

public class WebApp
{
    string SettingsPath => Path.Combine(Mod.ModPath, "appsettings.json");
    string RootPath => Path.Combine(Mod.ModPath, "wwwroot");

    private WebApplication app;
    public void Start()
    {
        Directory.CreateDirectory(RootPath);

        Console.WriteLine("Starting up...");
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = Mod.ModPath,  //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-7.0&tabs=windows#content-root
            WebRootPath = RootPath,         //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-7.0&tabs=windows#web-root
        });

        //Configure builder.  Defaults to HTTP:5000 on Kestrel: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-7.0
        builder.Configuration.AddJsonFile(SettingsPath, false, true);

        //Set up logging.  Defaults to none in favor of ACE logging
        builder.Logging.ClearProviders();
        //builder.Logging.AddConsole();

        //Add services
        builder.Services.AddDirectoryBrowser();
        builder.Services.AddControllers();

        //Make the app
        app = builder.Build();

        //Configure errors / HTTPS
        //app.UseExceptionHandler("/Error");
        //app.UseHsts();    

        //Routes
        app.UseDefaultFiles();  //Set defaults served: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-7.0#serve-default-documents
        app.UseStaticFiles();   //Work as a file server: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-7.0

        MapRoutes();

        app.RunAsync();
    }

    private void MapRoutes()
    {
        MapUpload();
        MapPlayer();
    }

    private void MapUpload()
    {
        //Upload a file to root
        var result = app.MapPost("/upload", async Task<IResult> (HttpRequest request) =>
        {
            var form = await request.ReadFormAsync();
            var formFile = form.Files["file"];
            var path = Path.Combine(RootPath, formFile.FileName);
            formFile.OpenReadStream().SaveFile(path);
            return Results.Ok($"Uploaded {formFile.FileName}");
        });
    }

    private void MapPlayer()
    {
        //List players online status
        app.MapGet("/player/", () =>
            string.Join(Environment.NewLine, PlayerManager.GetAllPlayers().Select(x => $"{x.Name} is {(x is OfflinePlayer ? "offline" : "online")}").ToList()));

        //Check a named player and list their items if online
        app.MapGet("/player/{name}", (string name) =>
        {
            var player = PlayerManager.GetAllPlayers().Where(x => x.Name == name).FirstOrDefault();
            if (player is null)
                return $"{name} does not exist";

            if (player is OfflinePlayer)
                return $"{name} is offline";

            if (player is not Player p)
                return "Error";

            var sb = new StringBuilder(p.Name + "\r\n");
            foreach (var item in p.Inventory)
                sb.AppendLine($"{item.Key} -- {item.Value.Name}");

            return sb.ToString();
        });

        //Search an online players items for a string
        app.MapGet("/player/{name}/{query}", (string name, string query) =>
        {
            var player = PlayerManager.GetAllPlayers().Where(x => x.Name == name).FirstOrDefault();
            if (player is null)
                return $"{name} does not exist";

            if (player is not Player p || string.IsNullOrEmpty(query))
                return "Error";

            var sb = new StringBuilder(p.Name + "\r\n");

            foreach (var item in p.Inventory.Where(x => x.Value.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)))
                sb.AppendLine($"{item.Key} -- {item.Value.Name}");

            return sb.ToString();
        });
    }

    public void Stop()
    {
        ModManager.Log("Shutting down web app...");
        app.Lifetime.StopApplication();
        app.Lifetime.ApplicationStopped.WaitHandle.WaitOne();
        ModManager.Log("Shut down");
        //app.DisposeAsync().GetAwaiter().GetResult();
        //app.StopAsync().GetAwaiter().GetResult();    
    }
}