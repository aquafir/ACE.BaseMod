namespace CustomLoot.Helpers;

public static class Helpers
{
    public static void SetCloakSpellProc(this WorldObject wo, SpellId spellId)
    {
        if (spellId != SpellId.Undef)
        {
            wo.ProcSpell = (uint)spellId;
            wo.ProcSpellSelfTargeted = spellId.IsSelfTargeting();
            wo.CloakWeaveProc = 1;
        }
        else
        {
            // Damage Reduction proc?
            wo.CloakWeaveProc = 2;
        }
    }

    //Todo: decide whether I need to create an instance of the spell to check?
    //CloakAllId was the original cloak check
    //Aetheria uses a lookup
    public static bool IsSelfTargeting(this SpellId spellId) => new Spell(spellId).IsSelfTargeted; //spellId == SpellId.CloakAllSkill;
}

public static class FlagExtensions
{
    //https://stackoverflow.com/questions/71931456/check-if-flag-contains-any-value-of-other-flag
    //public static bool HasAny(this Enum me, Enum other) => (Convert.ToInt32(me) & Convert.ToInt32(other)) != 0;
    public static bool HasAny<TEnum>(this TEnum me, TEnum other) where TEnum : Enum, IConvertible
        => (me.ToInt32(null) & other.ToInt32(null)) != 0;
}

public static class RandomExtensions
{
    private static Random random = new Random();

    public static bool TryGetRandom<T>(this T[] array, out T value)
    {
        value = default;

        if (array == null || array.Length == 0)
            return false;

        value = array[random.Next(array.Length)];
        return true;
    }
}

public static class NormalizeExtensions
{
    public static double? Normalize(this bool? value) => value != null ? (double?)Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this int? value) => value != null ? (double?)Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this long? value) => value != null ? (double?)Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this double? value) => value != null ? (double?)Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this float? value) => value != null ? (double?)Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this uint? value) => value != null ? (double?)Convert.ToDouble(value.Value) : null;

    public static double? Normalize(this bool value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this int value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this long value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this double value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this float value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this uint value) => (double?)Convert.ToDouble(value);
}

public static class ComparisonExtensions
{
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