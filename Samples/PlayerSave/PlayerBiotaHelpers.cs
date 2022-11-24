using ACE.Database.Models.Shard;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using System.Collections.Concurrent;

namespace PlayerSave;

public static class PlayerBiotaHelpers
{
    public static ACE.Entity.Models.Biota CopyBiotaAs(this ACE.Database.Models.Shard.Biota playerBiota, Character newCharacter, LoadOptions options)
    {
        //Create a new biota for a player
        var newPlayerBiota = ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota(playerBiota);
        newPlayerBiota.Id = newCharacter.Id;

        //Clear as needed
        newPlayerBiota.PropertiesAllegiance?.Clear();
        newPlayerBiota.HousePermissions?.Clear();

        if (!options.IncludeEnchantments)
            newPlayerBiota.PropertiesEnchantmentRegistry?.Clear();

        if (options.ResetPositions)
        {
            newPlayerBiota.PropertiesPosition = new Dictionary<PositionType, PropertiesPosition>()
            {
                { PositionType.Location, LoadOptions.DefaultPosition },
                { PositionType.Instantiation, LoadOptions.DefaultPosition },
                { PositionType.Sanctuary, LoadOptions.DefaultPosition }
            };

        }

        return newPlayerBiota;
    }

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

    public static Player CreatePlayer(this ACE.Entity.Models.Biota newPlayerBiota,
        List<ACE.Database.Models.Shard.Biota> newInventoryItems,
        List<ACE.Database.Models.Shard.Biota> newWieldedItems,
        Character newCharacter,
        Session session)
    {
        Player newPlayer;
        if (newPlayerBiota.WeenieType == WeenieType.Admin)
            newPlayer = new Admin(newPlayerBiota, newInventoryItems, newWieldedItems, newCharacter, session);
        else if (newPlayerBiota.WeenieType == WeenieType.Sentinel)
            newPlayer = new Sentinel(newPlayerBiota, newInventoryItems, newWieldedItems, newCharacter, session);
        else
            newPlayer = new Player(newPlayerBiota, newInventoryItems, newWieldedItems, newCharacter, session);

        newPlayer.Name = newCharacter.Name;
        newPlayer.ChangesDetected = true;
        newPlayer.CharacterChangesDetected = true;

        newPlayer.Allegiance = null;
        newPlayer.AllegianceOfficerRank = null;
        newPlayer.MonarchId = null;
        newPlayer.PatronId = null;
        newPlayer.HouseId = null;
        newPlayer.HouseInstance = null;

        return newPlayer;
    }

    //public static List<ACE.Database.Models.Shard.Biota> CopyInventoryAs(
    //    this PossessedBiotas existingPossessions,
    //    Character newOwner,
    //    ref ConcurrentDictionary<uint, uint> idSwaps) =>
    //    ChangeOwner(existingPossessions.Inventory, newOwner, ref idSwaps);

    //public static List<ACE.Database.Models.Shard.Biota> CopyWieldedItemsAs(
    //    this PossessedBiotas existingPossessions,
    //    Character newOwner,
    //    ref ConcurrentDictionary<uint, uint> idSwaps) =>
    //    ChangeOwner(existingPossessions.WieldedItems, newOwner, ref idSwaps);
}