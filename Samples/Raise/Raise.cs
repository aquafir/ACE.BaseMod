namespace Raise;

[HarmonyPatchCategory(nameof(Raise))]
[CommandCategory(nameof(Raise))]
public static class Raise
{
    static RaiseSettings Settings => PatchClass.Settings.Raise;

    [CommandHandler("raise", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "/raise str/end/coord/focus/self, /raise enlighten, /raise offense, and /raise defense..", "/raise <target> <amount>")]
    public static void HandleAttribute(Session session, params string[] parameters)
    {
        Player player = session.Player;
        RaiseTarget target;

        #region Validate
        if (parameters.Length < 1 || !Enum.TryParse(parameters[0], true, out target))
        {
            //If a bad target was selected quit and list the valid commands
            session.Network.EnqueueSend(new GameMessageSystemChat($"You must specify what you wish to raise: /raise <{string.Join("|", Enum.GetNames(typeof(RaiseTarget)))}> [amount]", ChatMessageType.Broadcast));
            return;
        }

        int amt = 1;
        if (parameters.Length > 1)
        {
            if (!int.TryParse(parameters[1], out amt))
            {
                session.Network.EnqueueSend(new GameMessageSystemChat($"You must specify or omit the amount to raise: /raise <{string.Join("|", Enum.GetNames(typeof(RaiseTarget)))}> [amount]", ChatMessageType.Broadcast));
                return;
            }
        }
        //Check for bad amounts to level
        if (amt < 1 || amt > Settings.RaiseMax)
        {
            session.Network.EnqueueSend(new GameMessageSystemChat($"Provide an amount from 1-{Settings.RaiseMax}: /raise <{string.Join("|", Enum.GetNames(typeof(RaiseTarget)))}> [amount]", ChatMessageType.Broadcast));
            return;
        }
        #endregion

        //Check if the Rating has already been maxed normally
        var currentLevel = target.CurrentLevel(player);
        if (currentLevel < target.StartingLevel())
        {
            //To gift up to the starting level:
            session.Network.EnqueueSend(new GameMessageSystemChat($"You've been granted {target.StartingLevel()} {target} through Nalicana's benificence.", ChatMessageType.Broadcast));
            target.SetLevel(player, target.StartingLevel());
            //currentLevel = target.StartingLevel();

            //To gate it
            //session.Network.EnqueueSend(new GameMessageSystemChat($"You must raise {target} to {target.StartingLevel()} with Nalicana before using /raise on it.", ChatMessageType.Broadcast));
            //return;
        }

        //Get the cost
        long cost = long.MaxValue;
        if (!target.TryGetCostToLevel(currentLevel, amt, out cost))
        {
            session.Network.EnqueueSend(new GameMessageSystemChat($"Error: Unable to find cost required to raise.  Report to admin.", ChatMessageType.Broadcast));
            return;
        }

        //If there's an acceptable attribute try to raise it
        if (target.TryGetAttribute(player, out var attribute))
        {
            RaiseAttribute(session, player, target, amt, currentLevel, cost, attribute);
            return;
        }

        RaiseDefault(session, player, target, amt, currentLevel, cost);
    }

    private static void RaiseAttribute(Session session, Player player, RaiseTarget target, int amt, int currentLevel, long cost, CreatureAttribute? attribute)
    {
        //Require max attr first
        if (!attribute.IsMaxRank)
        {
            session.Network.EnqueueSend(new GameMessageSystemChat($"Your {target} is not max level yet. Please raise {target} until it is maxed out. ", ChatMessageType.Broadcast));
            return;
        }

        //Halt if there isn't enough xp
        if (session.Player.AvailableExperience < cost)
        {
            session.Network.EnqueueSend(new GameMessageSystemChat($"You do not have enough available experience to level your {target}{(amt == 1 ? "" : $" {amt} times")}.  {cost:N0} needed.", ChatMessageType.Broadcast));
            return;
        }

        //Otherwise go ahead raising the attribute
        player.AvailableExperience -= cost;
        target.SetLevel(player, currentLevel + amt);

        //Update the player
        UpdatePlayer(session, player, target, amt, cost);
        return;
    }

