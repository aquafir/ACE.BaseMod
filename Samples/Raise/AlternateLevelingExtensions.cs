using Raise;

public static class AlternateLevelingExtensions
{
    static AlternateLevelingSettings Settings => PatchClass.Settings.AlternateLeveling;

    const int ATTR_OFFSET = 55;
    const int VITAL_OFFSET = 62;
    #region Properties
    //Levels
    public static PropertyInt AltLevelProp(this Skill skill) => (PropertyInt)(Settings.LevelPropertyStart + skill);
    public static PropertyInt AltLevelProp(this PropertyAttribute attribute) => (PropertyInt)(Settings.LevelPropertyStart + ATTR_OFFSET + (int)attribute);
    public static PropertyInt AltLevelProp(this PropertyAttribute2nd vital) => (PropertyInt)(Settings.LevelPropertyStart + VITAL_OFFSET + (int)vital);

    public static int GetLevel(this Creature player, Skill key) =>
        player.GetProperty(key.AltLevelProp()) ?? 0;
    public static void SetLevel(this Creature player, Skill key, int value) =>
        player.SetProperty(key.AltLevelProp(), value);
    public static void IncLevel(this Creature player, Skill key, int change) =>
        player.SetProperty(key.AltLevelProp(), player.GetLevel(key) + change);

    public static int GetLevel(this Creature player, PropertyAttribute key) =>
        player.GetProperty(key.AltLevelProp()) ?? 0;
    public static void SetLevel(this Creature player, PropertyAttribute key, int value) =>
        player.SetProperty(key.AltLevelProp(), value);
    public static void IncLevel(this Creature player, PropertyAttribute key, int change) =>
        player.SetProperty(key.AltLevelProp(), player.GetLevel(key) + change);

    public static int GetLevel(this Creature player, PropertyAttribute2nd key) =>
        player.GetProperty(key.AltLevelProp()) ?? 0;
    public static void SetLevel(this Creature player, PropertyAttribute2nd key, int value) =>
        player.SetProperty(key.AltLevelProp(), value);
    public static void IncLevel(this Creature player, PropertyAttribute2nd key, int change) =>
        player.SetProperty(key.AltLevelProp(), player.GetLevel(key) + change);


    //Costs
    public static PropertyInt64 AltSkillProp(this Skill skill) => (PropertyInt64)(Settings.SpentPropertyStart + skill);
    public static PropertyInt64 AltSkillProp(this PropertyAttribute attribute) => (PropertyInt64)(Settings.SpentPropertyStart + ATTR_OFFSET + (int)attribute);
    public static PropertyInt64 AltSkillProp(this PropertyAttribute2nd vital) => (PropertyInt64)(Settings.SpentPropertyStart + VITAL_OFFSET + (int)vital);

    public static long GetCost(this Creature player, Skill key) =>
        player.GetProperty(key.AltSkillProp()) ?? 0;
    public static void SetCost(this Creature player, Skill key, long value) =>
        player.SetProperty(key.AltSkillProp(), value);
    public static void IncCost(this Creature player, Skill key, long change) =>
        player.SetProperty(key.AltSkillProp(), change);

    public static long GetCost(this Creature player, PropertyAttribute key) =>
        player.GetProperty(key.AltSkillProp()) ?? 0;
    public static void SetCost(this Creature player, PropertyAttribute key, long value) =>
        player.SetProperty(key.AltSkillProp(), value);
    public static void IncCost(this Creature player, PropertyAttribute key, long change) =>
        player.SetProperty(key.AltSkillProp(), player.GetCost(key) + change);

    public static long GetCost(this Creature player, PropertyAttribute2nd key) =>
        player.GetProperty(key.AltSkillProp()) ?? 0;
    public static void SetCost(this Creature player, PropertyAttribute2nd key, long value) =>
        player.SetProperty(key.AltSkillProp(), value);
    public static void IncCost(this Creature player, PropertyAttribute2nd key, long change) =>
        player.SetProperty(key.AltSkillProp(), player.GetCost(key) + change);
    #endregion

