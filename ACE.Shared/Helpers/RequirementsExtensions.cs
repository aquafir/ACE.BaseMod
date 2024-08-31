namespace ACE.Shared.Helpers;

/// <summary>
/// Helpers or rewrites of logic of various requirements
/// </summary>
public static class RequirementsExtensions
{
    //
    //public static 
    /// <summary>
    /// Verifies the use requirements for activating an item
    /// </summary>
    public static ActivationResult CheckUseRequirements(this WorldObject activator)
    {
        ActivationResult result = new(true);
        if (activator == null)
            return result;

        //Try to find the player doing the activation?
        if (activator is not Player player && !activator.TryGetOwner(out player))
            return new ActivationResult(true);

        result = CheckArcaneLore(activator, result, player);
        if (!result.Success)
            return result;

        result = CheckItemSkillLimit(activator, result, player);
        if (!result.Success)
            return result;

        result = CheckSkill(activator, result, player);
        if (!result.Success)
            return result;

        result = CheckSpecialized(activator, result, player);
        if (!result.Success)
            return result;

        result = CheckLevel(activator, result, player);
        if (!result.Success)
            return result;

        result = CheckAttributes(activator, result, player);
        if (!result.Success)
            return result;

        result = CheckVitals(activator, result, player);
        if (!result.Success)
            return result;

        result = CheckCooldown(activator, result, player);
        if (!result.Success)
            return result;

        result = CheckOlthoi(activator, result, player);
        if (!result.Success)
            return result;

        return new ActivationResult(true);
    }

    private static ActivationResult CheckOlthoi(WorldObject activator, ActivationResult result, Player player)
    {
        if (player.IsOlthoiPlayer)
        {
            //player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, "Olthoi can't interact with that!"));
            //player.SendWeenieError(WeenieError.OlthoiCannotInteractWithThat);
            //result = new ActivationResult(false);

            if (activator is Creature)
            {
                if (activator.CreatureType == ACE.Entity.Enum.CreatureType.Olthoi)
                    result = new ActivationResult(true);
                else
                {
                    if (activator is Vendor)
                        player.SendWeenieError(WeenieError.OlthoiVendorLooksInHorror);
                    else if (activator.NpcLooksLikeObject ?? false)
                        player.SendWeenieError(WeenieError.OlthoiCannotInteractWithThat);
                    else
                        player.SendWeenieErrorWithString(WeenieErrorWithString._CowersFromYou, activator.Name);

                    result = new ActivationResult(false);
                }
            }
            else if (activator is Lifestone)
            {
                player.SendWeenieError(WeenieError.OlthoiCannotUseLifestones);
                result = new ActivationResult(false);
            }
            else if (activator is Container && !(activator is Corpse))
            {
                player.SendWeenieError(WeenieError.OlthoiCannotInteractWithThat);
                result = new ActivationResult(false);
            }
            else if (activator is AttributeTransferDevice || activator is AugmentationDevice || activator is Bindstone || activator is Book
                || activator is Game || activator is Gem || activator is GenericObject || activator is Key || activator is SkillAlterationDevice)
            {
                player.SendWeenieError(WeenieError.OlthoiCannotInteractWithThat);
                result = new ActivationResult(false);
            }
        }

        return result;
    }

