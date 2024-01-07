using ACE.Database;
using ACE.Entity.Enum.Properties;
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

        foreach (var item in items)
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

    /// <summary>
    /// Flag all items in Container for Ironman
    /// </summary>
    public static void SetClaimedBy(this Container container, Player player)
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