    private static void RaiseDefault(Session session, Player player, RaiseTarget target, int amt, int currentLevel, long cost)
    {
        //Handle luminance
        if (cost > player.AvailableLuminance || !player.SpendLuminance(cost))
        {
            var lumMult = target == RaiseTarget.World ? Settings.WorldMult : Settings.RatingMulti;
            ChatPacket.SendServerMessage(session, $"Not enough Luminance, you require {lumMult} Luminance per point of {target}.", ChatMessageType.Broadcast);
            return;
        }
        //Update luminance available -- should be taken care of by Spend
        //session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableLuminance, player.AvailableLuminance ?? 0));

        //If successful in spending luminance level the target
        target.SetLevel(player, currentLevel + amt);

        //...and update player
        UpdatePlayer(session, player, target, amt, cost);
    }

    private static void UpdatePlayer(Session session, Player player, RaiseTarget target, int amt, long cost)
    {
        switch (target)
        {
            //case RaiseTarget.World:
            //    ChatPacket.SendServerMessage(session, $"You have raised your World Aug to {player.LumAugAllSkills}! Skills increased by {amt} for {cost:N0} Luminance.", ChatMessageType.Broadcast);
            //    break;
            //case RaiseTarget.Defense:
            //    ChatPacket.SendServerMessage(session, $"Your Damage Reduction Rating has increased by {amt} to {player.LumAugDamageReductionRating} for {cost:N0} Luminance.", ChatMessageType.Broadcast);
            //    break;
            //case RaiseTarget.Offense:
            //    ChatPacket.SendServerMessage(session, $"Your Damage Rating has increased by {amt} to {player.LumAugDamageRating} for {cost:N0} Luminance.", ChatMessageType.Broadcast);
            //    break;

            case var t when t.TryGetAttribute(player, out var attribute):
                session.Network.EnqueueSend(new GameMessageSystemChat($"Your base {target} is now {attribute.Base}! {amt} levels for {cost:N0} experience.", ChatMessageType.Advancement));
                break;

            default:
                ChatPacket.SendServerMessage(session, $"You have raised your {target} to {target.CurrentLevel(player)}! {amt} levels for {cost:N0} Luminance.", ChatMessageType.Broadcast);
                break;
        }
    }

    [CommandHandler("raiserefund", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0, "Refunds costs associated with /raise.")]
    public static void HandleRaiseRefund(Session session, params string[] parameters) => HandleRefund(session);
    [CommandHandler("rr", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0, "Refunds costs associated with /raise.")]
    public static void HandleRR(Session session, params string[] parameters) => HandleRefund(session);

    private static void HandleRefund(Session session)
    {
        var player = session.Player;
        //var timeLapse = DateTime.Now - new DateTime(player.LastRaisedRefundTimestamp);
        //var timeToUse = Settings.RAISE_TIME_BETWEEN_REFUND - timeLapse;

        ////Check if enough time has passed
        //if (timeToUse.TotalSeconds > 0)
        //{
        //ChatPacket.SendServerMessage(session, $"You must wait {timeToUse.TotalMinutes:0.##} minutes before refunding.", ChatMessageType.Broadcast);
        //return;
        //}

        //Refund player and set last use
        //TODO: Check if the player has anything to refund before setting a cooldown
        RaiseRefundToPlayer(player);
        //player.LastRaisedRefundTimestamp = DateTime.Now.Ticks;
    }

    [CommandHandler("raiselist", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "/raise str/end/coord/focus/self, /raise enlighten, /raise offense, and /raise defense..", "/raise <target> <amount>")]
    public static void HandleRaiseList(Session session, params string[] parameters) => HandleRaiseList(session);
    [CommandHandler("rl", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "/raise str/end/coord/focus/self, /raise enlighten, /raise offense, and /raise defense..", "/raise <target> <amount>")]
    public static void HandleRL(Session session, params string[] parameters) => HandleRaiseList(session);

