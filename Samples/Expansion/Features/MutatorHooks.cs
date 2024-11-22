namespace Expansion.Features;
[CommandCategory(nameof(Feature.MutatorHooks))]
[HarmonyPatchCategory(nameof(Feature.MutatorHooks))]
internal class MutatorHooks
{
    public static readonly Dictionary<MutationEvent, List<Mutator>> mutators = new();
    static Settings Settings => PatchClass.Settings;

    public static void SetupMutators()
    {
        //Make the Mutator directory if needed and load from there
        //Directory.CreateDirectory(Settings.MutatorPath);
        //MutatorHelpers.LoadMutators(Settings.MutatorPath);

        //enabledPatches.Clear();
        mutators.Clear();

        //Get atomic MutationEvents
        var basicMutators = FlagExtensions.GetIndividualFlags<MutationEvent>();

        foreach (var mutator in basicMutators)
            mutators[mutator] = new();

        foreach (var mutatorOptions in S.Settings.Mutators)
        {
            if (!mutatorOptions.Enabled)
                continue;

            try
            {
                var mutator = mutatorOptions.GetMutator();
                mutator.Start();

                //enabledPatches.Add(mutator);
                foreach (var basicFlag in mutator.Event.GetIndividualFlags())
                    mutators[basicFlag].Add(mutator);

                if (PatchClass.Settings.Verbose)
                    ModManager.Log($"Enabled mutator: {mutatorOptions.Mutation}");
            }
            catch (Exception ex)
            {
                if (PatchClass.Settings.Verbose)
                    ModManager.Log($"Failed to patch {mutatorOptions.Mutation}: {ex.Message}", ModManager.LogLevel.Error);
            }
        }
    }

    public static void ShutdownMutators()
    {
        //if (Mod.State == ModState.Running)

        //Shutdown/unpatch everything on settings change to support repatching by category
        foreach (var eventType in mutators.Values)
        {
            if (eventType is null)
                continue;

            //Todo: Prevent duplicate shutdowns..?
            HashSet<Mutator> encountered = new();

            foreach (var mutator in eventType)
            {
                if (encountered.Contains(mutator))
                    continue;

                //Shut down the mutator / remember it
                encountered.Add(mutator);
                mutator.Shutdown();
            }
        }
    }

