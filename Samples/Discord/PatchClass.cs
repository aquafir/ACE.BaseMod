using ACE.Server.Network.GameAction.Actions;

namespace Discord;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public static Discord DiscordRelay;
    public Discord Relay;

    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;

        //Start Dscord
        DiscordRelay = new Discord();
        Relay = DiscordRelay;
        DiscordRelay.RunAsync()
            .GetAwaiter()
            .GetResult();
    }

    public override void Stop()
    {
        base.Stop();
        if (ModC.State == ModState.Running)
            Task.Run(async () => await DiscordRelay.Shutdown());
    }

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

