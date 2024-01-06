using ACE.Database;
using ACE.Entity.Enum.Properties;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Network;
using CustomLoot.Enums;
using ACE.DatLoader.Entity;
using ACE.Server.Network.GameAction.Actions;
using ACE.Server.Factories;
using ACE.Server.Command.Handlers;
using ACE.Server.Entity.Mutations;
using ACE.Entity;
using ACE.Server.WorldObjects;

namespace Ironman;

[HarmonyPatch]
public static class FakeIronman
{
    public static bool TryAdvanceSkill(this Player player, Skill skill)
    {
        if (player.GetCreatureSkill(skill) is not CreatureSkill s)
            return false;

        bool success = s.AdvancementClass == SkillAdvancementClass.Trained ?
            player.SpecializeSkill(skill) : player.TrainSkill(skill);

        return success;
    }

    public static void SendUpdatedSkills(this Player player)
    {
        //Update the player
        foreach (var skill in player.Skills)
        {
            var sType = skill.Key;
            var sac = skill.Value.AdvancementClass;
            if (sac != SkillAdvancementClass.Trained && sac != SkillAdvancementClass.Specialized)
                continue;

            GameMessagePrivateUpdateSkill gameMessagePrivateUpdateSkill = new GameMessagePrivateUpdateSkill(player, player.GetCreatureSkill(sType));
            GameMessageSystemChat gameMessageSystemChat = new GameMessageSystemChat($"{sType.ToSentence()} {(sac == SkillAdvancementClass.Trained ? "trained" : "specialized")}.", ChatMessageType.Advancement);
            player.Session.Network.EnqueueSend(gameMessagePrivateUpdateSkill, gameMessageSystemChat);
        }
        GameMessagePrivateUpdatePropertyInt gameMessagePrivateUpdatePropertyInt = new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.AvailableSkillCredits, player.AvailableSkillCredits.GetValueOrDefault());
        player.Session.Network.EnqueueSend(gameMessagePrivateUpdatePropertyInt);
    }

    //Remove all skills then generate up to max credits a plan
    public static void RollIronmanSkills(this Player player)
    {
        //Untrain skills
        //Enlightenment.RemoveSkills(player);
        int num = Enum.GetNames(typeof(Skill)).Length;
        for (int i = 1; i < num; i++)
        {
            Skill skill = (Skill)i;
            player.ResetSkill(skill);
        }
        player.AvailableExperience = 0L;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableExperience, 0L));
        HeritageGroupCG heritageGroupCG = DatManager.PortalDat.CharGen.HeritageGroups[(uint)player.Heritage.Value];
        player.AvailableSkillCredits = (int)heritageGroupCG.SkillCredits;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.AvailableSkillCredits, player.AvailableSkillCredits.GetValueOrDefault()));


        //Local copy of the skill pool
        List<Skill> pool = new(PatchClass.Settings.SecondarySkillPool);
        var log = new StringBuilder();

        //Specialize a primary skill
        var primary = PatchClass.Settings.PrimarySkillPool.Random();
        player.TrainSkill(primary);
        player.SpecializeSkill(primary);
        pool.Remove(primary);
        int remainingCreds = PatchClass.Settings.CreditsToPlanFor + player.AvailableSkillCredits ?? 0;
        log.Append($"\nPrimary skill: {primary}, {remainingCreds} credits left");

        //Train MC if magic was rolled or spec a different skill if not
        HashSet<Skill> trained = new();
        var second = (primary == Skill.WarMagic || primary == Skill.VoidMagic || primary == Skill.LifeMagic) ?
            Skill.ManaConversion : pool.Random();

        player.TrainSkill(second);
        trained.Add(second);

        if (second != Skill.ManaConversion)
        {
            player.SpecializeSkill(second);
            pool.Remove(second);  //No spec'ing after initial
        }
        remainingCreds = PatchClass.Settings.CreditsToPlanFor + player.AvailableSkillCredits ?? 0;
        log.Append($"\nSecondary skill: {second}, {remainingCreds} credits left");

        //Train random until a failure
        //int numSpec = 1;
        var plan = new StringBuilder();

        while (true)
        {
            //Get a random skill
            var skill = pool.Random();

            //Skip attempts to specialize past the allowed amount
            bool spec = trained.Contains(skill);
            //if (spec && numSpec >= PatchClass.Settings.MaxSpecializedSkills)
            //    continue;
            if (spec)
                continue;

            //Get the skill credit cost
            if (!DatManager.PortalDat.SkillTable.SkillBaseHash.TryGetValue((uint)skill, out var skillBase))
            {
                ModManager.Log($"Error looking up {skill} skill data.", ModManager.LogLevel.Error);
                break;
            }
            var cost = spec ? skillBase.UpgradeCostFromTrainedToSpecialized : skillBase.TrainedCost;

            //Check if done
            if (remainingCreds < cost)
                break;

            //If credits are available learn the skill
            if (player.AvailableSkillCredits.Value >= remainingCreds)
            {
                if (spec)
                    player.SpecializeSkill(skill);
                else
                    player.TrainSkill(skill);
            }
            //Otherwise add the skill to the plan
            else
            {
                plan.Append($"{skill};");

                //And update the pool
                if (spec)
                    pool.Remove(skill);
                else
                    trained.Add(skill);
            }

            //Update the pool and credits
            remainingCreds -= cost;

            //Remove from the pool and increase the tally
            pool.Remove(skill);
            //if (spec)
            //{
            //    pool.Remove(skill);
            //    //numSpec++;
            //}
            //else
            //    trained.Add(skill);

            log.Append($"\n{(spec ? "Specialized" : "Trained")} {skill} for {cost}, {remainingCreds} credits left");
        }

        //Set the plan / output what's done
        player.SetProperty(FakeString.IronmanPlan, plan.ToString());
        player.SetProperty(FakeString.IronmanFullPlan, log.ToString());
        ModManager.Log(log.ToString());

        player.SendUpdatedSkills();

        //Add items
        player.GiveIronmanItems(primary);
    }

    public static void GiveIronmanItems(this Player player, Skill primary)
    {
        if (!PatchClass.Settings.SkillItems.TryGetValue(primary, out var items) || items is null)
            return;

        foreach(var item in items)
        {
            AdminCommands.HandleCI(player.Session, item.Split(" "));
        }
    }

    public static void ApplyIronman(this Player player)
    {
        if (player is null)
            return;

        if (player.GetProperty(FakeBool.Ironman) == true)
        {
            player.SendMessage($"You are already an Ironman.");
            return;
        }

        player.SetProperty(FakeInt.HardcoreLives, PatchClass.Settings.HardcoreStartingLives);
        player.SetProperty(FakeFloat.TimestampLastPlayerDeath, Time.GetUnixTime());
        player.SetProperty(FakeBool.Ironman, true);
        player.SetProperty(FakeBool.Hardcore, true);
        player.RadarColor = RadarColor.Sentinel;
        player.SendMessage($"You're now participating in Ironman.  Stay safe!  Unless you rolled trash" +
            $"\nYou have {PatchClass.Settings.HardcoreStartingLives} remaining and {PatchClass.Settings.HardcoreSecondsBetweenDeathAllowed} seconds between deaths.");
    }

    public static void RemoveIronman(this Player player)
    {
        if (player is null)
            return;

        if (player.GetProperty(FakeBool.Ironman) == true)
        {
            player.SendMessage($"You are not an Ironman.");
            return;
        }

        if (player is null)
            return;

        player.SetProperty(FakeBool.Ironman, false);
        player.SetProperty(FakeBool.Hardcore, false);
        player.RadarColor = RadarColor.Default;
        player.SendMessage("You're no longer participating in Ironman");
    }

    #region Flag on Corpse/Emote/Vendor
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GenerateTreasure), new Type[] { typeof(DamageHistoryInfo), typeof(Corpse) })]
    public static void PostGenerateTreasure(DamageHistoryInfo killer, Corpse corpse, Creature __instance, List<WorldObject> __result)
    {
        if (killer is null || killer.TryGetPetOwnerOrAttacker() is not Player player)
            return;

        if (player.GetProperty(FakeBool.Ironman) != true)
            return;

        //foreach (var item in __result)
        foreach (var item in corpse.Inventory.Values)
            item.SetProperty(FakeBool.Ironman, true);

        //player.SendMessage($"Claimed corpse");
    }

    //Add Ironman to emote given items
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.TryCreateForGive), new Type[] { typeof(WorldObject), typeof(WorldObject) })]
    public static void PreTryCreateForGive(WorldObject giver, WorldObject itemBeingGiven, ref Player __instance, ref bool __result)
    {
        if (__instance is null || itemBeingGiven is null)
            return;

        if (__instance.GetProperty(FakeBool.Ironman) == true)
            itemBeingGiven.SetProperty(FakeBool.Ironman, true);

        __instance.SendMessage($"{itemBeingGiven.Name} now Ironman");
    }

    //Add Ironman to vendor items?
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.FinalizeBuyTransaction), new Type[] { typeof(Vendor), typeof(List<WorldObject>), typeof(List<WorldObject>), typeof(uint) })]
    public static void PreFinalizeBuyTransaction(Vendor vendor, List<WorldObject> genericItems, List<WorldObject> uniqueItems, uint cost, ref Player __instance)
    {
        if (__instance is null || __instance.GetProperty(FakeBool.Ironman) != true)
            return;

        foreach (var item in genericItems)
        {
            item.SetProperty(FakeBool.Ironman, true);
            __instance.SendMessage($"{item.Name} now Ironman");
        }
        foreach (var item in uniqueItems)
        {
            item.SetProperty(FakeBool.Ironman, true);
            __instance.SendMessage($"{item.Name} now Ironman");
        }
    }

    #endregion

    #region Restrictions
    //Check on add to inventory
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.TryCreateInInventoryWithNetworking), new Type[] { typeof(WorldObject), typeof(Container) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    public static bool PreTryCreateInInventoryWithNetworking(WorldObject item, Container container, ref Player __instance, ref bool __result)
    {
        //Skip non-Ironman players
        if (__instance is null || __instance.GetProperty(FakeBool.Ironman) != true)
            return true;

        if (item.GetProperty(FakeBool.Ironman) != true)
        {
            __result = false;
            __instance.SendMessage($"{item.Name} unable to be added to inventory of non-Ironman");
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionPutItemInContainer_Verify), new Type[] { typeof(uint), typeof(uint), typeof(int), typeof(Container), typeof(WorldObject), typeof(Container), typeof(Container), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Out, ArgumentType.Out, ArgumentType.Out })]
    public static void PreHandleActionPutItemInContainer_Verify(uint itemGuid, uint containerGuid, int placement, Container itemRootOwner, WorldObject item, Container containerRootOwner, Container container, bool itemWasEquipped, ref Player __instance, ref bool __result)
    {
        //Many parameters will not be populated in a prefix.  Runs other checks first
        //If the container is Ironman and the item is not reject it
        if (containerRootOwner is Player player && player?.GetProperty(FakeBool.Ironman) == true && item?.GetProperty(FakeBool.Ironman) != true)
        {
            player.Session.Network.EnqueueSend(new GameEventWeenieError(player.Session, WeenieError.YoureTooBusy));
            player.Session.Network.EnqueueSend(new GameEventInventoryServerSaveFailed(player.Session, itemGuid));
            player.SendMessage($"Unable to loot non-Ironman {item?.Name}");
            __result = false;
        }
    }


    //Check enchantments
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CreateEnchantment), new Type[] { typeof(WorldObject), typeof(WorldObject), typeof(WorldObject), typeof(Spell), typeof(bool), typeof(bool), typeof(bool) })]
    public static bool PreCreateEnchantment(WorldObject target, WorldObject caster, WorldObject weapon, Spell spell, bool equip, bool fromProc, bool isWeaponSpell, ref WorldObject __instance)
    {
        if (target is Player player && player.GetProperty(FakeBool.Ironman) == true && caster.GetProperty(FakeBool.Ironman) != true)
        {
            player.SendMessage($"{caster.Name} failed to cast {spell.Name ?? ""} on you.  Blame Ironmode");

            if (caster is Player p)
                p.SendMessage($"Failed to cast {spell.Name ?? ""} on {player.Name}. Blame Ironmode");

            return false;
        }

        return true;
    }

    //Check allegiance
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.IsPledgable), new Type[] { typeof(Player) })]
    public static bool PreIsPledgable(Player target, ref Player __instance, ref bool __result)
    {
        if (target.GetProperty(FakeBool.Ironman) == true && __instance.GetProperty(FakeBool.Ironman) != true)
        {
            __instance.SendMessage($"You can't swear to {target.Name}.  Blame Ironmode");
            __instance.Session.Network.EnqueueSend(new GameEventWeenieError(__instance.Session, WeenieError.OlthoiCannotJoinAllegiance));
            __result = false;
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.FellowshipRecruit), new Type[] { typeof(Player) })]
    public static bool PreFellowshipRecruit(Player newPlayer, ref Player __instance)
    {
        if (newPlayer.GetProperty(FakeBool.Ironman) == true && __instance.GetProperty(FakeBool.Ironman) != true)
        {
            __instance.SendMessage($"You can't recruit {newPlayer.Name}.  Blame Ironmode");
            __instance.Session.Network.EnqueueSend(new GameEventWeenieError(__instance.Session, WeenieError.FellowshipIllegalLevel));
            return false;
        }

        return true;
    }


    //Check on wield
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), nameof(Player.CheckWieldRequirements), new Type[] { typeof(WorldObject) })]
    //public static bool PreCheckWieldRequirementsCustom(WorldObject item, ref Player __instance, ref WeenieError __result)
    //{
    //    //Add check only to Ironman players
    //    if (__instance is null || __instance.GetProperty(FakeBool.Ironman) != true)
    //        return true;

    //    if (item.GetProperty(FakeBool.Ironman) != true)
    //    {
    //        __instance.SendMessage($"Unable to wield non-Ironman items!");

    //        __result = WeenieError.BeWieldedFailure;
    //        return false;
    //    }

    //    //Do regular checks
    //    return true;
    //} 
    #endregion

    #region Chests
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Chest), nameof(Chest.Unlock), new Type[] { typeof(uint), typeof(uint), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref })]
    public static void PostUnlock(uint unlockerGuid, uint playerLockpickSkillLvl, int difficulty, ref Chest __instance, ref UnlockResults __result)
