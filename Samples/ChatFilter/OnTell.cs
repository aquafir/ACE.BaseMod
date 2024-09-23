using ACE.Server.Network.GameAction.Actions;

namespace ChatFilter;

[HarmonyPatchCategory(Settings.TellCategory)]
internal static class OnTell
{
    //Rewrite of GameActionTell with a filter check added
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameActionTell), nameof(GameActionTell.Handle), new Type[] { typeof(ClientMessage), typeof(Session) })]
    public static bool PreHandle(ClientMessage clientMessage, Session session)
    {
        var message = clientMessage.Payload.ReadString16L(); // The client seems to do the trimming for us
        var target = clientMessage.Payload.ReadString16L(); // Needs to be trimmed because it may contain white spaces after the name and before the ,

        if (PatchClass.Settings.FilterTells)
        {
            if (PatchClass.TryHandleToxicity(ref message, session.Player, ChatSource.Tell, target))
                return false;
        }

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
            session.Network.EnqueueSend(new GameEventWeenieErrorWithString(session, WeenieErrorWithString.AFK, $"{targetPlayer.Name} is away: " + (string.IsNullOrWhiteSpace(targetPlayer.AfkMessage) ? Player.DefaultAFKMessage : targetPlayer.AfkMessage)));
            //return;
        }

        var tell = new GameEventTell(targetPlayer.Session, message, session.Player.GetNameWithSuffix(), session.Player.Guid.Full, targetPlayer.Guid.Full, ChatMessageType.Tell);
        targetPlayer.Session.Network.EnqueueSend(tell);

        return false;
    }
}