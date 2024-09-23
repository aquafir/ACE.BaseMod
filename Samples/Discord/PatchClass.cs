using ACE.Common.Extensions;
using ACE.Server.Network.GameAction.Actions;
using ACE.Server.Network.GameMessages.Messages;
using System.Text.Encodings.Web;

namespace Tinkering;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
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
    public static Discord DiscordRelay;
    public Discord Relay;

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

        //Start Dscord
        DiscordRelay = new Discord();
        Relay = DiscordRelay;
        DiscordRelay.RunAsync()
            .GetAwaiter()
            .GetResult();


        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        Mod.State = ModState.Running;
    }

    public void Shutdown()
    {
        if (Mod.State == ModState.Running)
            Task.Run(async () => await DiscordRelay.Shutdown());

        //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
        //SaveSettings();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameMessageTurbineChat), MethodType.Constructor,
       new Type[] { typeof(ChatNetworkBlobType), typeof(ChatNetworkBlobDispatchType), typeof(uint), typeof(string), typeof(string), typeof(uint), typeof(ChatType) })]
    public static void HandleTurbineChatRelay(ChatNetworkBlobType chatNetworkBlobType, ChatNetworkBlobDispatchType chatNetworkBlobDispatchType, uint channel, string senderName, string message, uint senderID, ChatType chatType)
    {
        try
        {
            //ModManager.Log($"Routing message from {senderName}:\n\t{message}");
            DiscordRelay.RelayIngameChat(message, senderName, chatType, channel, senderID, chatNetworkBlobType, chatNetworkBlobDispatchType);
        }
        catch (Exception ex)
        {
            ModManager.Log(ex.Message, ModManager.LogLevel.Error);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameActionTell), nameof(GameActionTell.Handle), new Type[] { typeof(ClientMessage), typeof(Session) })]
    public static bool HandleTellRelay(ClientMessage clientMessage, Session session)
    {
        try
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
                //ModManager.Log($"Trying to message offline player {target} through Discord:\n  {msg}");
                DiscordRelay.RelayIngameDirectMessage(target, msg, session);
                return false;
            }
        }
        catch (Exception ex)
        {
            ModManager.Log(ex.Message, ModManager.LogLevel.Error);
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.SendChatMessage), new Type[] { typeof(WorldObject), typeof(string), typeof(ChatMessageType) })]
    public static bool SendChatMessage(WorldObject source, string msg, ChatMessageType msgType)
    {
        if (PlayerManager.GetOnlinePlayer(source.Guid) is null)
        {
            ModManager.Log("Trying to message offline player through Discord");
            return false;
        }

        return true;
    }


    public static Discord GetDiscordRelay() => DiscordRelay;
}