=> HandleClaimChest(unlockerGuid, __instance, __result);
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Chest), nameof(Chest.Unlock), new Type[] { typeof(uint), typeof(Key), typeof(string) })]
    public static void PostUnlock(uint unlockerGuid, Key key, string keyCode, ref Chest __instance, ref UnlockResults __result)
        => HandleClaimChest(unlockerGuid, __instance, __result);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Chest), nameof(Chest.Reset), new Type[] { typeof(double?) })]
    public static void PostReset(double? resetTimestamp, ref Chest __instance) => __instance.RemoveProperty(FakeBool.Ironman);

    //Claim a container by unlocking it
    public static void HandleClaimChest(uint unlockerGuid, Chest container, UnlockResults result)
    {
        //Only care about successful unlocks
        if (result != UnlockResults.UnlockSuccess) return;

        //Check for Ironman players
        if (PlayerManager.GetOnlinePlayer(unlockerGuid) is not Player player || player.GetProperty(FakeBool.Ironman) != true)
            return;

        ClaimContainer(container, player);
    }
    #endregion

    public static void ClaimContainer(Container container, Player player)
    {
        foreach (var item in container.Inventory.Values)
            item.SetProperty(FakeBool.Ironman, true);

        //Debug
        var sb = new StringBuilder($"\n{player.Name} is claiming {container.Name}");
        foreach (var item in container.Inventory.Values)
            sb.Append($"\n  {item.Name}");

        player.SendMessage(sb.ToString());
    }

    #region Commands
    //[CommandHandler("iron", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    //public static void HandleIronman(Session session, params string[] parameters)
    //{
    //    var player = session.Player;

    //    if (player is null)
    //        return;

    //    if (player.GetProperty(FakeBool.Ironman) == true)
    //    {
    //        player.SendMessage($"You are already an Ironman.");
    //        return;
    //    }

    //    player.SetProperty(FakeInt.HardcoreLives, S.Settings.HardcoreStartingLives);
    //    player.SetProperty(FakeFloat.TimestampLastPlayerDeath, Time.GetUnixTime());
    //    player.SetProperty(FakeBool.Ironman, true);
    //    player.SetProperty(FakeBool.Hardcore, true);
    //    player.RadarColor = RadarColor.Sentinel;
    //    player.SendMessage($"You're now participating in Ironman.  Stay safe!  Unless you rolled trash" +
    //        $"\nYou have {S.Settings.HardcoreStartingLives} remaining and {S.Settings.HardcoreSecondsBetweenDeathAllowed} seconds between deaths.");
    //}

    //[CommandHandler("uniron", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    //public static void HandleUnIronman(Session session, params string[] parameters)
    //{
    //    var player = session.Player;

    //    if (player is null)
    //        return;

    //    player.SetProperty(FakeBool.Ironman, false);
    //    player.SetProperty(FakeBool.Hardcore, false);
    //    player.RadarColor = RadarColor.Default;
    //    player.SendMessage("You're no longer participating in Ironman");
    //}

    #endregion
}