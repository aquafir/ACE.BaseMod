namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(LevelCost))]
public class LevelCost : AngouriMathPatch
{
    #region Fields / Props   
    public override string Formula { get; set; } = "1000x^(3/2)";
    protected override Dictionary<string, MType> Variables { get; } = new()
    {
        ["x"] = MType.Long, // level
    };

    //Function parsed from formula used in patches
    static Func<long, int> func;
    //Approach using Math.Net that can interpolate from existing costs
    //static Barycentric interpolation;
    //Todo: Use an array sized to max level?
    static Dictionary<uint, ulong> totalCosts = new();
    static MethodInfo updateXpVitaeMethod;
    #endregion

    #region Start / Stop
    public override void Start()
    {
        //If you can parse the formulas patch the corresponding category
        if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
        {
            Mod.Instance.Harmony.PatchCategory(nameof(LevelCost));
        }
        else
            throw new Exception($"Failure parsing formula: {Formula}");

        //Example of getting reference to an inaccessible private method in Player instead of replacing
        try
        {
            updateXpVitaeMethod = AccessTools.Method(typeof(Player), "UpdateXpVitae", new Type[] { typeof(long) });
        }
        catch (Exception ex)
        {
            ModManager.Log("Error accessing private Player method 'UpdateXpVitae'");
        }

        //Reset total cost cache
        totalCosts.Clear();

        //Interpolate level costs from dats
        //try
        //{
        //    var ydata = DatManager.PortalDat.XpTable.CharacterLevelXPList.Skip(1).Select(x => (double)x).ToArray(); //Skip 1 for the double level 0
        //    var xdata = Enumerable.Range(1, ydata.Length).Select(x => (double)x).ToArray();
        //    interpolation = Barycentric.InterpolateRationalFloaterHormannSorted(xdata, ydata);
        //}
        //catch (Exception ex)
        //{
        //    ModManager.Log("Error interpolating from present XP table values: " + ex.Message);
        //}

        //Backup and replace XpTable
        //originalXpTable = new();
        //originalXpTable.AddRange(DatManager.PortalDat.XpTable.CharacterLevelXPList);
        //originalCreditTable = new();
        //originalCreditTable.AddRange(DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList);
        //DatManager.PortalDat.XpTable.CharacterLevelXPList.Clear();
        //DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Clear();

        //Replace level costs.  Doesn't require replacing anything with this approach
        //var table = DatManager.PortalDat.XpTable;
        //var xp = table.CharacterLevelXPList;
        //var skills = table.CharacterLevelSkillCreditList;
        //xp.Clear();
        ////table.CharacterLevelSkillCreditList.Clear();
        //for (uint i = 1; i <= GetMaxLevel(); i++)
        //{
        //    xp.Add((ulong)LevelCost(i));
        //    if (skills.Count < i)
        //        skills.Add(i % 10 == 0 ? 1u : 0);
        //}
    }

    public override void Shutdown()
    {
        //func = null;

        //Clear to make sure nothing but the original tables are added
        //DatManager.PortalDat.XpTable.CharacterLevelXPList.Clear();
        //DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Clear();

        //DatManager.PortalDat.XpTable.CharacterLevelXPList.AddRange(originalXpTable);
        //DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.AddRange(originalCreditTable);
    }
    #endregion

    #region Commands
    //Standalone non-persistent command to adjust level costs
    //[CommandHandler("setmax", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 1, "Set max level.")]
    //public static void HandleMax(Session session, params string[] parameters)
    //{
    //    //Try to parse first param for max level
    //    if (parameters.Length < 1 || !uint.TryParse(parameters[0], out var maxLevel))
    //        return;
    //    //1 billion xp per level post 275
    //    var costPerLevel = 1_000_000_000u;
    //    //Add a skill credit every 10 levels
    //    var creditInterval = 10;

