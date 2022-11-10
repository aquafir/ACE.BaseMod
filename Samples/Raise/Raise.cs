using ACE.Entity.Enum.Properties;
using ACE.Server.Network.GameMessages.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raise
{
    public static class Raise
    {
        #region Helpers
        /// <summary>
        /// Sets the number of times a RaiseTarget has been raised and adjusts the thing it corresponds to
        /// </summary>
        public static void SetLevel(this RaiseTarget target, Player player, int level)
        {
            CreateRaiseTarget(target, player);

            //If it's an attribute being changed, make sure to update the starting value
            if (target.TryGetAttribute(player, out CreatureAttribute attribute))
            {
                //Find the change in current and desired level
                var levelChange = level - GetLevel(target, player);
                attribute.StartingValue += (uint)levelChange;   //Tested to work with negatives
            }

            //Set the appropriate RaisedAttr or rating to desired level
            _ = target switch
            {
                RaiseTarget.World => player.LumAugAllSkills = level,
                RaiseTarget.Defense => player.LumAugDamageReductionRating = level,
                RaiseTarget.Offense => player.LumAugDamageRating = level,
                _ => PatchClass.History.Raises[player.Name][target] = level,
            };
        }

        /// <summary>
        /// Gets or initializes the number of times a RaiseTarget has been raised
        /// </summary>
        public static int GetLevel(this RaiseTarget target, Player player)
        {
            CreateRaiseTarget(target, player);

            //Todo: decide on handling attributes different from luminance/things with existing player property
            //return PatchClass.Settings.PLAYER_RAISES[player.Name][target];

            return target switch
            {
                RaiseTarget.World => player.LumAugAllSkills,
                RaiseTarget.Defense => player.LumAugDamageReductionRating,
                RaiseTarget.Offense => player.LumAugDamageRating,
                _ => PatchClass.History.Raises[player.Name][target],
            };
        }

        /// <summary>
        /// Initial levels for different targets
        /// </summary>
        public static int StartingLevel(this RaiseTarget target)
        {
            switch (target)
            {
                //Attributes
                case RaiseTarget t when t.IsAttribute(): return 0;
                //Ratings return the normal max.
                ////Comment out to allow leveling down to 0 which would let a player go through the normal process to net a little Lum
                case RaiseTarget.World: return 10;  //Max World 
                case RaiseTarget.Defense: return 5;
                case RaiseTarget.Offense: return 5;
                default: return 0;
            }
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
            long avgCost = (long)(PatchClass.Settings.AttrMult * avgLevel / (PatchClass.Settings.AttrMultDecay - PatchClass.Settings.AttrLevelDecay * avgLevel));

            try
            {
                checked
                {
                    cost = target switch
                    {
                        RaiseTarget t when t.IsAttribute() => checked(avgCost * numLevels),
                        RaiseTarget.Offense => checked(numLevels * PatchClass.Settings.RaitingMult),
                        RaiseTarget.Defense => checked(numLevels * PatchClass.Settings.RaitingMult),
                        RaiseTarget.World => checked(numLevels * PatchClass.Settings.WorldMult),
                    };
                }
                return true;
            }
            catch (OverflowException ex) { }  //Eats errors to return false?  Can't remember
            return false;
        }

        /// <summary>
        /// True if RaiseTarget is a CreatureAttribute
        /// </summary>
        private static bool IsAttribute(this RaiseTarget target) => target < RaiseTarget.World;
        /// <summary>
        /// Get the CreatureAttribute corresponding to a RaiseTarget if that target is an attribute
        /// </summary>
        public static bool TryGetAttribute(this RaiseTarget target, Player player, out CreatureAttribute? attribute)
        {
            attribute = null;
            if (!target.IsAttribute())
                return false;

            //If the target is an attribute set it and succeed
            attribute = player.Attributes[(PropertyAttribute)target];  //TODO: Requires the RaiseTarget enum to line up with the PropertyAttribute-- probably should do this a better way
            return true;
        }
        #endregion

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
                    var level = target.GetLevel(player);
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
        private static void CreateRaiseTarget(RaiseTarget target, Player player)
        {
            //Add record of player if needed
            if (!PatchClass.History.Raises.TryGetValue(player.Name, out var levels))
            {
                levels = new();
                PatchClass.History.Raises.Add(player.Name, levels);
            }
            if (!levels.ContainsKey(target))
                levels.Add(target, 0);
        }
    }
}