    /// <summary>
    /// After any loot is generated
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LootGenerationFactory), nameof(LootGenerationFactory.CreateAndMutateWcid), new Type[] { typeof(TreasureDeath), typeof(TreasureRoll), typeof(bool) })]
    public static void PostCreateAndMutateWcid(TreasureDeath treasureDeath, TreasureRoll treasureRoll, bool isMagical, ref WorldObject __result)
    {
        if (treasureDeath is null) return;

        //Keeps track of what mutations have been applied
        HashSet<Mutation> mutations = new();

        foreach (var mutator in mutators[MutationEvent.Loot])
        {
            //Check for elligible item type along with standard check
            if (!mutator.CheckMutatesLoot(mutations, treasureDeath, treasureRoll, __result))
                continue;

            //If an item was mutated add the type
            if (mutator.TryMutateLoot(mutations, treasureDeath, treasureRoll, __result))
                mutations.Add(mutator.MutationType);
        }

        if (PatchClass.Settings.Verbose && mutations.Count > 0)
            ModManager.Log($"{__result.Name} was mutated with: {string.Join(", ", mutations)}");
    }

    /// <summary>
    /// After creature dies
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GenerateTreasure), new Type[] { typeof(DamageHistoryInfo), typeof(Corpse) })]
    public static void PostGenerateTreasure(DamageHistoryInfo killer, Corpse corpse, Creature __instance, ref List<WorldObject> __result)
    {
        var validMutators = mutators[MutationEvent.Corpse].Where(x => x.CanMutateCorpse(killer, corpse, __instance));
        if (validMutators.Count() == 0) return;

        //!!DROPPED ITEMS in __result, the rest are moved to corpse?!!
        //foreach (var item in __result)
        foreach (var item in corpse.Inventory.Values)
        {
            //Keeps track of what mutations have been applied
            HashSet<Mutation> mutations = new();

            foreach (var mutator in validMutators)
            {
                //Standard checks
                if (!mutator.CheckMutates(item))
                    continue;

                //If an item was mutated add the type
                if (mutator.TryMutateCorpse(mutations, __instance, killer, corpse, item))
                    mutations.Add(mutator.MutationType);

                if (PatchClass.Settings.Verbose && mutations.Count > 0)
                    ModManager.Log($"{item.Name} was mutated with: {string.Join(", ", mutations)}");
            }
        }
    }

    /// <summary>
    /// Vendor has validated the transactions and sent a list of items for processing.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.FinalizeBuyTransaction), new Type[] { typeof(Vendor), typeof(List<WorldObject>), typeof(List<WorldObject>), typeof(uint) })]
    public static void PreFinalizeBuyTransaction(Vendor vendor, List<WorldObject> genericItems, List<WorldObject> uniqueItems, uint cost, ref Player __instance)
    {
        var validMutators = mutators[MutationEvent.VendorBuy].Where(x => x.CanMutateVendor(vendor));
        if (validMutators.Count() == 0) return;

        foreach (var item in genericItems)
        {
            //Keeps track of what mutations have been applied
            HashSet<Mutation> mutations = new();

            foreach (var mutator in validMutators)
            {
                //Standard checks
                if (!mutator.CheckMutates(item))
                    continue;

                //If an item was mutated add the type
                if (mutator.TryMutateVendorBuy(mutations, item, vendor, __instance))
                    mutations.Add(mutator.MutationType);

                if (PatchClass.Settings.Verbose && mutations.Count > 0)
                    ModManager.Log($"{item.Name} was mutated with: {string.Join(", ", mutations)}");
            }
        }
    }

    /// <summary>
    /// Container/treasure generator?
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GeneratorProfile), nameof(GeneratorProfile.TreasureGenerator))]
    public static void PostTreasureGenerator(GeneratorProfile __instance, ref List<WorldObject> __result)
    {
        var validMutators = mutators[MutationEvent.Generator].Where(x => x.CanMutateGenerator(__instance));
        if (validMutators.Count() == 0) return;

        //Loop through each item
        foreach (var item in __result)
        {
            //Keeps track of what mutations have been applied
            HashSet<Mutation> mutations = new();

            foreach (var mutator in validMutators)
            {
                //Standard checks
                if (!mutator.CheckMutates(item))
                    continue;

                //If an item was mutated add the type
                if (mutator.TryMutateGenerator(mutations, __instance, item))
                    mutations.Add(mutator.MutationType);

                if (PatchClass.Settings.Verbose && mutations.Count > 0)
                    ModManager.Log($"{item.Name} was mutated with: {string.Join(", ", mutations)}");
            }
        }
    }

    #region WeenieFactory Methods
#if REALM
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Weenie), typeof(ObjectGuid), typeof(AppliedRuleset) })]
    public static void PostCreateWorldObject(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset, ref WorldObject __result)
        => MutateFactory(__result);
#else
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Weenie), typeof(ObjectGuid) })]
    public static void PostCreateWorldObject(Weenie weenie, ObjectGuid guid, ref WorldObject __result)
        => MutateFactory(__result);
