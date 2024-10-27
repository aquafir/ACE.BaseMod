namespace Tower;

[CommandCategory(nameof(Feature.AutoLoot))]
[HarmonyPatchCategory(nameof(Feature.AutoLoot))]
public static class AutoLoot
{
    static LootSettings Settings => PatchClass.Settings.Loot;
    static Random random = new();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.CreateCorpse), new Type[] { typeof(DamageHistoryInfo), typeof(bool) })]
    public static bool PreCreateCorpse(DamageHistoryInfo killer, bool hadVitae, ref Creature __instance)
    {
        //If you can't find the killer handle the corpse in the regular fashion
        if (killer?.TryGetPetOwnerOrAttacker() is not Player player)
            return true;

        //Don't apply to players
        //if (__instance is Player p)
        //    return true;




        //Alternative loot system.  Drop some amount of a currency item?
        //return false;





        //Get a list of lootable items
        var loot = GetLoot(player, __instance);

        if (loot.Count == 0)
            return false;

        //Get a list of looters, just the player if not in a fellow with some restriction
        List<Player> looters = Settings.LooterRequirements switch
        {
            LooterRequirements.Landblock =>
                player.GetFellowshipTargets().Where(x => x.CurrentLandblock.Id == player.CurrentLandblock.Id).ToList(),
            LooterRequirements.Range =>
                player.GetFellowshipTargets().Where(x => x.Location.Distance2D(player.Location) < Fellowship.MaxDistance * 2).ToList(),
            _ => player.GetFellowshipTargets().ToList(),
        };


        if (player.GetProperty(LootMuted) != true)
            player.SendMessage($"Looting {loot.Count} items for {looters.Count} players", Settings.MessageType);

        //Roll a random starting player for round-robin
        var index = random.Next(looters.Count);

        //For each loot item
        foreach (var item in loot)
        {
            switch (Settings.LootStyle)
            {
                case LootStyle.Finder:
                    player.Loot(item);
                    break;
                case LootStyle.RoundRobin:
                    var looter = looters[index];
                    index = (index + 1) % looters.Count;
                    looter.Loot(item);
                    break;
                case LootStyle.OneForAll:
                    foreach (var l in looters)
                    {
                        //TODO: proper clone instead of weenie clone
                        var clonedItem = WorldObjectFactory.CreateNewWorldObject(item.WeenieClassId);
                        l.Loot(item);
                    }
                    item?.Destroy(); //Clean up source item?
                    break;
            }
        }

        return false;
    }

    /// <summary>
    /// Handles autoloot behavior for a player being given an item
    /// </summary>
    public static void Loot(this Player player, WorldObject item)
    {
        //if(item.TryGetBankedItem(out var bankItem))
        //{
        //    var amt = ThreadSafeRandom.Next(0f, 10f);
        //    player.IncBanked(bankItem.Prop, amt);
        //    player.SendMessage($"{amt:0.00} {item.Name} added for {player.GetBanked(bankItem.Prop):0.00} in bank.", PatchClass.Settings.MessageType);
        //    return;
        //}

        var success = player.TryCreateInInventoryWithNetworking(item);
        if (player.GetProperty(LootMuted) != true)
            player.SendMessage($"{(success ? "Looted" : "Failed to loot")} {item.Name}.", Settings.MessageType);

        //Could drop or something else on failure...
    }

    /// <summary>
    /// Return a list of loot items the way ACE would
    /// </summary>
    public static List<WorldObject> GetLoot(Player player, Creature creature)
    {
        var droppedItems = new List<WorldObject>();

        //Death
        if (creature.DeathTreasure != null)
        {
            foreach (var item in LootGenerationFactory.CreateRandomLootObjects(creature.DeathTreasure))
                //if (!player.TryCreateInInventoryWithNetworking(item))
                droppedItems.Add(item);
        }

        //Wielded
        var dropFlags = PropertyManager.GetBool("creatures_drop_createlist_wield").Item ? DestinationType.WieldTreasure : DestinationType.Treasure;
        var wieldedTreasure = creature.Inventory.Values.Concat(creature.EquippedObjects.Values).Where(i => (i.DestinationType & dropFlags) != 0);
        foreach (var item in wieldedTreasure)
        {
            if (item.Bonded == BondedStatus.Destroy)
                continue;

            if (creature.TryDequipObjectWithBroadcasting(item.Guid, out var wo, out var wieldedLocation))
                creature.EnqueueBroadcast(new GameMessagePublicUpdateInstanceID(item, PropertyInstanceId.Wielder, ObjectGuid.Invalid));

            //if (!player.TryCreateInInventoryWithNetworking(item))
            droppedItems.Add(item);
        }

        //Non-wielded Create
        if (creature.Biota.PropertiesCreateList != null)
        {
            var createList = creature.Biota.PropertiesCreateList.Where(i => (i.DestinationType & DestinationType.Contain) != 0 ||
                            (i.DestinationType & DestinationType.Treasure) != 0 && (i.DestinationType & DestinationType.Wield) == 0).ToList();

            var selected = Creature.CreateListSelect(createList);

            foreach (var create in selected)
            {
                var item = WorldObjectFactory.CreateNewWorldObject(create);
                if (item is null)
                    continue;

                //if (!player.TryCreateInInventoryWithNetworking(item))
                droppedItems.Add(item);
            }
        }

        return droppedItems;
    }

    //Create a fake property bool to track
    static PropertyBool LootMuted = (PropertyBool)39998;

    //Create a command to toggle the variable
    [CommandHandler("lootmute", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleT2(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player is null) return;

        //Get the current property, defaulting to false if absent
        var toggle = player.GetProperty(LootMuted) ?? false;

        //Toggle the prop
        toggle = !toggle;

        //Set it to the opposite and inform the player
        player.SetProperty(LootMuted, toggle);
        player.SendMessage($"Loot messages will {(toggle ? "not" : "")} be sent.");
    }




    [CommandHandler("clean", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void Clean(Session session, params string[] parameters)
    {
        // @delete - Deletes the selected object. Players may not be deleted this way.

        var player = session.Player;

        foreach (var item in player.Inventory.Values)
        {
            if (item is Container)
                continue;

            item.DeleteObject(player);
            session.Network.EnqueueSend(new GameMessageDeleteObject(item));
        }
    }
}
