using ACE.Database.Models.Shard;
using ACE.Database.Models.World;
using ACE.Database;
using ACE.Entity.Enum.Properties;
using ACE.Server.Managers;
using Microsoft.EntityFrameworkCore;
using ACE.Server.WorldObjects;
using ACE.Server.Entity.Actions;
using ACE.Database.SQLFormatters.Shard;
using ACE.Database.Entity;
using ACE.Entity.Models;
using ACE.Server.Network.GameEvent.Events;
using System.Net;

namespace AccessDb;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
    const int RETRIES = 10;

    public static Settings Settings = new();
    private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
    private static FileInfo settingsInfo = new(settingsPath);

    private static JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

        if (!settingsInfo.RetryWrite(jsonString, RETRIES))
        {
            ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
        }
    }

    private static void LoadSettings()
    {
        if (!settingsInfo.Exists)
        {
            ModManager.Log($"Creating {settingsInfo}...");
            SaveSettings();
        }
        else
            ModManager.Log($"Loading settings from {settingsPath}...");

        if (!settingsInfo.RetryRead(out string jsonString, RETRIES))
        {
            Mod.State = ModState.Error;
            return;
        }

        try
        {
            Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
        }
        catch (Exception)
        {
            ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
            return;
        }
    }
    #endregion

    private static HttpListener listener = new();
    #region Start/Shutdown
    public static void Start()
    {
        //Need to decide on async use
        Mod.State = ModState.Loading;
        LoadSettings();

        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }
        
        listener.Prefixes.Add("http://localhost:8002/");

        listener.Start();
        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest req = context.Request;

            Console.WriteLine($"Received request for {req.Url}");

            using HttpListenerResponse resp = context.Response;
            resp.Headers.Set("Content-Type", "text/plain");

            string data = "Hello there!";
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            resp.ContentLength64 = buffer.Length;

            using Stream ros = resp.OutputStream;
            ros.Write(buffer, 0, buffer.Length);
        }

        Mod.State = ModState.Running;
    }

    public static void Shutdown()
    {
        //if (Mod.State == ModState.Running)
        // Shut down enabled mod...

        listener.Stop();
        listener.Prefixes.Clear();
        listener.Close();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    #region Commands
    [CommandHandler("ctypes", AccessLevel.Admin, CommandHandlerFlag.None, -1)]
    public static void HandleCTypes(Session session, params string[] parameters)
    {
        DisplayCreatureTypes();
    }

    [CommandHandler("lcount", AccessLevel.Admin, CommandHandlerFlag.None, -1)]
    public static void HandleLCount(Session session, params string[] parameters)
    {
        DisplayLoginCount();
    }

    [CommandHandler("plocs", AccessLevel.Admin, CommandHandlerFlag.None, -1)]
    public static void HandlePLocs(Session session, params string[] parameters)
    {
        DisplayPlayerLocations(PositionType.Location, PositionType.Sanctuary);
    }

    [CommandHandler("moveall", AccessLevel.Admin, CommandHandlerFlag.ConsoleInvoke, 2, "Moves everyone to a location")]
    public static void HandleMoveAll(Session session, params string[] parameters)
    {
        //There's a few ways of getting the position you want:
        //POI approach
        var teleportPOI = DatabaseManager.World.GetCachedPointOfInterest("holtburg");
        if (teleportPOI == null)
            return;
        var weenie = DatabaseManager.World.GetCachedWeenie(teleportPOI.WeenieClassId);
        var portalDest = new ACE.Entity.Position(weenie.GetPosition(PositionType.Destination));
        WorldObject.AdjustDungeon(portalDest);

        //Negatives for switching between ns/ew, or you can construct with /loc data
        //var holt = new ACE.Entity.Position(42.1f, 33.6f);

        //Similar but no guessing with negatives and checks for errors
        if (!CommandParameterHelpers.TryParsePosition(parameters, out var error, out var position))
        {
            ModManager.Log($"Error parsing position: {error}", ModManager.LogLevel.Error);
            return;
        }

        MoveAllPlayers(position);
    }

    [CommandHandler("search", AccessLevel.Admin, CommandHandlerFlag.ConsoleInvoke, -1)]
    public static void HandleSearch(Session session, params string[] parameters)
    {
        //Dictionary<PropertyType, List<object>> query = new Dictionary<PropertyType, List<object>>();
        var query = String.Join(" ", parameters);
        Search(query);
    }
    #endregion

    private static void Search(string query)
    {
        //Maybe use RecipeManager.VerifyRequirements  approach?

        var propType = PropertyType.PropertyString;
        var propKey = (ushort)PropertyString.Name;
        var propValue = query;
        var typedPropKey = PropertyString.Name;


        var sb = new StringBuilder("\r\n");
        foreach (var player in PlayerManager.GetAllOffline())
        {
            //var player = PlayerManager.GetOfflinePlayer(player.Guid);
            if (player is null)
                continue;

            sb.AppendLine($"{player.Name} ({player.Guid})");


            PossessedBiotas biotas = null;
            DatabaseManager.Shard.GetPossessedBiotasInParallel(player.Guid.Full, bios => biotas = bios);

            List<ACE.Database.Models.Shard.Biota> merged = new();

            Debugger.Break();
            if (biotas is null) continue;

            if (biotas.Inventory is not null)
                merged.AddRange(biotas.Inventory);
            if (biotas.WieldedItems is not null)
                merged.AddRange(biotas.WieldedItems);


            Debugger.Break();
            List<ACE.Database.Models.Shard.Biota> matches = new();


            foreach (var item in merged)
            {
                string value = item.GetProperty(typedPropKey);
                if (value is null)
                    continue;

                if (!value.Contains(propValue))
                    continue;

                matches.Add(item);
            }


            Debugger.Break();
            foreach (var match in matches)
            {
                sb.AppendLine($"{match.GetProperty(PropertyString.Name) ?? ""} {match.GetProperty(PropertyInt.CurrentWieldedLocation).GetValueOrDefault()}");
            }
        }
        ModManager.Log(sb.ToString());
    }

    private static void DoNeedful(ACE.Entity.Models.Biota biota, ACE.Database.Entity.PossessedBiotas biotas)
    {
        Debugger.Break();
    }

    /// <summary>
    /// Moves all online and offline players to a Position
    /// </summary>
    private static void MoveAllPlayers(Position position)
    {
        foreach (var p in PlayerManager.GetAllOnline())
        {
            p.SendMessage($"You have been forcibly relocated to {position}.");
            p.Teleport(position);
        }

        foreach (var p in PlayerManager.GetAllOffline())
        {
            //Only move regular players / restrict based on access level.  
            if (p.Account == null || p.Account.AccessLevel >= (uint)AccessLevel.Sentinel)
                continue;

            //Set Location position for a player to holt and save
            p.Biota.SetPosition(PositionType.Location, position, p.BiotaDatabaseLock);
            p.SaveBiotaToDatabase();
        }

        return;
    }

    /// <summary>
    /// Displays offline player locations
    /// </summary>
    /// <param name="locationTypes">Location types to print, defaulting to all</param>
    private static void DisplayPlayerLocations(params PositionType[] locationTypes)
    {
        if (locationTypes is null || locationTypes.Length == 0)
            locationTypes = Enum.GetValues<PositionType>();

        var sb = new StringBuilder("\r\n");
        foreach (var accounts in PlayerManager.GetAllOffline().OrderBy(x => x.Account.AccountId).GroupBy(g => g.Account.AccountName))
        {
            sb.AppendLine($"{accounts.Key}");
            foreach (var player in accounts)
            {
                //Get location position/other positions for player if you need it.  Not needed to move but using it to print position info
                sb.AppendLine($"  {player.Name} ({player.Account.AccountId})");
                foreach (var pt in locationTypes)
                {
                    if (!player.Biota.PropertiesPosition.TryGetValue(PositionType.Location, out var pos))
                        continue;

                    sb.AppendLine($"    {pt,-20}{pos.ObjCellId:X} - {pos.PositionX},{pos.PositionY},{pos.PositionZ}");
                }
            }
        }

        //Print out locations
        ModManager.Log($"{sb}");

    }

    /// <summary>
    /// Displays creature types and the number of types of creatures belonging to that type
    /// </summary>
    private static void DisplayCreatureTypes()
    {
        using (var ctx = new WorldDbContext())
        {
            // Group creatures by type
            var query = from creature in ctx.Weenie
                        where creature.Type == (int)(WeenieType.Creature)
                        join cType in ctx.WeeniePropertiesInt on creature.ClassId equals cType.ObjectId
                        where cType.Type == (ushort)(PropertyInt.CreatureType)
                        select new
                        {
                            Name = creature.ClassName,
                            Id = creature.ClassId,
                            Type = cType.Value,
                        };

            var sb = new StringBuilder($"\n\n{"Name",-40}{"Type",-15}{"Type #",-10}\n");
            foreach (var group in query.ToList().GroupBy(x => x.Type).OrderBy(x => x.Count()))
                sb.AppendLine($"{(CreatureType)group.Key,-40}{group.Key,-15}{group.Count(),-10}");

            ModManager.Log(sb.ToString());
        }
    }

    //For header spacing
    const int nSpace = 25; const int iSpace = 20; const int lSpace = 10;
    /// <summary>
    /// Uses EF to display the number of logins for each player
    /// </summary>
    private static void DisplayLoginCount()
    {
        using (var ctx = new ShardDbContext())
        {

            var sb = new StringBuilder($"\r\n{"Name",nSpace}{"ID",iSpace}{"Logins",lSpace}");

            var query = ctx.Character.ToList()
                .Select(s => new { s.AccountId, s.Name, s.Id, s.TotalLogins })
                .ToList();

            foreach (var result in query.GroupBy(x => x.AccountId))
            {
                sb.AppendLine($"\r\nAccount {result.Key}:");
                foreach (var character in result)
                {
                    sb.AppendLine($"  {character.Name,nSpace - 2}{character.Id,iSpace}{character.TotalLogins,lSpace}");
                }
            }

            ModManager.Log(sb.ToString());
        }
    }
}