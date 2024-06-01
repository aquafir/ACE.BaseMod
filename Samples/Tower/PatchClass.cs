using ACE.Database.Models.Shard;
using ACE.Server.Command;
using ACE.Server.Managers;
using System.Text;

namespace Tower;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    const int RETRIES = 10;

    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);

    private JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

        if (!settingsInfo.RetryWrite(jsonString, RETRIES))
        {
            ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
        }
    }

    private void LoadSettings()
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

    #region Start/Shutdown
    public void Start()
    {
        //Need to decide on async use
        Mod.State = ModState.Loading;
        LoadSettings();

        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        Mod.State = ModState.Running;
    }

    public void Shutdown()
    {
        //if (Mod.State == ModState.Running)
        // Shut down enabled mod...

        //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
        //SaveSettings();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    [CommandHandler("t1", AccessLevel.Admin, CommandHandlerFlag.None, -1)]
    public static void T1(Session session, params string[] parameters)
    {
        Test(session.Player);
    }


    private static void Test(Player p)
    {
        using (var ctx = new ShardDbContext())
        {

            var sb = new StringBuilder("\n");
            foreach (var player in PlayerManager.GetAllOffline().Where(x => p.Account.AccountId == x.Account.AccountId).ToList())
            {
                sb.Append($"{player.Name}\n");
            }
            p.SendMessage($"{sb}");
            //var results = ctx.p
            //    ctx.CharacterPropertiesShortcutBar
            //    .FromSqlRaw("SELECT * FROM character_properties_shortcut_bar ORDER BY character_Id, shortcut_Bar_Index, id")
            //    .ToList();

            //var sqlCommands = new List<string>();

            //uint characterId = 0;
            //string playerName = null;
            //var idxToObj = new Dictionary<uint, uint>();
            //var objToIdx = new Dictionary<uint, uint>();
            //var buggedChar = false;
            //var buggedPlayerCount = 0;

            //foreach (var result in results)
            //{
            //    if (characterId != result.CharacterId)
            //    {
            //        if (buggedChar)
            //        {
            //            buggedPlayerCount++;
            //            Console.WriteLine($"Player {playerName} ({characterId}) was found to have errors in their shortcuts.");
            //            sqlCommands.AddRange(OutputShortcutSQLCommand(playerName, characterId, idxToObj));
            //            buggedChar = false;
            //        }

            //        // begin parsing new character
            //        characterId = result.CharacterId;
            //        var player = PlayerManager.FindByGuid(characterId);
            //        playerName = player != null ? player.Name : $"{characterId:X8}";
            //        idxToObj = new Dictionary<uint, uint>();
            //        objToIdx = new Dictionary<uint, uint>();
            //    }

            //    var dupeIdx = idxToObj.ContainsKey(result.ShortcutBarIndex);
            //    var dupeObj = objToIdx.ContainsKey(result.ShortcutObjectId);

            //    if (dupeIdx || dupeObj)
            //    {
            //        //Console.WriteLine($"Player: {playerName}, Idx: {result.ShortcutBarIndex}, Obj: {result.ShortcutObjectId:X8} ({result.Id})");
            //        buggedChar = true;
            //    }

            //    objToIdx[result.ShortcutObjectId] = result.ShortcutBarIndex;

            //    if (!dupeObj)
            //        idxToObj[result.ShortcutBarIndex] = result.ShortcutObjectId;
            //}

            //if (buggedChar)
            //{
            //    Console.WriteLine($"Player {playerName} ({characterId}) was found to have errors in their shortcuts.");
            //    buggedPlayerCount++;
            //    sqlCommands.AddRange(OutputShortcutSQLCommand(playerName, characterId, idxToObj));
            //}

            //Console.WriteLine($"Total players found with bugged shortcuts: {buggedPlayerCount}");

            //if (execute)
            //{
            //    Console.WriteLine("Executing changes...");

            //    foreach (var cmd in sqlCommands)
            //        ctx.Database.ExecuteSqlRaw(cmd);
            //}

        }
        //foreach (var p in PlayerManager.GetAllOffline())
        //{
        //    //Only move regular players / restrict based on access level.  
        //    if (p.Account == null || p.Account.AccessLevel >= (uint)AccessLevel.Sentinel)
        //        continue;

        //    //Set Location position for a player to holt and save
        //    p.Biota.SetPosition(PositionType.Location, position, p.BiotaDatabaseLock);
        //    p.SaveBiotaToDatabase();
        //}

    }
}

