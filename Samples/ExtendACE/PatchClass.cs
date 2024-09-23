using ACE.Server.Command.Handlers;

namespace ExtendACE;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
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

        Mod.State = ModState.Running;
    }

    public static void Shutdown()
    {
        //if (Mod.State == ModState.Running)
        // Shut down enabled mod...

        //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
        //SaveSettings();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    #region Patches
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    //public static bool PreDamageTarget(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    //{
    //    if (target is Player) return true;

    //    var cullThreshold = damageSource.GetProperty(PropertyFloatEx.Cull);
    //    if(cullThreshold is null || ((float)target.Health.Current / target.Health.MaxValue) > cullThreshold) return true;

    //    __result.Damage = target.Health.Current;
    //    __result.
    //    if (damageSource.SetProperty(PropertyFloatEx.Cull)
    //    //Return false to override
    //    //return false;

    //    //Return true to execute original
    //    return true;
    //}

    #endregion

    //[CommandHandler("clearquest", AccessLevel.Admin, CommandHandlerFlag.None, 0)]
    //public static void HandleQuestClear(Session session, params string[] parameters)
    //{
    //    DatabaseManager.World.ClearCachedQuest(parameters[0]);
    //}

    //[CommandHandler("cq", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    //public static void HandleCheckQuest(Session session, params string[] parameters)
    //{
    //    //var quest = DatabaseManager.World.GetCachedQuest("stipendtimer_0812");
    //    var p = session.Player;
    //    var quests = p.QuestManager.GetQuests().Where(x => x.QuestName.Contains("0812"));

    //    foreach (var playerQuest in quests)
    //    {
    //        var questName = QuestManager.GetQuestName(playerQuest.QuestName);
    //        DatabaseManager.World.ClearCachedQuest(questName);
    //        var quest = DatabaseManager.World.GetCachedQuest(questName);
    //        var nextSolve = p.QuestManager.GetNextSolveTime(questName);
    //        var hours = nextSolve.TotalHours;
    //        ModManager.Log($"{questName} can be solved in {hours} hours, adding delta {quest.MinDelta} to last solve {playerQuest.LastTimeCompleted}");
    //        //Debugger.Break();
    //    }
    //}

    static Creatures.CreatureType[] types = Enum.GetValues<Creatures.CreatureType>();
    static string availableTypes = string.Join('\n', types.Select(x => $"  {x.ToString()} - {(int)x}"));
    [CommandHandler("cex", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld)]
    public static void HandleCreateEx(Session session, params string[] parameters)
    {
        //Check parameters
        if (parameters.Length != 2)
        {
            session.Player.SendMessage($"Available types are: \n{availableTypes}");
            return;
        }

        //Check valid CreatureEx type
        //todo: allow use of enum value
        var name = parameters[0];
        if (!types.Any(x => x.ToString().Contains(parameters[0], StringComparison.InvariantCultureIgnoreCase)))
        {
            session.Player.SendMessage($"Available types are: \n{availableTypes}");
            return;
        }

        var weenie = AdminCommands.GetWeenieForCreate(session, parameters[1]);
        if (weenie is null)
        {
            session.Player.SendMessage($"Provide a valid weenie ID: {parameters[1]}");
            return;
        }

        var type = types.Where(x => x.ToString().Contains(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        var creature = type.Create(weenie, GuidManager.NewDynamicGuid());
        creature.MoveInFrontOf(session.Player);
        creature.EnterWorld();

        session.Player.SendMessage($"{creature.Name} created.");
    }

    [CommandHandler("hp", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleHp(Session session, params string[] parameters)
    {
        session.Player.Health.StartingValue = 10000000;
    }
}