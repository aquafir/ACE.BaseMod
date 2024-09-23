using ACE.Database.Entity;
using System.Collections.Concurrent;

namespace PlayerSave.Helpers;

public static class PlayerHelpers
{
    public static void SwapShortcuts(this Player newPlayer, ConcurrentDictionary<uint, uint> idSwaps)
    {
        if (newPlayer.Character.CharacterPropertiesShortcutBar != null)
        {
            foreach (var entry in newPlayer.Character.CharacterPropertiesShortcutBar)
            {
                if (idSwaps.ContainsKey(entry.ShortcutObjectId))
                    entry.ShortcutObjectId = idSwaps[entry.ShortcutObjectId];
            }
        }
    }
    public static void SwapEnchantmentRegistry(this Player newPlayer, ConcurrentDictionary<uint, uint> idSwaps)
    {
        if (newPlayer.Biota.PropertiesEnchantmentRegistry != null)
        {
            foreach (var entry in newPlayer.Biota.PropertiesEnchantmentRegistry)
            {
                if (idSwaps.ContainsKey(entry.CasterObjectId))
                    entry.CasterObjectId = idSwaps[entry.CasterObjectId];
            }
        }
    }



    public static byte[] CreateBinarySave(Character character, ACE.Database.Models.Shard.Biota playerBiota, PossessedBiotas possessions)
    {
        using MemoryStream ms = new();
        using BinaryWriter writer = new(ms);

        character.Write(writer);
        playerBiota.Write(writer);
        possessions.Inventory.Write(writer);
        possessions.WieldedItems.Write(writer);

        return ms.ToArray();
    }
}
