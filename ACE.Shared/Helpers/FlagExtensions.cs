namespace ACE.Shared.Helpers;

public static class FlagExtensions
{
    //https://stackoverflow.com/questions/71931456/check-if-flag-contains-any-value-of-other-flag
    //public static bool HasAny(this Enum me, Enum other) => (Convert.ToInt32(me) & Convert.ToInt32(other)) != 0;
    public static bool HasAny<TEnum>(this TEnum me, TEnum other) where TEnum : Enum, IConvertible
        => (me.ToInt32(null) & other.ToInt32(null)) != 0;
}