    //    for (int i = DatManager.PortalDat.XpTable.CharacterLevelXPList.Count; i <= maxLevel; i++)
    //    {
    //        var cost = DatManager.PortalDat.XpTable.CharacterLevelXPList.Last() + costPerLevel;
    //        var credits = (uint)(i % creditInterval == 0 ? 1 : 0); // + DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Last();
    //        DatManager.PortalDat.XpTable.CharacterLevelXPList.Add(cost);
    //        DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Add(credits);
    //        session?.Player?.SendMessage($"Adding level {i} for {cost}.  {credits} skill credits.");
    //    }
    //}

    [CommandHandler("costs", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 2, "Print level information in a range.", "/costs startLvl numLevels")]
    public static void HandleCosts(Session session, params string[] parameters)
    {
        //Try to parse first param for max level
        if (parameters.Length < 1 || !uint.TryParse(parameters[0], out var low))
            return;
        if (parameters.Length < 2 || !uint.TryParse(parameters[1], out var range))
            return;

        var sb = new StringBuilder("Level costs: \n");

        uint max = Math.Min(GetMaxLevel(session.Player), low + range);
        for (uint i = low; i < max; i++)
        {
            var credits = GetLevelSkillCredits(session.Player, (int)i);
            sb.Append($"  {i}: {CostOfLevel(session.Player, i),-20:N0}{TotalLevelCost(session.Player, i),-20:N0}  {(credits > 0 ? credits + " credit" : "")}\n");
        }
        session?.Player?.SendMessage(sb.ToString());
    }

