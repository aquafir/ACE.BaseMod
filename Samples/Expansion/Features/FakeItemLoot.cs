namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeItemLoot))]
[HarmonyPatchCategory(nameof(Feature.FakeItemLoot))]
internal class FakeItemLoot
{
    // readonly Dictionary<(uint, )> 

    //Override method
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GenerateTreasure), new Type[] { typeof(DamageHistoryInfo), typeof(Corpse) })]
    public static bool PreGenerateTreasure(DamageHistoryInfo killer, Corpse corpse, ref Creature __instance, ref List<WorldObject> __result)
    {
        //Modified loot
        var lootProfile = __instance?.DeathTreasure?.Clone() as TreasureDeath;

        bool replaceProfile = false;

        if (killer.IsPlayer && killer.TryGetPetOwnerOrAttacker() is Player player && lootProfile is not null)
        {
            replaceProfile = true;
            lootProfile.MundaneItemChance += player.GetCachedFake(FakeInt.LootMundaneItemChance);
            lootProfile.MundaneItemMinAmount += player.GetCachedFake(FakeInt.LootMundaneItemMinAmount);
            lootProfile.MundaneItemMaxAmount += player.GetCachedFake(FakeInt.LootMundaneItemMaxAmount);
            lootProfile.ItemChance += player.GetCachedFake(FakeInt.LootItemChance);
            lootProfile.ItemMinAmount += player.GetCachedFake(FakeInt.LootItemMinAmount);
            lootProfile.ItemMaxAmount += player.GetCachedFake(FakeInt.LootItemMaxAmount);
            lootProfile.MagicItemChance += player.GetCachedFake(FakeInt.LootMagicItemChance);
            lootProfile.MagicItemMinAmount += player.GetCachedFake(FakeInt.LootMagicItemMinAmount);
            lootProfile.MagicItemMaxAmount += player.GetCachedFake(FakeInt.LootMagicItemMaxAmount);
            lootProfile.LootQualityMod += (float)player.GetCachedFake(FakeFloat.LootItemQualityMod);
            //Short circuit roll?
            var chance = player.GetCachedFake(FakeFloat.ItemLootTierUpgrade);
            if (chance > 0 && ThreadSafeRandom.Next(0f, 1.0f) < chance)
            {
                lootProfile.Tier = Math.Min(8, lootProfile.Tier + 1);
                player.SendMessage($"Upgraded treasure tier of {corpse.Name} to {lootProfile.Tier}");
            }
        }

        #region Original
        var droppedItems = new List<WorldObject>();

        // create death treasure from loot generation factory
        if (__instance.DeathTreasure != null)
        {
            //List<WorldObject> items = LootGenerationFactory.CreateRandomLootObjects(__instance.DeathTreasure);
            List<WorldObject> items = LootGenerationFactory.CreateRandomLootObjects(replaceProfile ? lootProfile : __instance.DeathTreasure);

            foreach (WorldObject wo in items)
            {
                if (corpse != null)
                    corpse.TryAddToInventory(wo);
                else
                    droppedItems.Add(wo);

                __instance.DoCantripLogging(killer, wo);
            }
        }

        // move wielded treasure over, which also should include Wielded objects not marked for destroy on death.
        // allow server operators to configure this behavior due to errors in createlist post 16py data
        var dropFlags = PropertyManager.GetBool("creatures_drop_createlist_wield").Item ? DestinationType.WieldTreasure : DestinationType.Treasure;

        var wieldedTreasure = __instance.Inventory.Values.Concat(__instance.EquippedObjects.Values).Where(i => (i.DestinationType & dropFlags) != 0);
        foreach (var item in wieldedTreasure.ToList())
        {
            if (item.Bonded == BondedStatus.Destroy)
                continue;

            if (__instance.TryDequipObjectWithBroadcasting(item.Guid, out var wo, out var wieldedLocation))
                __instance.EnqueueBroadcast(new GameMessagePublicUpdateInstanceID(item, PropertyInstanceId.Wielder, ObjectGuid.Invalid));

            if (corpse != null)
            {
                corpse.TryAddToInventory(item);
                __instance.EnqueueBroadcast(new GameMessagePublicUpdateInstanceID(item, PropertyInstanceId.Container, corpse.Guid), new GameMessagePickupEvent(item));
            }
            else
                droppedItems.Add(item);
        }

        // contain and non-wielded treasure create
        if (__instance.Biota.PropertiesCreateList != null)
        {
            var createList = __instance.Biota.PropertiesCreateList.Where(i => (i.DestinationType & DestinationType.Contain) != 0 ||
                            (i.DestinationType & DestinationType.Treasure) != 0 && (i.DestinationType & DestinationType.Wield) == 0).ToList();

            var selected = Creature.CreateListSelect(createList);

            foreach (var item in selected)
            {
                var wo = WorldObjectFactory.CreateNewWorldObject(item);

                if (wo != null)
                {
                    if (corpse != null)
                        corpse.TryAddToInventory(wo);
                    else
                        droppedItems.Add(wo);
                }
            }
        }
        #endregion

        __result = droppedItems;
        return false;
    }


    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(LootGenerationFactory), nameof(LootGenerationFactory.CreateRandomLootObjects), new Type[] { typeof(TreasureDeath) })]
    //public static bool PreCreateRandomLootObjects(TreasureDeath profile, ref List<WorldObject> __result)
    //{
    //   //No access to killer
    //    return true;
    //}

}
