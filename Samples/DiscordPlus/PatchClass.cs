using ACE.Common.Extensions;
using ACE.Server.Managers;
using ACE.Server.Network.GameAction.Actions;
using ACE.Server.Network.GameMessages.Messages;

namespace DiscordPlus;

[HarmonyPatch]
public class PatchClass
{
    private static DiscordRelay _relay = new();

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
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
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
        //_relay.Initialize();
        //Task.Run(async () => _relay.Initialize());
        _relay.RunAsync().GetAwaiter().GetResult();

        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        Mod.State = ModState.Running;
    }

    public static void Shutdown()
    {
        if (Mod.State == ModState.Running)
        {
            Task.Run(async () => await _relay.Shutdown());
        }

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameMessageTurbineChat), MethodType.Constructor,
        new Type[] { typeof(ChatNetworkBlobType), typeof(ChatNetworkBlobDispatchType), typeof(uint), typeof(string), typeof(string), typeof(uint), typeof(ChatType) })]
    public static void HandleTurbineChatRelay(ChatNetworkBlobType chatNetworkBlobType, ChatNetworkBlobDispatchType chatNetworkBlobDispatchType, uint channel, string senderName, string message, uint senderID, ChatType chatType)
    {
        ModManager.Log($"Routing message from {senderName}:\n\t{message}");
        _relay.RelayIngameChat(message, senderName, chatType, channel, senderID, chatNetworkBlobType, chatNetworkBlobDispatchType);
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameActionTell), nameof(GameActionTell.Handle), new Type[] { typeof(ClientMessage), typeof(Session) })]
    public static bool HandleTellRelay(ClientMessage clientMessage, Session session)
    {
        //Todo: think about ways to reduce redundancy
        var position = clientMessage.Payload.BaseStream.Position;
        var msg = clientMessage.Payload.ReadString16L(); 
        var target = clientMessage.Payload.ReadString16L();
        //Rewind what was read
        clientMessage.Payload.BaseStream.Position = position;
        target = target.Trim();

        if (PlayerManager.GetOnlinePlayer(target) is null)
        {
            ModManager.Log($"Trying to message offline player {target} through Discord:\n  {msg}");
            _relay.RelayIngameDirectMessage(target, msg, session);
            return false;
        }

        return true;
    }



    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), nameof(Player.SendChatMessage), new Type[] { typeof(WorldObject), typeof(string), typeof(ChatMessageType) })]
    //public static bool SendChatMessage(WorldObject source, string msg, ChatMessageType msgType)
    //{
    //    Debugger.Break();
    //    if(PlayerManager.GetOnlinePlayer(source.Guid) is null)
    //    {
    //        ModManager.Log("Trying to message offline player through Discord");
    //        return false;
    //    }

    //    return true;
    //}
    #endregion
}