    [CommandHandler("resetlevel", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleResetLevel(Session session, params string[] parameters)
    {
        var p = session?.Player;
        p.Level = 1;
        p.TotalExperience = 0;
        p.AvailableExperience = 0;

        var xp = new GameMessagePrivateUpdatePropertyInt64(p, PropertyInt64.AvailableExperience, p.AvailableExperience ?? 0);
        var totalXp = new GameMessagePrivateUpdatePropertyInt64(p, PropertyInt64.TotalExperience, p.TotalExperience ?? 0);
        var level = new GameMessagePrivateUpdatePropertyInt(p, PropertyInt.Level, p.Level ?? 1);
        session.Network.EnqueueSend(xp, totalXp, level);
    }
    #endregion

    #region Logic
    //Most important thing to implement
    /// <summary>
    /// Cost for a given level
    /// </summary>
    static long CostOfLevel(Player player, uint level)
    {
        ulong cost = long.MaxValue;
        if (level < 0 || level > GetMaxLevel(player))
            return long.MaxValue;

        if (func is not null)
        {
            //cost = (ulong)func((double)level);
            cost = (ulong)func((int)level);
        }
        //else if (interpolation is not null)
        //{
        //    cost = (ulong)interpolation.Interpolate(level);
        //}

        return cost > long.MaxValue ? long.MaxValue : (long)cost;
    }

    /// <summary>
    /// Total cost to reach a given level
    /// </summary>
    static ulong TotalLevelCost(Player player, uint level)
    {
        if (level < 0 || level > GetMaxLevel(player))
            return ulong.MaxValue;

        //Todo: Better ways to implement this (closed-form), but this should work with just LevelCost implemented?
        if (totalCosts.ContainsKey(level))
            return totalCosts[level];

        //Fill in total costs up to requested level
        ulong cost = 0;
        for (uint i = 1; i <= level; i++)
        {
            cost += (ulong)CostOfLevel(player, i);
            totalCosts.TryAdd(i, cost);
        }
        return cost;
    }

    /// <summary>
    /// Remaining experience a given player needs to reach a given level
    /// </summary>
    static long CostToLevel(Player player, uint level) => CostOfLevel(player, level) - player.TotalExperience.Value;
    //GetMaxLevel is static and is nulled when called for proficiency.  Would require some thought for per-player max levels
    static uint GetMaxLevel(Player player) => player is null ? PatchClass.Settings.MaxLevel : PatchClass.Settings.MaxLevel;
    static uint GetLevelSkillCredits(Player player) => GetLevelSkillCredits(player, player.Level ?? 0);
    static uint GetLevelSkillCredits(Player player, int level)
    {
        //Use player table for skill credits if available
        var xpTable = DatManager.PortalDat.XpTable;
        if (level < xpTable.CharacterLevelSkillCreditList.Count)
            return xpTable.CharacterLevelSkillCreditList[level];

        //Otherwise default to a credit every 10 levels
        return level % 10 == 0 ? 1u : 0;
    }

    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.IsMaxLevel), MethodType.Getter)]
    public static bool PreGet_IsMaxLevel(ref Player __instance, ref bool __result)
    {
        __result = __instance.Level >= GetMaxLevel(__instance);

        //Return false to override
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), "UpdateXpAndLevel", new Type[] { typeof(long), typeof(XpType) })]
    public static bool PreUpdateXpAndLevel(long amount, XpType xpType, ref Player __instance)
    {
        var maxLevel = GetMaxLevel(__instance);
        var maxLevelXp = TotalLevelCost(__instance, maxLevel);

        if (__instance.Level != maxLevel)
        {
            var addAmount = amount;

            var amountLeftToEnd = (long)maxLevelXp - __instance.TotalExperience ?? 0;
            if (amount > amountLeftToEnd)
                addAmount = amountLeftToEnd;

            __instance.AvailableExperience += addAmount;
            __instance.TotalExperience += addAmount;

            var xpTotalUpdate = new GameMessagePrivateUpdatePropertyInt64(__instance, PropertyInt64.TotalExperience, __instance.TotalExperience ?? 0);
            var xpAvailUpdate = new GameMessagePrivateUpdatePropertyInt64(__instance, PropertyInt64.AvailableExperience, __instance.AvailableExperience ?? 0);
            __instance.Session.Network.EnqueueSend(xpTotalUpdate, xpAvailUpdate);

            //CheckForLevelup();
            PreCheckForLevelup(ref __instance);
        }

        if (xpType == XpType.Quest)
            __instance.Session.Network.EnqueueSend(new GameMessageSystemChat($"You've earned {amount:N0} experience.", ChatMessageType.Broadcast));

        if (__instance.HasVitae && xpType != XpType.Allegiance)
            updateXpVitaeMethod?.Invoke(__instance, new object[] { amount });
        //UpdateXpVitae(amount);

        //Return false to override
        return false;
    }

    /// <summary>
    /// Returns the maximum possible character level
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetMaxLevel))]
    public static bool PreGetMaxLevel(ref uint __result)
    {
        //Proficiency.OnSuccessUse would need a rewrite
        //__result = __instance is null ? GetMaxLevel(null) : GetMaxLevel(__instance);
        __result = GetMaxLevel(null);

        //Return false to override
        return false;
    }

    /// <summary>
    /// Returns the remaining XP required to reach a level
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetRemainingXP), new Type[] { typeof(uint) })]
    public static bool PreGetRemainingXP(uint level, ref Player __instance, ref long? __result)
    {
        var maxLevel = GetMaxLevel(__instance);
        if (level < 1 || level > maxLevel)
            __result = null;
        else
            __result = CostToLevel(__instance, level);

        //Return false to override
        return false;
    }

    /// <summary>
    /// Returns the remaining XP required to the next level
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetRemainingXP), new Type[] { })]
    public static bool PreGetRemainingXP(ref Player __instance, ref ulong __result)
    {
        uint level = (uint)(__instance.Level ?? 0);
        __result = __instance.Level >= GetMaxLevel(__instance) ? 0 :
            (ulong)CostToLevel(__instance, level + 1);

        //Return false to override
        return false;
    }

    /// <summary>
    /// Returns the total XP required to reach a level
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetTotalXP), new Type[] { typeof(int) })]
    public static bool PreGetTotalXP(int level, ref Player __instance, ref ulong __result)
    {
        __result = TotalLevelCost(__instance, (uint)level);

        //Return false to override
        return false;
    }

    /// <summary>
    /// Returns the total amount of XP required for a player reach max level
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.MaxLevelXP), MethodType.Getter)]
    public static bool PreGet_MaxLevelXP(ref Player __instance, ref long __result)
    {
        __result = (long)TotalLevelCost(__instance, GetMaxLevel(__instance));

        //Return false to override
        return false;
    }

    /// <summary>
    /// Returns the XP required to go from level A to level B
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetXPBetweenLevels), new Type[] { typeof(int), typeof(int) })]
    public static bool PreGetXPBetweenLevels(int levelA, int levelB, ref Player __instance, ref ulong __result)
    {
        // special case for max level
        var maxLevel = (int)GetMaxLevel(__instance);

        levelA = Math.Clamp(levelA, 1, maxLevel - 1);
        levelB = Math.Clamp(levelB, 1, maxLevel);

        var levelA_totalXP = TotalLevelCost(__instance, (uint)levelA);
        var levelB_totalXP = TotalLevelCost(__instance, (uint)levelB);

        __result = levelB_totalXP - levelA_totalXP;

        //Return false to override
        return false;
    }

    /// <summary>
    /// Determines if the player has advanced a level
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), "CheckForLevelup")]
    public static bool PreCheckForLevelup(ref Player __instance)
    {
        var maxLevel = GetMaxLevel(__instance);

        if (__instance.Level >= maxLevel) return false;

        var startingLevel = __instance.Level;
        bool creditEarned = false;

        // increases until the correct level is found
        while ((ulong)(__instance.TotalExperience ?? 0) >= TotalLevelCost(__instance, (uint)(__instance.Level ?? 0) + 1))
        {
            __instance.Level++;

            // increase the skill credits if the chart allows this level to grant a credit
            if (GetLevelSkillCredits(__instance) > 0)
            {
                __instance.AvailableSkillCredits += (int)GetLevelSkillCredits(__instance);
                __instance.TotalSkillCredits += (int)GetLevelSkillCredits(__instance);
                creditEarned = true;
            }

            // break if we reach max
            if (__instance.Level == maxLevel)
            {
                __instance.PlayParticleEffect(PlayScript.WeddingBliss, __instance.Guid);
                break;
            }
        }

        if (__instance.Level > startingLevel)
        {
            var message = __instance.Level == maxLevel ? $"You have reached the maximum level of {__instance.Level}!" : $"You are now level {__instance.Level}!";

            message += __instance.AvailableSkillCredits > 0 ? $"\nYou have {__instance.AvailableExperience:#,###0} experience points and {__instance.AvailableSkillCredits} skill credits available to raise skills and attributes." : $"\nYou have {__instance.AvailableExperience:#,###0} experience points available to raise skills and attributes.";

            var levelUp = new GameMessagePrivateUpdatePropertyInt(__instance, PropertyInt.Level, __instance.Level ?? 1);
            var currentCredits = new GameMessagePrivateUpdatePropertyInt(__instance, PropertyInt.AvailableSkillCredits, __instance.AvailableSkillCredits ?? 0);

            if (__instance.Level != maxLevel && !creditEarned)
            {
                var nextLevelWithCredits = 0;

                for (int i = (__instance.Level ?? 0) + 1; i <= maxLevel; i++)
                {
                    if (GetLevelSkillCredits(__instance) > 0)
                    {
                        nextLevelWithCredits = i;
                        break;
                    }
                }
                message += $"\nYou will earn another skill credit at level {nextLevelWithCredits}.";
            }

            if (__instance.Fellowship != null)
                __instance.Fellowship.OnFellowLevelUp(__instance);

            if (__instance.AllegianceNode != null)
                __instance.AllegianceNode.OnLevelUp();

            __instance.Session.Network.EnqueueSend(levelUp);

            __instance.SetMaxVitals();

            // play level up effect
            __instance.PlayParticleEffect(PlayScript.LevelUp, __instance.Guid);

            __instance.Session.Network.EnqueueSend(new GameMessageSystemChat(message, ChatMessageType.Advancement), currentCredits);
        }

        //Return false to override
        return false;
    }

    #region Proficiency OnSuccessUse override for allowing player injection of MaxLevel
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Proficiency), nameof(Proficiency.OnSuccessUse), new Type[] { typeof(Player), typeof(CreatureSkill), typeof(uint) })]
    //public static bool PreOnSuccessUse(Player player, CreatureSkill skill, uint difficulty, ref Proficiency __instance)
    //{
    //    if (player.IsOlthoiPlayer)
    //        return false;

    //    // ensure skill is at least trained
    //    if (skill.AdvancementClass < SkillAdvancementClass.Trained)
    //        return false;

    //    var last_difficulty = skill.PropertiesSkill.ResistanceAtLastCheck;
    //    var last_used_time = skill.PropertiesSkill.LastUsedTime;

    //    var currentTime = Time.GetUnixTime();

    //    var timeDiff = currentTime - last_used_time;

    //    if (timeDiff < 0)
    //    {
    //        // can happen if server clock is rewound back in time
    //        ModManager.Log($"Proficiency.OnSuccessUse({player.Name}, {skill.Skill}, {difficulty}) - timeDiff: {timeDiff}", ModManager.LogLevel.Warn);
    //        skill.PropertiesSkill.LastUsedTime = currentTime;       // update to prevent log spam
    //        return false;
    //    }

    //    var difficulty_check = difficulty > last_difficulty;
    //    var time_check = timeDiff >= Proficiency.FullTime.TotalSeconds;

    //    if (difficulty_check || time_check)
    //    {
    //        // todo: not independent variables?
    //        // always scale if timeDiff < FullTime?
    //        var timeScale = 1.0f;
    //        if (!time_check)
    //        {
    //            // 10 mins elapsed from 15 min FullTime:
    //            // 0.66f timeScale
    //            timeScale = (float)(timeDiff / Proficiency.FullTime.TotalSeconds);

    //            // any rng involved?
    //        }

    //        skill.PropertiesSkill.ResistanceAtLastCheck = difficulty;
    //        skill.PropertiesSkill.LastUsedTime = currentTime;

    //        player.ChangesDetected = true;

    //        if (player.IsMaxLevel) return false;

    //        var pp = (uint)Math.Round(difficulty * timeScale);
    //        var totalXPGranted = (long)Math.Round(pp * 1.1f);   // give additional 10% of proficiency XP to unassigned XP

    //        if (totalXPGranted > 10000)
    //        {
    //            ModManager.Log($"Proficiency.OnSuccessUse({player.Name}, {skill.Skill}, {difficulty}) - totalXPGranted: {totalXPGranted:N0}", ModManager.LogLevel.Warn);
    //        }

    //        var maxLevel = Player.GetMaxLevel();
    //        var remainingXP = player.GetRemainingXP(maxLevel).Value;

    //        if (totalXPGranted > remainingXP)
    //        {
    //            // checks and balances:
    //            // total xp = pp * 1.1
    //            // pp = total xp / 1.1

    //            totalXPGranted = remainingXP;
    //            pp = (uint)Math.Round(totalXPGranted / 1.1f);
    //        }

    //        // if skill is maxed out, but player is below MaxLevel,
    //        // not sure if retail granted 0%, 10%, or 110% of the pp to TotalExperience here
    //        // since pp is such a miniscule system at the higher levels,
    //        // going to just naturally add it to TotalXP for now..

    //        pp = Math.Min(pp, skill.ExperienceLeft);

    //        //Console.WriteLine($"Earned {pp} PP ({skill.Skill})");

    //        // send CP to player as unassigned XP
    //        player.GrantXP(totalXPGranted, XpType.Proficiency, ShareType.None);

    //        // send PP to player as skill XP, which gets spent from the CP sent
    //        if (pp > 0)
    //        {
    //            player.HandleActionRaiseSkill(skill.Skill, pp);
    //        }
    //    }

    //    //Return false to override
    //    return false;
    //}

    #endregion

    #region XpTable Getter Swap Approach
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(PortalDatDatabase), nameof(PortalDatDatabase.XpTable), MethodType.Getter)]
    //public static bool Get_XpTable(ref XpTable __result)
    //{
    //    //__result = someXpTable;

    //    //Return false to override
    //    return false;
    //}

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(XpTable), nameof(XpTable.CharacterLevelSkillCreditList), MethodType.Getter)]
    //public static bool PreGet_CharacterLevelSkillCreditList(ref XpTable __instance, ref List<uint> __result)
    //{
    //    //__result = someSkillCreditTable;

    //    //Return false to override
    //    return false;
    //}
    #endregion
    #endregion
}