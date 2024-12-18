﻿using ACE.Database;
using ACE.Database.Entity;
using ACE.Database.Models.World;

namespace AccessDb;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
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
        var portalDest = new Position(weenie.GetPosition(PositionType.Destination));
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
        var query = string.Join(" ", parameters);
        Search(query);
    }
    #endregion

    #region Logic
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
                        where creature.Type == (int)WeenieType.Creature
                        join cType in ctx.WeeniePropertiesInt on creature.ClassId equals cType.ObjectId
                        where cType.Type == (ushort)PropertyInt.CreatureType
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
    #endregion
}