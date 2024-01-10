using ACE.Server.Network.GameMessages.Messages;

namespace Tinkering;

public static class ComparisonHelpers
{
    public static bool Compare(this CompareType compareType, double? prop, double val, Player player, string failMsg = "")
    {
        //Todo: not sure I get the inverted comparisons..?
        var success = compareType switch
        {
            CompareType.GreaterThan => ((prop ?? 0) > val),
            CompareType.LessThanEqual => ((prop ?? 0) <= val),
            CompareType.LessThan => ((prop ?? 0) < val),
            CompareType.GreaterThanEqual => ((prop ?? 0) >= val),
            CompareType.NotEqual => ((prop ?? 0) != val),
            CompareType.NotEqualNotExist => (prop == null || prop.Value != val),
            CompareType.Equal => ((prop ?? 0) == val),
            CompareType.NotExist => (prop == null),
            CompareType.Exist => (prop != null),
            CompareType.NotHasBits => (((int)(prop ?? 0) & (int)val) == 0),
            CompareType.HasBits => (((int)(prop ?? 0) & (int)val) == (int)val),
            _ => true,
        };
        //Debugger.Break();

        if (!success)
            player.Session.Network.EnqueueSend(new GameMessageSystemChat(failMsg, ChatMessageType.Craft));

        return success;
    }
}