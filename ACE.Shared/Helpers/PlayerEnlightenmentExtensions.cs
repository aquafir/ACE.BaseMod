namespace ACE.Shared.Helpers;

public static class PlayerEnlightenmentExtensions
{
    public static void ResetLikeEnlightenment(this Player player)
    {
        player.ResetLevel();
        player.ResetAttributes();
        player.ResetLuminance();
        player.ResetSkills();
    }

    /// <summary>
    /// Reset Level based on the Enlightenment code
    /// </summary>
    public static void ResetLevel(this Player player)
    {
        player.TotalExperience = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.TotalExperience, player.TotalExperience ?? 0));

        player.Level = 1;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.Level, player.Level ?? 0));
    }

    /// <summary>
    /// Reset Attributes based on the Enlightenment code
    /// </summary>
    public static void ResetAttributes(this Player player)
    {
        var propertyCount = Enum.GetNames(typeof(PropertyAttribute)).Length;
        for (var i = 1; i < propertyCount; i++)
        {
            var attribute = (PropertyAttribute)i;

            player.Attributes[attribute].Ranks = 0;
            player.Attributes[attribute].ExperienceSpent = 0;
            player.Session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, player.Attributes[attribute]));
        }

        propertyCount = Enum.GetNames(typeof(PropertyAttribute2nd)).Length;
        for (var i = 1; i < propertyCount; i += 2)
        {
            var attribute = (PropertyAttribute2nd)i;

            player.Vitals[attribute].Ranks = 0;
            player.Vitals[attribute].ExperienceSpent = 0;
            player.Session.Network.EnqueueSend(new GameMessagePrivateUpdateVital(player, player.Vitals[attribute]));
        }

        //player.SendMessage("Your attribute training fades.", ChatMessageType.Broadcast);
    }

    /// <summary>
    /// Reset Skills based on the Enlightenment code
    /// </summary>
    /// <param name="player"></param>
    public static void ResetSkills(this Player player)
    {
        var propertyCount = Enum.GetNames(typeof(Skill)).Length;
        for (var i = 1; i < propertyCount; i++)
        {
            var skill = (Skill)i;
            player.ResetSkillQuiet(skill, false);
        }

        player.AvailableExperience = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableExperience, 0));

        var heritageGroup = DatManager.PortalDat.CharGen.HeritageGroups[(uint)player.Heritage];
        var availableSkillCredits = 0;

        availableSkillCredits += (int)heritageGroup.SkillCredits; // base skill credits allowed

        availableSkillCredits += player.QuestManager.GetCurrentSolves("ArantahKill1");       // additional quest skill credit
        availableSkillCredits += player.QuestManager.GetCurrentSolves("OswaldManualCompleted");  // additional quest skill credit
        availableSkillCredits += player.QuestManager.GetCurrentSolves("LumAugSkillQuest");   // additional quest skill credits

        player.AvailableSkillCredits = availableSkillCredits;

        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.AvailableSkillCredits, player.AvailableSkillCredits ?? 0));
    }

    /// <summary>
    /// Resets the skill, refunds all experience and skill credits, if allowed.
    /// </summary>
    public static bool ResetSkillQuiet(this Player player, Skill skill, bool refund = true)
    {
        var creatureSkill = player.GetCreatureSkill(skill, false);

        if (creatureSkill == null || creatureSkill.AdvancementClass < SkillAdvancementClass.Trained)
            return false;

        // gather skill credits to refund
        DatManager.PortalDat.SkillTable.SkillBaseHash.TryGetValue((uint)creatureSkill.Skill, out var skillBase);

        if (skillBase == null)
            return false;

        // salvage / tinkering skills specialized via augmentations
        // Salvaging cannot be untrained or unspecialized => skillIsSpecializedViaAugmentation && !untrainable
        player.IsSkillSpecializedViaAugmentation(creatureSkill.Skill, out var skillIsSpecializedViaAugmentation);

        var typeOfSkill = creatureSkill.AdvancementClass.ToString().ToLower() + " ";
        var untrainable = Player.IsSkillUntrainable(skill);
        var creditRefund = (creatureSkill.AdvancementClass == SkillAdvancementClass.Specialized && !(skillIsSpecializedViaAugmentation && !untrainable)) || untrainable;

        if (creatureSkill.AdvancementClass == SkillAdvancementClass.Specialized && !(skillIsSpecializedViaAugmentation && !untrainable))
        {
            creatureSkill.AdvancementClass = SkillAdvancementClass.Trained;
            creatureSkill.InitLevel = 0;
            if (!skillIsSpecializedViaAugmentation) // Tinkering skills can be unspecialized, but do not refund upgrade cost.
                player.AvailableSkillCredits += skillBase.UpgradeCostFromTrainedToSpecialized;
        }

        // temple untraining 'always trained' skills:
        // cannot be untrained, but skill XP can be recovered
        if (untrainable)
        {
            creatureSkill.AdvancementClass = SkillAdvancementClass.Untrained;
            creatureSkill.InitLevel = 0;
            player.AvailableSkillCredits += skillBase.TrainedCost;
        }

        if (refund)
            player.RefundXP(creatureSkill.ExperienceSpent);

        creatureSkill.ExperienceSpent = 0;
        creatureSkill.Ranks = 0;

        var updateSkill = new GameMessagePrivateUpdateSkill(player, creatureSkill);
        var availableSkillCredits = new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.AvailableSkillCredits, player.AvailableSkillCredits ?? 0);

        //var msg = $"Your {(untrainable ? $"{typeOfSkill}" : "")}{skill.ToSentence()} skill has been {(untrainable ? "removed" : "reset")}. ";
        //msg += $"All the experience {(creditRefund ? "and skill credits " : "")}that you spent on this skill have been refunded to you.";

        if (refund)
            //player.Session.Network.EnqueueSend(updateSkill, availableSkillCredits, new GameMessageSystemChat(msg, ChatMessageType.Broadcast));
            player.Session.Network.EnqueueSend(updateSkill, availableSkillCredits);
        else
            //player.Session.Network.EnqueueSend(updateSkill, new GameMessageSystemChat(msg, ChatMessageType.Broadcast));
            player.Session.Network.EnqueueSend(updateSkill);

        return true;
    }


    /// <summary>
    /// Reset Luminance based on the Enlightenment code
    /// </summary>
    /// <param name="player"></param>
    public static void ResetLuminance(this Player player)
    {
        player.QuestManager.Erase("OracleLuminanceRewardsAccess_1110");
        player.QuestManager.Erase("LoyalToShadeOfLadyAdja");
        player.QuestManager.Erase("LoyalToKahiri");
        player.QuestManager.Erase("LoyalToLiamOfGelid");
        player.QuestManager.Erase("LoyalToLordTyragar");

        player.LumAugDamageRating = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageRating, 0));
        player.LumAugDamageReductionRating = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageReductionRating, 0));
        player.LumAugCritDamageRating = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugCritDamageRating, 0));
        player.LumAugCritReductionRating = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugCritReductionRating, 0));
        //player.LumAugSurgeEffectRating = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugSurgeEffectRating, 0));
        player.LumAugSurgeChanceRating = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugSurgeChanceRating, 0));
        player.LumAugItemManaUsage = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugItemManaUsage, 0));
        player.LumAugItemManaGain = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugItemManaGain, 0));
        player.LumAugVitality = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugVitality, 0));
        player.LumAugHealingRating = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugHealingRating, 0));
        player.LumAugSkilledCraft = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugSkilledCraft, 0));
        player.LumAugSkilledSpec = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugSkilledSpec, 0));
        player.LumAugAllSkills = 0;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugAllSkills, 0));

        player.AvailableLuminance = null;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableLuminance, 0));
        player.MaximumLuminance = null;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.MaximumLuminance, 0));

        //player.SendMessage("Your Luminance and Luminance Auras fade from your spirit.", ChatMessageType.Broadcast);
    }
}
