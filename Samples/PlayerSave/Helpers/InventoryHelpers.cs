using System.Collections.Concurrent;
using Biota = ACE.Database.Models.Shard.Biota;

namespace PlayerSave.Helpers;

public static class InventoryHelpers
{
    /// <summary>
    /// Create a new dynamic GUID for each item and add a map from the old ID to the new ID
    /// </summary>
    /// <param name="possessions">Items to assign a new ID to</param>
    /// <param name="idSwaps">Old ID keys pointing to new ID values </param>
    public static void AssignNewGuids(this List<Biota> possessions, ref ConcurrentDictionary<uint, uint> idSwaps, bool verbose = false)
    {
        var sb = new StringBuilder($"\nContainers added:\n  Player: {idSwaps.FirstOrDefault().Value}\n");
        foreach (var item in possessions)
        {
            //Give each item a new ID and map their old one to it for use with shortcuts, etc.
            var newGuid = GuidManager.NewDynamicGuid();
            idSwaps[item.Id] = newGuid.Full;
            item.Id = newGuid.Full;

            if (verbose && item.WeenieType == (int)WeenieType.Container)
                sb.AppendLine($"  {item.GetProperty(PropertyString.Name),-20}WCID: {item.WeenieClassId,-8}ID: {item.Id}");
        }
        if (verbose)
            ModManager.Log(sb.ToString());
    }

    /// <summary>
    /// Changes references for possessed items to a new owner.  Old method converted DB->Entity->Changed->DB
    /// </summary>
    /// <param name="possessions">Items to change references for</param>
    /// <param name="newOwner">New owner to use the name and ID of</param>
    /// <param name="idSwaps">Map from old IDs to newly assigned ones</param>
    public static void ChangeOwner(this List<Biota> possessions, Character newOwner, ConcurrentDictionary<uint, uint> idSwaps, bool verbose = false)
    {
        var sb = new StringBuilder($"\nChanged:\n");

        foreach (var item in possessions)
        {
            //Update enchantments?  Maybe just remove them
            foreach (var entry in item.BiotaPropertiesEnchantmentRegistry)
            {
                if (idSwaps.ContainsKey(entry.CasterObjectId))
                    entry.CasterObjectId = idSwaps[entry.CasterObjectId];
            }

            //Wielded items don't have container (and owner?) IIDs
            var wielderId = item.GetProperty(PropertyInstanceId.Wielder);
            if (wielderId != null && idSwaps.ContainsKey(wielderId.Value))
            {
                item.SetProperty(PropertyInstanceId.Wielder, newOwner.Id);
                if (verbose)
                    sb.AppendLine($"  Wield: {item.GetProperty(PropertyString.Name)}");
            }
            else
            {
                //Container equal to biota/character ID if main pack. All updated IDs should be in ID swaps
                var containerId = item.GetProperty(PropertyInstanceId.Container) ?? 0;
                if (idSwaps.TryGetValue(containerId, out var newContainer))
                {
                    item.SetProperty(PropertyInstanceId.Container, newContainer);

                    if (verbose)
                        sb.AppendLine($"  {(newContainer == newOwner.Id ? "Main" : "Side")}: {item.GetProperty(PropertyString.Name)}");
                }
                else
                {
                    Debugger.Break();
                    //throw new Exception();
                    //item.SetProperty(PropertyInstanceId.Container, newOwner.Id);
                    //if (verbose)
                    //    sb.AppendLine($"  Main: {item.GetProperty(PropertyString.Name)}");
                }

                //Just set owner id of all to new owner?
                var ownerId = item.GetProperty(PropertyInstanceId.Owner);
                if (ownerId != null && idSwaps.ContainsKey(ownerId.Value))
                    item.SetProperty(PropertyInstanceId.Owner, ownerId.Value);
            }

            var activatorId = item.GetProperty(PropertyInstanceId.AllowedActivator);
            if (activatorId != null && idSwaps.ContainsKey(activatorId.Value))
            {
                item.SetProperty(PropertyInstanceId.AllowedActivator, activatorId.Value);
                item.SetProperty(PropertyString.CraftsmanName, newOwner.Name);
            }

            var allowedWielderId = item.GetProperty(PropertyInstanceId.AllowedWielder);
            if (allowedWielderId != null && idSwaps.ContainsKey(allowedWielderId.Value))
            {
                item.SetProperty(PropertyInstanceId.AllowedWielder, allowedWielderId.Value);
                item.SetProperty(PropertyString.CraftsmanName, newOwner.Name);
            }
        }

        if (verbose)
            ModManager.Log(sb.ToString());
        //return newItems;
    }

    public static List<Biota> ChangeOwnerOld(
        this List<Biota> possessions,
        Character newOwner,
        List<ACE.Entity.Models.Biota> tempItems,
        ConcurrentDictionary<uint, uint> idSwaps)
    {
        List<Biota> newItems = new();

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

    //Not converting to entity and back to DB anymore
    /// <summary>
    /// Convert a list of Biotas from Database to Entity and  create and fills a collection of IDs
    /// </summary>
    //public static List<ACE.Entity.Models.Biota> CreateEntityCopies(this List<Biota> items, ref ConcurrentDictionary<uint, uint> idSwaps)
    //{
    //    var newTempWieldedItems = new List<ACE.Entity.Models.Biota>();
    //    foreach (var item in items)
    //    {
    //        //Todo, rethink this
    //        if (item.WeenieClassId == (uint)WeenieClassName.W_DEED_CLASS)
    //            continue;

    //        var newItemBiota = ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota(item);
    //        var newGuid = GuidManager.NewDynamicGuid();
    //        idSwaps[newItemBiota.Id] = newGuid.Full;
    //        newItemBiota.Id = newGuid.Full;
    //        newTempWieldedItems.Add(newItemBiota);
    //    }

    //    return newTempWieldedItems;
    //}
}