    private static ActivationResult CheckCooldown(WorldObject activator, ActivationResult result, Player player)
    {
        // Check for a cooldown
        if (!player.EnchantmentManager.CheckCooldown(activator.CooldownId))
        {
            player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, "You have used this item too recently"));
            result = new ActivationResult(false);
        }

        return result;
    }

    private static ActivationResult CheckVitals(WorldObject activator, ActivationResult result, Player player)
    {
        // verify vitals
        if (activator.ItemAttribute2ndLimit != null)
        {
            var playerVital = player.Vitals[activator.ItemAttribute2ndLimit.Value];

            if (playerVital.MaxValue < activator.ItemAttribute2ndLevelLimit)
                result = new ActivationResult(new GameEventWeenieErrorWithString(player.Session, WeenieErrorWithString.Your_IsTooLowToUseItemMagic, playerVital.Vital.ToSentence()));
        }

        return result;
    }

    private static ActivationResult CheckAttributes(WorldObject activator, ActivationResult result, Player player)
    {
        // verify attributes

        if (activator.ItemAttributeLimit != null)
        {
            var playerAttr = player.Attributes[activator.ItemAttributeLimit.Value];

            if (playerAttr.Current < activator.ItemAttributeLevelLimit)
                result = new ActivationResult(new GameEventWeenieErrorWithString(player.Session, WeenieErrorWithString.Your_IsTooLowToUseItemMagic, playerAttr.Attribute.ToString()));
        }

        return result;
    }

    private static ActivationResult CheckLevel(WorldObject activator, ActivationResult result, Player player)
    {
        // verify player level
        if (activator.UseRequiresLevel != null)
        {
            var playerLevel = player.Level ?? 1;
            if (playerLevel < activator.UseRequiresLevel.Value)
                result = new ActivationResult(new GameEventCommunicationTransientString(player.Session, "You are not high enough level to use that!"));
        }

        return result;
    }

    private static ActivationResult CheckSpecialized(WorldObject activator, ActivationResult result, Player player)
    {
        // verify skill specialized
        // is this always in conjunction with UseRequiresSkill?
        // again, only seems to be for summoning so far...
        if (activator.UseRequiresSkillSpec != null)
        {
            var skill = activator.ConvertToMoASkill((Skill)activator.UseRequiresSkillSpec.Value);
            var playerSkill = player.GetCreatureSkill(skill);

            if (playerSkill.AdvancementClass < SkillAdvancementClass.Specialized)
                result = new ActivationResult(new GameEventWeenieErrorWithString(player.Session, WeenieErrorWithString.YouMustSpecialize_ToUseItemMagic, playerSkill.Skill.ToSentence()));

            // verify skill level
            if (activator.UseRequiresSkillLevel != null)
            {
                if (playerSkill.Current < activator.UseRequiresSkillLevel.Value)
                    result = new ActivationResult(new GameEventWeenieErrorWithString(player.Session, WeenieErrorWithString.Your_IsTooLowToUseItemMagic, playerSkill.Skill.ToSentence()));
            }
        }

        return result;
    }

    private static ActivationResult CheckSkill(WorldObject activator, ActivationResult result, Player player)
    {
        if (activator.UseRequiresSkill != null)
        {
            var skill = activator.ConvertToMoASkill((Skill)activator.UseRequiresSkill.Value);
            var playerSkill = player.GetCreatureSkill(skill);

            if (playerSkill.AdvancementClass < SkillAdvancementClass.Trained)
            {
                player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, $"You must have {playerSkill.Skill.ToSentence()} trained to use that item's magic"));
                result = new ActivationResult(false);
            }

            // verify skill level
            if (activator.UseRequiresSkillLevel != null)
            {
                if (playerSkill.Current < activator.UseRequiresSkillLevel.Value)
                    result = new ActivationResult(new GameEventWeenieErrorWithString(player.Session, WeenieErrorWithString.Your_IsTooLowToUseItemMagic, playerSkill.Skill.ToSentence()));
            }
        }

        return result;
    }

    private static ActivationResult CheckItemSkillLimit(WorldObject activator, ActivationResult result, Player player)
    {
        // verify skill - does this have to be trained, or only in conjunction with UseRequiresSkillLevel?
        // only seems to be used for summoning so far...
        if (activator.ItemSkillLimit != null && activator.ItemSkillLevelLimit != null)
        {
            var skill = activator.ConvertToMoASkill((Skill)activator.ItemSkillLimit.Value);
            var playerSkill = player.GetCreatureSkill(skill);

            if (playerSkill.Current < activator.ItemSkillLevelLimit.Value)
                result = new ActivationResult(new GameEventWeenieErrorWithString(player.Session, WeenieErrorWithString.Your_IsTooLowToUseItemMagic, playerSkill.Skill.ToSentence()));
        }

        return result;
    }

    private static ActivationResult CheckArcaneLore(WorldObject activator, ActivationResult result, Player player)
    {
        // verify arcane lore requirement
        if (activator.ItemDifficulty != null)
        {
            var arcaneLore = player.GetCreatureSkill(Skill.ArcaneLore);
            if (arcaneLore.Current < activator.ItemDifficulty.Value)
                result = new ActivationResult(new GameEventWeenieErrorWithString(player.Session, WeenieErrorWithString.Your_IsTooLowToUseItemMagic, arcaneLore.Skill.ToSentence()));
        }

        return result;
    }
}