    #region Cost Functions
    static double SkillCost(SkillAdvancementClass sac, int level) 
    {
        if (Settings.PreferStandard)
        {
            //Return standard cost
            var table = sac == SkillAdvancementClass.Specialized ? DatManager.PortalDat.XpTable.SpecializedSkillXpList : DatManager.PortalDat.XpTable.TrainedSkillXpList;

            if (table.Count > level)
                return table[level];

            //If past standard costs optionally remove the offset from the level
            if (Settings.OffsetByLastStandard)
                level -= table.Count;
        }

        return sac switch
        {
            SkillAdvancementClass.Specialized => Settings.Specialized.GetCost(level),
            _ => Settings.Trained.GetCost(level),
        };
    }

    static double AttributeCost(int level)
    {
        if (Settings.PreferStandard)
        {
            //Return standard cost
            var table = DatManager.PortalDat.XpTable.AttributeXpList;

            if (table.Count > level)
                return table[level];

            //If past standard costs optionally remove the offset from the level
            if (Settings.OffsetByLastStandard)
                level -= table.Count;
        }

        return Settings.Attribute.GetCost(level);
    }
    static double VitalCost(int level)
    {
        if (Settings.PreferStandard)
        {
            //Return standard cost
            var table = DatManager.PortalDat.XpTable.VitalXpList;

            if (table.Count > level)
                return table[level];

            //If past standard costs optionally remove the offset from the level
            if (Settings.OffsetByLastStandard)
                level -= table.Count;
        }

        return Settings.Vital.GetCost(level);
    }
    #endregion

    #region Skills
    /// <summary>
    /// Tries to get the cost needed to raise a skill by one level
    /// </summary>
    public static bool TryGetSkillCost(this Creature player, Skill skill, out long cost)
    {
        cost = long.MaxValue;

        //Verify player can spend xp on skill
        var creatureSkill = player.GetCreatureSkill(skill, false);
        if (creatureSkill == null || creatureSkill.AdvancementClass < SkillAdvancementClass.Trained)
            return false;

        //Use fake skill level and trained status to get a cost for the next level
        var currentLevel = player.GetLevel(skill);
        cost = Convert.ToInt64(SkillCost(creatureSkill.AdvancementClass, currentLevel));

        return true;
    }

    /// <summary>
    /// Handles an attempt from client to raise a skill by one
    /// </summary>
    public static bool TryRaiseSkill(this Player player, Skill skill)
    {
        //Assume the amount the client supplies is always looking to raise the level once
        if (!player.TryGetSkillCost(skill, out var cost))
        {
            player.SendMessage($"Failed to get skill cost for {skill}");
            return false;
        }

        if (cost > player.AvailableExperience)
        {
            player.SendMessage($"Insufficient XP to raise {skill}, {cost - player.AvailableExperience:N0} needed.");
            return false;
        }

        //Try to spend xp
        if (!player.SpendXP(cost))
        {
            player.SendMessage($"Failed to spend {cost:N0} to raise {skill}");
            return false;
        }

        //All clear to add the alternative level
        var creatureSkill = player.GetCreatureSkill(skill, false);
        var level = player.GetLevel(skill) + 1;
        player.SetLevel(skill, level);
        player.IncCost(skill, cost);

        //Update skill
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdateSkill(player, creatureSkill));
        player.SendMessage($"Your base {skill.ToSentence()} skill is now {level}, costing {cost:N0}!", ChatMessageType.Advancement);

        if (skill == Skill.Run && PropertyManager.GetBool("runrate_add_hooks").Item)
            player.HandleRunRateUpdate();


