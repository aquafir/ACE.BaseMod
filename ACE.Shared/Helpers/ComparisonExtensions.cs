namespace ACE.Shared.Helpers;

public static class ComparisonExtensions
{
    public static bool Compare(this CompareType compareType, double? prop, double val, Player player, string failMsg = "")
    {
        //Todo: not sure I get the inverted comparisons..?
        var success = compareType switch
        {
            CompareType.GreaterThan => (prop ?? 0) > val,
            CompareType.LessThanEqual => (prop ?? 0) <= val,
            CompareType.LessThan => (prop ?? 0) < val,
            CompareType.GreaterThanEqual => (prop ?? 0) >= val,
            CompareType.NotEqual => (prop ?? 0) != val,
            CompareType.NotEqualNotExist => prop == null || prop.Value != val,
            CompareType.Equal => (prop ?? 0) == val,
            CompareType.NotExist => prop == null,
            CompareType.Exist => prop != null,
            CompareType.NotHasBits => ((int)(prop ?? 0) & (int)val) == 0,
            CompareType.HasBits => ((int)(prop ?? 0) & (int)val) == (int)val,
            _ => true,
        };
        //Debugger.Break();

        if (!success)
            player.Session.Network.EnqueueSend(new GameMessageSystemChat(failMsg, ChatMessageType.Craft));

        return success;
    }

    public static bool VerifyRequirement(this CompareType comparison, double? prop, double? targetValue)
    {

        return comparison switch
        {
            CompareType.GreaterThan => (prop ?? 0) > targetValue,
            CompareType.GreaterThanEqual => (prop ?? 0) >= targetValue,
            CompareType.LessThan => (prop ?? 0) < targetValue,
            CompareType.LessThanEqual => (prop ?? 0) <= targetValue,
            CompareType.NotEqual => (prop ?? 0) != targetValue,
            CompareType.Equal => (prop ?? 0) == targetValue,
            CompareType.NotEqualNotExist => (prop == null || prop.Value != targetValue),    //Todo, not certain about the inversion.  I'm tired.
            CompareType.NotExist => prop is null,
            CompareType.Exist => prop is not null,
            CompareType.NotHasBits => ((int)(prop ?? 0) & (int)targetValue) == 0,
            CompareType.HasBits => ((int)(prop ?? 0) & (int)targetValue) == (int)targetValue,
            _ => true,
        };
    }

    public static string Friendly(this CompareType type) => type switch
    {
        CompareType.GreaterThan => ">",
        CompareType.LessThanEqual => "<=",
        CompareType.LessThan => "<",
        CompareType.GreaterThanEqual => ">=",
        CompareType.NotEqual => "!=",
        CompareType.NotEqualNotExist => "!=??",
        CompareType.Equal => "==",
        CompareType.NotExist => "??",
        CompareType.Exist => "?",
        CompareType.NotHasBits => "!B",
        CompareType.HasBits => "B",
        _ => "",
    };
}