    /// <summary>
    /// Send the player a list of the levels and costs of their RaiseTargets
    /// </summary>
    /// <param name="session"></param>
    private static void HandleRaiseList(Session session)
    {
        var player = session.Player;
        var sb = new StringBuilder("\nRaised target levels: ");

        foreach (var targetType in Enum.GetValues<RaiseTarget>())
        {
            if (!targetType.TryGetCostToLevel(targetType.StartingLevel(), targetType.CurrentLevel(player) - targetType.StartingLevel(), out var cost))
                cost = 0;
            //if (!targetType.TryGetCostToLevel(targetType.StartingLevel(), targetType.GetLevel(player) - targetType.StartingLevel(), out var cost))
            //    cost = 0;

            sb.Append($"\n{targetType.FromProperty(player),5} - {targetType}\n{cost,15:E2}");
        }

        player.SendMessage(sb.ToString());
    }

    /// <summary>
    /// Refunds all resources used in the /raise command
    /// </summary>
    /// <param name="player">Player to refund</param>
    /// <returns>Update messages that would be sent to the player.</returns>
    private static List<string> RaiseRefundResources(Player player)
    {
        var playerMessages = new List<string>();

        try
        {
            //Try to refund every target
            foreach (var target in Enum.GetValues<RaiseTarget>())
            {
                var level = target.CurrentLevel(player);
                var startLevel = target.StartingLevel();
                var timesLeveled = level - startLevel;

                //Check if anything invested
                if (timesLeveled < 1)
                {
                    continue;
                }

                //Get resources spent
                if (!target.TryGetCostToLevel(startLevel, timesLeveled, out long cost))
                {
                    playerMessages.Add($"Failed to get cost for {timesLeveled} levels of {target}.");
                    continue;
                }

                //Refund the resource and update the player
                if (target.TryGetAttribute(player, out var attribute))
                {
                    player.AvailableExperience += cost;
                    playerMessages.Add($"Refunding {timesLeveled} levels of {target} for {cost:N0} xp.");
                    //session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, attribute));
                }
                else
                {
                    player.AvailableLuminance += cost;
                    playerMessages.Add($"Refunding {timesLeveled} levels of {target} for {cost:N0} lum.");
                }
                //Finally, set the level to what it should be
                target.SetLevel(player, startLevel);
            }
        }
        catch (Exception ex)
        {
            playerMessages.Add($"Failed to reset /raise values for {player.Account}\\{player.Name}.");
        }

        return playerMessages;
    }

    /// <summary>
    /// Refunds all resources used in the /raise command and informs the refunded player if possible.
    /// </summary>
    /// <param name="player">Player to refund</param>
    /// <param name="session">Optional session used to send messages and updated properties to.</param>
    public static void RaiseRefundToPlayer(Player player, Session session = null)
    {
        if (player == null)
            return;

        //Refund the player
        var refundMessages = RaiseRefundResources(player);

        //Providing a session will use that for the messaging, in case an admin wants to specify using their session.
        //Falls back to the players session if available
        if (session == null)
        {
            if (player.Session != null)
                session = player.Session;
            else
                return;
        }

        //Send messages
        foreach (var msg in refundMessages)
            ChatPacket.SendServerMessage(session, msg, ChatMessageType.Broadcast);

        UpdatePlayerRaiseProperties(player, session);
    }

    /// <summary>
    /// Send property updates for any value that may be changed through the /raise command.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="session"></param>
    private static void UpdatePlayerRaiseProperties(Player player, Session session)
    {
        //Send updated attributes
        foreach (var target in Enum.GetValues<RaiseTarget>())
        {
            if (target.TryGetAttribute(player, out var attribute))
                session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, attribute));
        }
        //Send player their updated ratings
        session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugAllSkills, player.LumAugAllSkills));
        session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageReductionRating, player.LumAugDamageReductionRating));
        session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageRating, player.LumAugDamageRating));
        //Send updated resources
        session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableLuminance, player.AvailableLuminance ?? 0));
        session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableExperience, player.AvailableExperience ?? 0));
    }

    /// <summary>
    /// Add a Player or RaiseTarget to Settings where it is tracked
    /// </summary>
    //private static void CreateRaiseTarget(RaiseTarget target, Player player)
    //{
    //    //Add record of player if needed
    //    if (!PatchClass.History.Raises.TryGetValue(player.Name, out var levels))
    //    {
    //        levels = new();
    //        PatchClass.History.Raises.Add(player.Name, levels);
    //    }
    //    if (!levels.ContainsKey(target))
    //        levels.Add(target, 0);
    //}
}