using ACE.Database.Models.Shard;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.WebSockets;

namespace MinimalAPI;

public class WebApp
{
    string SettingsPath => Path.Combine(Mod.ModPath, "appsettings.json");
    string RootPath => Path.Combine(Mod.ModPath, "wwwroot");

    private WebApplication app;
    public void Start()
    {
        Directory.CreateDirectory(RootPath);  //Check is redundant

        Console.WriteLine("Starting up...");
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = Mod.ModPath,  //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-7.0&tabs=windows#content-root
            WebRootPath = RootPath,         //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-7.0&tabs=windows#web-root
            //ApplicationName = "",,
            //EnvironmentName = Environments.Staging
        });

        //Configure builder.  Defaults to HTTP:5000 on Kestrel: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-7.0
        builder.Configuration.AddJsonFile(SettingsPath, false, true);
        //builder.Configuration.AddJsonFile("launchSettings.json", true);

        //Log if you want.  Nothing be default
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        //Add services
        builder.Services.AddRazorPages(); 
        //builder.Services.AddControllersWithViews();
        builder.Services.AddControllers();
        //builder.Services.AddDbContext<ShardDbContext>();

        //Work as a directory browser
        //builder.Services.AddDirectoryBrowser();

        app = builder.Build();

        //Websockets
        //var webSocketOptions = new WebSocketOptions
        //{
        //    KeepAliveInterval = TimeSpan.FromMinutes(2)
        //};
        //app.UseWebSockets(webSocketOptions);

        //app.Use(async (context, next) =>
        //{
        //    if (context.WebSockets.IsWebSocketRequest)
        //    {
        //        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        //        Console.WriteLine("WebSocket Connected");
        //    }
        //    else
        //    {
        //        await next();
        //    }
        //});

        //Configure errors / HTTPS
        //if (!app.Environment.IsDevelopment())
        //{
        //    app.UseExceptionHandler("/Error");
        //    app.UseHsts();    
        //}

        //app.MapControllers();
        //app.MapRazorPages();

        //Work as a file server: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-7.0
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapControllers();
        //app.UseFileServer(new FileServerOptions()
        //{
        //    EnableDirectoryBrowsing = true,
        //    StaticFileOptions =
        //    {
        //        ServeUnknownFileTypes = true,
        //        DefaultContentType = "application/octet-stream"
        //    }
        //});

        MapRoutes();
        //MapWebsocket();

        //app.Run would block commandline
        app.RunAsync();
        //app.RunAsync("http://localhost:" + port++);
    }

    private void MapWebsocket()
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Controllers.WebSocketController.Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }

        });
    }

    private void MapRoutes()
    {
        //app.MapGet("/test/{id}", async (int id, ShardDbContext db) =>
        //    await db.Biota.FindAsync(id)
        //        is Biota biota
        //            ? Results.Ok(biota)
        //            : Results.NotFound());

        var handler = () => "Route with a lambda";
        app.MapGet("/foo", handler);

        //Display player list
        app.MapGet("/", () => string.Join(Environment.NewLine, PlayerManager.GetAllPlayers().Select(x => x.Name).ToList()));

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

        //Upload a file to root
        var result = app.MapPost("/upload", async Task<IResult> (HttpRequest request) =>
        {
            var form = await request.ReadFormAsync();
            var formFile = form.Files["file"];
            var path = Path.Combine(RootPath, formFile.FileName);
            formFile.OpenReadStream().SaveFile(path);
            return Results.Ok(formFile.FileName);
        });
    }

    public void Stop()
    {
        Console.WriteLine("Waiting for shutdown");
        app.Lifetime.StopApplication();

        app.Lifetime.ApplicationStopped.WaitHandle.WaitOne();
        //app.StopAsync().GetAwaiter().GetResult();
        //app.DisposeAsync().GetAwaiter().GetResult();
        //WaitForPortAvailability(5000, TimeSpan.FromSeconds(30));

        Console.WriteLine("Port {0} is now available.", 5000);
        Start();
    }

    public static void WaitForPortAvailability(int port, TimeSpan timeout)
    {
        DateTime startTime = DateTime.Now;
        DateTime endTime = startTime.Add(timeout);

        while (DateTime.Now < endTime)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(IPAddress.Loopback, port);
                    return; // Port is now available, exit the method
                }
            }
            catch (SocketException)
            {
                // Port is still not available, continue waiting
                Thread.Sleep(1000); // Wait for 1 second before retrying
            }
        }

        // Timeout reached, port is not available within the specified time
        throw new TimeoutException("Timeout waiting for port availability.");
    }
}