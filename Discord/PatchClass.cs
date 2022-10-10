using ACE.Entity.Enum;
using ACE.Server.Network.GameMessages.Messages;

namespace Discord
{
    [HarmonyPatch]
    public class PatchClass
    {
        //Todo: figure out what correct signatures don't work while explicit patching does

        //ChatNetworkBlobType chatNetworkBlobType, ChatNetworkBlobDispatchType chatNetworkBlobDispatchType, uint channel, string senderName, string message, uint senderID, ChatType chatType
        //[HarmonyPatch(typeof(GameMessageTurbineChat), 
        //    MethodType.Constructor, new Type[] { 
        //        typeof(ChatNetworkBlobType),
        //        typeof(ChatNetworkBlobDispatchType),
        //        typeof(uint),
        //        typeof(string),
        //        typeof(string),
        //        typeof(uint),
        //        typeof(ChatType)
        //    })]
        public static void Prefix(ChatNetworkBlobType chatNetworkBlobType, ChatNetworkBlobDispatchType chatNetworkBlobDispatchType, uint channel, string senderName, string message, uint senderID, ChatType chatType)
        {
            ModManager.Log($"Routing message from {senderName}:\n\t{message}");
            DiscordRelay.RelayIngameChat(message, senderName, chatType, channel, senderID, chatNetworkBlobType, chatNetworkBlobDispatchType);
        }
    }
}