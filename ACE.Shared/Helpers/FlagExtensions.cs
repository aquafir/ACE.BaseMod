namespace ACE.Shared.Helpers;

public static class FlagExtensions
{
    //https://stackoverflow.com/questions/71931456/check-if-flag-contains-any-value-of-other-flag
    //public static bool HasAny(this Enum me, Enum other) => (Convert.ToInt32(me) & Convert.ToInt32(other)) != 0;
    public static bool HasAny<TEnum>(this TEnum me, TEnum other) where TEnum : Enum, IConvertible
        => (me.ToInt32(null) & other.ToInt32(null)) != 0;

    /// <summary>
    /// Returns an array of the non-composite flags making up a composite flag
    /// </summary>
    public static TEnum[] GetIndividualFlags<TEnum>(this TEnum enumValue) where TEnum : Enum
    {
        //Filter out the individual flags
        var individualFlags = GetIndividualFlags<TEnum>();
        
        return individualFlags.Where(x => enumValue.HasFlag(x)).ToArray();
    }


    /// <summary>
    /// Finds the flags of an Enum by checking if they're powers of two and non-zero
    /// </summary>
    public static TEnum[] GetIndividualFlags<TEnum>() where TEnum : Enum
    {
        //Todo: make this an extension that works on generic Enum?  Couldn't figure out how
        // Get all the values of the enum type
        var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        // Filter out the individual flags
        return values.Where(value => IsPowerOfTwo(Convert.ToInt64(value))).ToArray();
    }

    /// <summary>
    /// Checks if a value is non-zero and a power of two to determine if it involves a single bit
    /// </summary>
    public static bool IsIndividualFlag<TEnum>(this TEnum enumValue) where TEnum : Enum, IConvertible
        => IsPowerOfTwo(enumValue.ToInt64(null));

    private static bool IsPowerOfTwo(long x) => (x != 0) && ((x & (x - 1)) == 0);

}
