using ACE.Database.Entity;
using ACE.Database.Models.Shard;
using ACE.Entity.Enum.Properties;
using ACE.Server.Managers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AccessDb
{
    public static class PlayerSaveHelpers
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


        /// <summary>
        /// Changes references to a new owner.  Old method converted DB->Entity->Changed->DB
        /// </summary>
        public static void ChangeOwner(
            this List<ACE.Database.Models.Shard.Biota> possessions,
            Character newOwner,
            ref ConcurrentDictionary<uint, uint> idSwaps)
        {
            foreach (var item in possessions)
            {
                //Give each item a new ID and map their old one to it for use with shortcuts, etc.
                var newGuid = GuidManager.NewDynamicGuid();
                idSwaps[item.Id] = newGuid.Full;
                item.Id = newGuid.Full;
            }

            foreach (var item in possessions)
            {
                //Update enchantments?  Maybe just remove them
                foreach (var entry in item.BiotaPropertiesEnchantmentRegistry)
                {
                    if (idSwaps.ContainsKey(entry.CasterObjectId))
                        entry.CasterObjectId = idSwaps[entry.CasterObjectId];
                }

                //Skip deeds from the copychar method?
                //if (item.WeenieClassId == (uint)WeenieClassName.W_DEED_CLASS)
                //continue;

                //Check for a container different from the new player?  Any need for a null check?

                //Wielded items don't have container (and owner?) IIDs
                var wielderId = item.GetProperty(PropertyInstanceId.Wielder);
                if (wielderId != null && idSwaps.ContainsKey(wielderId.Value))
                    item.SetProperty(PropertyInstanceId.Wielder, wielderId.Value);
                else
                {
                    //Container, equal to biota/character ID if main pack
                    var containerId = item.GetProperty(PropertyInstanceId.Container) ?? 0;
                    if (idSwaps.TryGetValue(containerId, out var newContainer)) // && item.WeenieType != (int)WeenieType.Container)
                        item.SetProperty(PropertyInstanceId.Container, newContainer);
                    else
                        item.SetProperty(PropertyInstanceId.Container, newOwner.Id);
                }

                var ownerId = item.GetProperty(PropertyInstanceId.Owner);
                if (ownerId != null && idSwaps.ContainsKey(ownerId.Value))
                    item.SetProperty(PropertyInstanceId.Owner, ownerId.Value);

                var activatorId = item.GetProperty(PropertyInstanceId.AllowedActivator);
                if (activatorId != null && idSwaps.ContainsKey(activatorId.Value))
                {
                    item.SetProperty(PropertyInstanceId.AllowedActivator, activatorId.Value);
                    item.SetProperty(PropertyString.CraftsmanName, newOwner.Name);
                }

                var allowedWielderId = item.GetProperty(PropertyInstanceId.AllowedWielder);
                if (allowedWielderId != null && idSwaps.ContainsKey(allowedWielderId.Value)) { 
                    item.SetProperty(PropertyInstanceId.AllowedWielder, allowedWielderId.Value);
                    item.SetProperty(PropertyString.CraftsmanName, newOwner.Name);
                }
            }

            //return newItems;
        }

        public static List<ACE.Database.Models.Shard.Biota> ChangeOwnerOld(
            this List<ACE.Database.Models.Shard.Biota> possessions,
            Character newOwner,
            List<ACE.Entity.Models.Biota> tempItems,
            ConcurrentDictionary<uint, uint> idSwaps)
        {
            List<ACE.Database.Models.Shard.Biota> newItems = new();           

            foreach (var item in tempItems)
            {
                if (item.PropertiesEnchantmentRegistry != null)
                {
                    foreach (var entry in item.PropertiesEnchantmentRegistry)
                    {
                        if (idSwaps.ContainsKey(entry.CasterObjectId))
                            entry.CasterObjectId = idSwaps[entry.CasterObjectId];
                    }
                }

                if (item.PropertiesIID != null)
                {
                    if (item.PropertiesIID.TryGetValue(PropertyInstanceId.Owner, out var ownerId))
                    {
                        if (idSwaps.ContainsKey(ownerId))
                        {
                            item.PropertiesIID.Remove(PropertyInstanceId.Owner);
                            item.PropertiesIID.Add(PropertyInstanceId.Owner, idSwaps[ownerId]);
                        }
                    }

                    if (item.PropertiesIID.TryGetValue(PropertyInstanceId.Wielder, out var wielderId))
                    {
                        if (idSwaps.ContainsKey(wielderId))
                        {
                            item.PropertiesIID.Remove(PropertyInstanceId.Wielder);
                            item.PropertiesIID.Add(PropertyInstanceId.Wielder, idSwaps[wielderId]);
                        }
                    }

                    if (item.PropertiesIID.TryGetValue(PropertyInstanceId.AllowedActivator, out var allowedActivatorId))
                    {
                        if (idSwaps.ContainsKey(allowedActivatorId))
                        {
                            item.PropertiesIID.Remove(PropertyInstanceId.AllowedActivator);
                            item.PropertiesIID.Add(PropertyInstanceId.AllowedActivator, idSwaps[allowedActivatorId]);

                            item.PropertiesString.Remove(PropertyString.CraftsmanName);
                            item.PropertiesString.Add(PropertyString.CraftsmanName, newOwner.Name);
                        }
                    }

                    if (item.PropertiesIID.TryGetValue(PropertyInstanceId.AllowedWielder, out var allowedWielderId))
                    {
                        if (idSwaps.ContainsKey(allowedWielderId))
                        {
                            item.PropertiesIID.Remove(PropertyInstanceId.AllowedWielder);
                            item.PropertiesIID.Add(PropertyInstanceId.AllowedWielder, idSwaps[allowedWielderId]);

                            item.PropertiesString.Remove(PropertyString.CraftsmanName);
                            item.PropertiesString.Add(PropertyString.CraftsmanName, newOwner.Name);
                        }
                    }
                }

                newItems.Add(ACE.Database.Adapter.BiotaConverter.ConvertFromEntityBiota(item));

            }
            return newItems;
        }


        /// <summary>
        /// Convert a list of Biotas from Database to Entity and  create and fills a collection of IDs
        /// </summary>
        public static List<ACE.Entity.Models.Biota> CreateEntityCopies(this List<Biota> items, ref ConcurrentDictionary<uint, uint> idSwaps)
        {
            var newTempWieldedItems = new List<ACE.Entity.Models.Biota>();
            foreach (var item in items)
            {
                //Todo, rethink this
                if (item.WeenieClassId == (uint)WeenieClassName.W_DEED_CLASS)
                    continue;

                var newItemBiota = ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota(item);
                var newGuid = GuidManager.NewDynamicGuid();
                idSwaps[newItemBiota.Id] = newGuid.Full;
                newItemBiota.Id = newGuid.Full;
                newTempWieldedItems.Add(newItemBiota);
            }

            return newTempWieldedItems;
        }
    }
}
