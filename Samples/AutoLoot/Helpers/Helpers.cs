namespace AutoLoot.Helpers;

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