#endif
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(Biota) })]
    public static void PreCreateWorldObject(ACE.Entity.Models.Biota biota, ref WorldObject __result)
        => MutateFactory(__result);
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldObjectFactory), nameof(WorldObjectFactory.CreateWorldObject), new Type[] { typeof(ACE.Database.Models.Shard.Biota) })]
    public static void PostCreateWorldObject(ACE.Database.Models.Shard.Biota databaseBiota, ref WorldObject __result)
        => MutateFactory(__result);

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(WorldObject), nameof(WorldObject.EnterWorld))]
    //public static void PostEnterWorld(ref WorldObject __instance, ref bool __result)
    //    => MutateFactory(__instance);

    private static void MutateFactory(WorldObject __result)
    {
        if (__result is null) return;

        //Keeps track of what mutations have been applied
        HashSet<Mutation> mutations = new();

        foreach (var mutator in mutators[MutationEvent.Factory])
        {
            //Check for elligible item type along with standard check
            if (!mutator.CheckMutates(__result))
                continue;

            //If an item was mutated add the type
            if (mutator.TryMutateFactory(mutations, __result))
                mutations.Add(mutator.MutationType);
        }

        if (PatchClass.Settings.Verbose && mutations.Count > 0)
            ModManager.Log($"{__result.Name} was mutated with: {string.Join(", ", mutations)}");

    }
    #endregion

    //Todo: postfix?
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.EnterWorld))]
    public static bool PreEnterWorld(ref WorldObject __instance, ref bool __result)
    {
        if (__instance is null) return true;

        //Keeps track of what mutations have been applied
        HashSet<Mutation> mutations = new();

        foreach (var mutator in mutators[MutationEvent.EnterWorld])
        {
            //Check for elligible item type along with standard check
            if (!mutator.CheckMutates(__instance))
                continue;

            //If an item was mutated add the type
            if (mutator.TryMutateEnterWorld(mutations, __instance))
                mutations.Add(mutator.MutationType);
        }

        if (PatchClass.Settings.Verbose && mutations.Count > 0)
            ModManager.Log($"{__instance.Name} was mutated with: {string.Join(", ", mutations)}");

        return true;
    }

    /// <summary>
    /// On successful addition to Player inventory
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.TryCreateInInventoryWithNetworking), new Type[] { typeof(WorldObject), typeof(Container) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    public static void PostTryCreateInInventoryWithNetworking(WorldObject item, Container container, ref Player __instance, ref bool __result)
    {
        if (!__result)
            return;

        //if (__instance is null || item is null) return;

        //Keeps track of what mutations have been applied
        HashSet<Mutation> mutations = new();
        foreach (var mutator in mutators[MutationEvent.Inventory])
        {
            //Check for elligible item type along with standard check
            if (!mutator.CheckMutates(item))
                continue;

            //If an item was mutated add the type
            //Todo: create a separate handler for entering inventory?
            if (mutator.TryMutateEnterInventory(mutations, item, __instance))
                mutations.Add(mutator.MutationType);
        }

        if (PatchClass.Settings.Verbose && mutations.Count > 0)
            ModManager.Log($"{item.Name} was mutated with: {string.Join(", ", mutations)}");
    }

    /// <summary>
    /// On successful Give or CreateTreasure emote
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.TryCreateForGive), new Type[] { typeof(WorldObject), typeof(WorldObject) })]
    public static void PreTryCreateForGive(WorldObject giver, WorldObject itemBeingGiven, ref Player __instance, ref bool __result)
    {
        if (__instance is null || itemBeingGiven is null) return;

        //Keeps track of what mutations have been applied
        HashSet<Mutation> mutations = new();
        foreach (var mutator in mutators[MutationEvent.EmoteGive])
        {
            //Check for elligible item type along with standard check
            if (!mutator.CheckMutates(itemBeingGiven))
                continue;

            //If an item was mutated add the type
            //Todo: create a separate handler for entering inventory?
            if (mutator.TryMutateEmoteGive(mutations, itemBeingGiven, giver, __instance))
                mutations.Add(mutator.MutationType);
        }

        if (PatchClass.Settings.Verbose && mutations.Count > 0)
            ModManager.Log($"{itemBeingGiven.Name} was mutated with: {string.Join(", ", mutations)}");
    }

    /// <summary>
    /// Container-level inventory?
    /// </summary>
    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(Container), nameof(Container.TryAddToInventory), new Type[] { typeof(WorldObject), typeof(Container), typeof(int), typeof(bool), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
    //public static void PreTryAddToInventory(WorldObject worldObject, Container container, int placementPosition, bool limitToMainPackOnly, bool burdenCheck, ref Container __instance, ref bool __result)
    //{
    //    //Skip failures
    //    if (!__result)
    //        return;
    //    //Return false to override
    //    //return false;

    //    //Return true to execute original
    //}
}
