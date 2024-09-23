namespace Raise;

public enum RaiseTarget
{
    // !!If RaiseTargets are added or re-ordered the TargetHelpers should be updated

    //Match attributes with ACE.Entity.Enum.Properties.PropertyAttribute to work with casting
    Str = PropertyAttribute.Strength,
    End = PropertyAttribute.Endurance,
    Quick = PropertyAttribute.Quickness,
    Coord = PropertyAttribute.Coordination,
    Focus = PropertyAttribute.Focus,
    Self = PropertyAttribute.Self,
    //Ratings
    World = PropertyInt.LumAugAllSkills,
    Offense = PropertyInt.LumAugDamageReductionRating,
    Defense = PropertyInt.LumAugDamageRating,
}


public static class TargetHelpers
{
    static RaiseSettings Settings => PatchClass.Settings.Raise;

    // Needed to support different handling for the type of target being raised
    public static bool IsAttribute(this RaiseTarget target) => target >= RaiseTarget.Str && target <= RaiseTarget.Self;
    public static bool IsRating(this RaiseTarget target) => target >= RaiseTarget.World && target <= RaiseTarget.Defense;

    // Map RaiseTarget to a PropertyInt used by the server DB to track levels
    /// <summary>
    /// Returns a fake PropertyInt based on the RaiseTarget to store the level on the server
    /// </summary>
    public static PropertyInt ToOffsetProperty(this RaiseTarget target) => (PropertyInt)(Settings.PropertyOffset + target);
    public static PropertyInt ToPropertyInt(this RaiseTarget target) => (PropertyInt)target;
    public static int FromProperty(this RaiseTarget target, Player player) => player.GetProperty(target.ToOffsetProperty()) ?? 0;

    //Todo: decide if this should be kept.  Only needed if something /raise'd needs special handling / isn't PropertyInt?
    /// <summary>
    /// Number of times a RaiseTarget has been raised
    /// </summary>
    public static int CurrentLevel(this RaiseTarget target, Player player) => target.FromProperty(player);
    /// <summary>
    /// Initial levels for different targets for naturally raised Luminance targets
    /// </summary>
    public static int StartingLevel(this RaiseTarget target)
    {
        switch (target)
        {
            //Attributes
            case RaiseTarget t when t.IsAttribute(): return 0;
            //Ratings return the normal max.
            //Comment out to allow leveling down to 0 which would let a player go through the normal process to net a little Lum?
            case RaiseTarget.World: return 10;  //Max World 
            case RaiseTarget.Defense: return 5;
            case RaiseTarget.Offense: return 5;
            default: return 0;
        }
    }
    /// <summary>
    /// Sets the number of times a RaiseTarget has been raised and adjusts the thing it corresponds to
    /// </summary>
    public static void SetLevel(this RaiseTarget target, Player player, int level)
    {
        //Handle special cases before defaulting to setting the property
        if (target.TryGetAttribute(player, out CreatureAttribute attribute))
        {
            //Find the change in current and desired level
            var levelChange = level - target.CurrentLevel(player);
            attribute.StartingValue += (uint)levelChange;   //Tested to work with negatives
        }
        else
        {
            //Default assumes the RaiseTarget corresponds to a PropertyInt
            player.SetProperty(target.ToPropertyInt(), level);
        }

        //Store the raise level
        player.SetProperty(target.ToOffsetProperty(), level);

        //Todo: decide about removing properties when level is 0
        //if (level == 0) player.RemoveProperty(target.ToOffsetProperty());

        UpdateTarget(player, target);
    }

    /// <summary>
    /// Update client with new property value
    /// </summary>
    private static void UpdateTarget(Player player, RaiseTarget target)
    {
        var session = player.Session;

        //Update experience and attribute
        if (target.TryGetAttribute(player, out CreatureAttribute attribute))
        {
            session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, attribute));
            session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableExperience, player.AvailableExperience ?? 0));
            return;
        }

        //Update PropInt
        session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, target.ToPropertyInt(), player.LumAugAllSkills));
    }

    /// <summary>
    /// Get the cost of raising a number of levels from an initial level of a RaiseTarget.  False on overflow
    /// </summary>
    public static bool TryGetCostToLevel(this RaiseTarget target, int startLevel, int numLevels, out long cost)
    {
        cost = uint.MaxValue;
        //This may be too restrictive but it guarantees you are /raising some amount from a valid starting point
        if (startLevel < target.StartingLevel() || numLevels < 1)
            return false;

        var avgLevel = (2 * startLevel + numLevels) / 2.0;
        long avgCost = (long)(Settings.RaiseMulti * avgLevel / (Settings.RaiseDecay - Settings.LevelDecay * avgLevel));
        try
        {
            checked
            {
                cost = target switch
                {
                    RaiseTarget t when t.IsAttribute() => checked(avgCost * numLevels),

                    //Custom flat costs
                    RaiseTarget.Offense => checked(numLevels * Settings.RatingMulti),
                    RaiseTarget.Defense => checked(numLevels * Settings.RatingMulti),
                    RaiseTarget.World => checked(numLevels * Settings.WorldMult),
                };
            }
            return true;
        }
        catch (OverflowException ex) { }  //Eats errors to return false?  Can't remember
        return false;
    }
    /// <summary>
    /// Get the CreatureAttribute corresponding to a RaiseTarget if that target is an attribute
    /// </summary>
    public static bool TryGetAttribute(this RaiseTarget target, Player player, out CreatureAttribute? attribute)
    {
        attribute = null;
        if (!target.IsAttribute())
            return false;

        attribute = player.Attributes[(PropertyAttribute)target];
        return true;
    }
}