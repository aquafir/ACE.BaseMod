namespace ACE.Shared.Helpers;

public static class EnchantmentExtensions
{
    /// <summary>
    /// Returns a random enchantment?
    /// </summary>
    public static bool TryGetRandomEnchantment(this Creature creature, out PropertiesEnchantmentRegistry random)
    {
        ICollection<PropertiesEnchantmentRegistry> value = creature.Biota.PropertiesEnchantmentRegistry;
        ReaderWriterLockSlim rwLock = creature.BiotaDatabaseLock;

        random = null;
        if (value == null)
            return false;

        rwLock.EnterReadLock();
        try
        {
            random = value.GetRandom();
        }
        finally
        {
            rwLock.ExitReadLock();
        }

        return random is not null;
    }
}