        return true;
    }
    #endregion

    #region Attribute
    /// <summary>
    /// Tries to get the cost needed to raise a skill by one level
    /// </summary>
    public static bool TryGetAttributeCost(this Creature player, PropertyAttribute attribute, out long cost)
    {
        cost = long.MaxValue;

        //Use fake Attribute level and trained status to get a cost for the next level
        var currentLevel = player.GetLevel(attribute);
        cost = Convert.ToInt64(AttributeCost(currentLevel));

        return true;
    }

    /// <summary>
    /// Handles an attempt from client to raise a Attribute by one
    /// </summary>
    public static bool TryRaiseAttribute(this Player player, PropertyAttribute attribute)
    {
        //Assume the amount the client supplies is always looking to raise the level once
        if (!player.TryGetAttributeCost(attribute, out var cost))
        {
            player.SendMessage($"Failed to get Attribute cost for {attribute}");
            return false;
        }

        if (cost > player.AvailableExperience)
        {
            player.SendMessage($"Insufficient XP to raise {attribute}, {cost - player.AvailableExperience:N0} needed.");
            return false;
        }

        //Try to spend xp
        if (!player.SpendXP(cost))
        {
            player.SendMessage($"Failed to spend {cost:N0} to raise {attribute}");
            return false;
        }

        if (!player.Attributes.TryGetValue(attribute, out var creatureAttribute))
        {
            player.SendMessage($"Failed to find attribute {attribute}");
            return false;
        }

        //All clear to add the alternative level
        var level = player.GetLevel(attribute) + 1;
        player.SetLevel(attribute, level);
        player.IncCost(attribute, cost);

        //Update Attribute
        //player.Session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, creatureAttribute));
        player.SendMessage($"Your base {attribute} is now {level}, costing {cost:N0}!", ChatMessageType.Advancement);

        return true;
    }
    #endregion

    #region Vital
    /// <summary>
    /// Tries to get the cost needed to raise a skill by one level
    /// </summary>
    public static bool TryGetVitalCost(this Creature player, PropertyAttribute2nd vital, out long cost)
    {
        cost = long.MaxValue;

        //Use fake vital level and trained status to get a cost for the next level
        var currentLevel = player.GetLevel(vital);
        cost = Convert.ToInt64(VitalCost(currentLevel));

        return true;
    }

    /// <summary>
    /// Handles an attempt from client to raise a vital by one
    /// </summary>
    public static bool TryRaiseVital(this Player player, PropertyAttribute2nd vital)
    {
        //Assume the amount the client supplies is always looking to raise the level once
        if (!player.TryGetVitalCost(vital, out var cost))
        {
            player.SendMessage($"Failed to get vital cost for {vital}");
            return false;
        }

        if (cost > player.AvailableExperience)
        {
            player.SendMessage($"Insufficient XP to raise {vital}, {cost - player.AvailableExperience:N0} needed.");
            return false;
        }

        //Try to spend xp
        if (!player.SpendXP(cost))
        {
            player.SendMessage($"Failed to spend {cost:N0} to raise {vital}");
            return false;
        }

        if (!player.Vitals.TryGetValue(vital, out var creatureVital))
        {
            player.SendMessage($"Failed to find vital {vital}");
            return false;
        }

        //All clear to add the alternative level
        var level = player.GetLevel(vital) + 1;
        player.SetLevel(vital, level);
        player.IncCost(vital, cost);

        //Update vital
        //player.Session.Network.EnqueueSend(new GameMessagePrivateUpdateVital(player, creatureVital));
        player.SendMessage($"Your base {vital.ToSentence()} is now {level}, costing {cost:N0}!", ChatMessageType.Advancement);

        return true;
    }
    #endregion

    //Getting Total cost or cost to level by X more convenient for refunds / purchasing multiple levels
    //public static uint GetTotalCost(int stepNumber, int qty) => GetTotalCost(stepNumber + qty) - GetTotalCost(stepNumber);
    //public static uint GetTotalCost(int stepNumber) => (uint)(multiplier == 1d ? (basePrice * stepNumber) : (basePrice * ((1 - Math.Pow(multiplier, stepNumber)) / (1 - multiplier))));

    //public static int Purchaseable(int currency, int owned) => (int)Math.Floor(Math.Log(Math.Pow(multiplier, (double)owned) - (currency * (1 - multiplier) / basePrice), multiplier) - owned);
    //public static int CurrentLevel(int totalSpent) => Purchaseable(totalSpent, 0);
}

