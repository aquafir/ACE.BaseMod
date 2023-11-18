namespace ChatFilter;

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

        SetupFilter();

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


    static ProfanityFilter.ProfanityFilter filter = new();

    private void SetupFilter()
    {
        filter = new ProfanityFilter.ProfanityFilter();
    }


    //Rewrite of GameActionTell with a filter check added
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameActionTell), nameof(GameActionTell.Handle), new Type[] { typeof(ClientMessage), typeof(Session) })]
    public static bool PreHandle(ClientMessage clientMessage, Session session)
    {
        var message = clientMessage.Payload.ReadString16L(); // The client seems to do the trimming for us
        var target = clientMessage.Payload.ReadString16L(); // Needs to be trimmed because it may contain white spaces after the name and before the ,

        if (TryHandleToxicity(message, session.Player))
            return false;

        if (session.Player.IsGagged)
        {
            session.Player.SendGagError();
            return false;
        }

        target = target.Trim();
        var targetPlayer = PlayerManager.GetOnlinePlayer(target);

        if (targetPlayer == null)
        {
            var statusMessage = new GameEventWeenieError(session, WeenieError.CharacterNotAvailable);
            session.Network.EnqueueSend(statusMessage);
            return false;
        }

        if (session.Player != targetPlayer)
            session.Network.EnqueueSend(new GameMessageSystemChat($"You tell {targetPlayer.Name}, \"{message}\"", ChatMessageType.OutgoingTell));

        if (targetPlayer.SquelchManager.Squelches.Contains(session.Player, ChatMessageType.Tell))
        {
            session.Network.EnqueueSend(new GameEventWeenieErrorWithString(session, WeenieErrorWithString.MessageBlocked_, $"{target} has you squelched."));
            //log.Warn($"Tell from {session.Player.Name} (0x{session.Player.Guid.ToString()}) to {targetPlayer.Name} (0x{targetPlayer.Guid.ToString()}) blocked due to squelch");
            return false;
        }

        if (targetPlayer.IsAfk)
        {
            session.Network.EnqueueSend(new GameEventWeenieErrorWithString(session, WeenieErrorWithString.AFK, $"{targetPlayer.Name} is away: " + (string.IsNullOrWhiteSpace(targetPlayer.AfkMessage) ? ACE.Server.WorldObjects.Player.DefaultAFKMessage : targetPlayer.AfkMessage)));
            //return;
        }

        var tell = new GameEventTell(targetPlayer.Session, message, session.Player.GetNameWithSuffix(), session.Player.Guid.Full, targetPlayer.Guid.Full, ChatMessageType.Tell);
        targetPlayer.Session.Network.EnqueueSend(tell);

        return false;
    }


    //White text
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionTalk), new Type[] { typeof(string) })]
    public static bool PreHandleActionTalk(string message, ref Player __instance)
    {
        if (TryHandleToxicity(message, __instance))
            return false;

        return true;
    }

    private static bool TryHandleToxicity(string message, Player player)
    {
        if (player.IsShadowBanned())
            return true;


        if (filter.ContainsProfanity(message))
        {
            player.ShadowBan();

            player.SendGagError();
            message = filter.CensorString(message);

            player.SendMessage($"Banned for: {message}");

            var chain = new ActionChain();
            chain.AddDelaySeconds(3);
            chain.AddAction(player, () => player.Session.LogOffPlayer(true));
            chain.EnqueueChain();

            return true;
        }

        return false;
    }
}
