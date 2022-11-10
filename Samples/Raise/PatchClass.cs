using ACE.Entity.Enum.Properties;
using ACE.Server.Network.GameMessages.Messages;

namespace Raise
{
    [HarmonyPatch]
    public class PatchClass
    {
        #region Settings
        //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
        const int RETRIES = 10;

        public static Settings Settings = new();
        private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
        private static FileInfo settingsInfo = new(settingsPath);

        private static JsonSerializerOptions _serializeOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }

        };

        private static void SaveSettings()
        {
            string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

            if (!settingsInfo.RetryWrite(jsonString, RETRIES))
            {
                ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
            }
        }

        private static void LoadSettings()
        {
            if (!settingsInfo.Exists)
            {
                ModManager.Log($"Creating {settingsInfo}...");
                SaveSettings();
            }
            else
                ModManager.Log($"Loading settings from {settingsPath}...");

            if (!settingsInfo.RetryRead(out string jsonString, RETRIES))
            {
                Mod.State = ModState.Error;
                return;
            }

            try
            {
                Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
            }
            catch (Exception)
            {
                ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
                return;
            }
        }
        #endregion

        #region Raise History
        public static RaiseHistory History = new();
        private static string historyPath = Path.Combine(Mod.ModPath, "History.json");
        private static FileInfo historyInfo = new(historyPath);

        private static void SaveHistory()
        {
            string jsonString = JsonSerializer.Serialize(History, _serializeOptions);

            if (!historyInfo.RetryWrite(jsonString, RETRIES))
            {
                ModManager.Log($"Failed to save history to {historyPath}...", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
            }
        }

        private static void LoadHistory()
        {
            if (historyInfo.Exists)
            {
                ModManager.Log($"Loading history from {historyPath}...");

                if (!historyInfo.RetryRead(out string jsonString, RETRIES))
                {
                    Mod.State = ModState.Error;
                    return;
                }

                try
                {
                    History = JsonSerializer.Deserialize<RaiseHistory>(jsonString, _serializeOptions);
                }
                catch (Exception)
                {
                    ModManager.Log($"Failed to deserialize: {historyPath}", ModManager.LogLevel.Warn);
                    Mod.State = ModState.Error;
                    return;
                }
            }
            else
            {
                ModManager.Log($"Creating {historyInfo}...");
                SaveHistory();
            }
        }

        #endregion

        #region Start/Shutdown
        public static void Start()
        {
            //Need to decide on async use
            Mod.State = ModState.Loading;
            LoadSettings();

            if (Mod.State == ModState.Error)
            {
                ModManager.DisableModByPath(Mod.ModPath);
                return;
            }

            Mod.State = ModState.Running;
        }

        public static void Shutdown()
        {
            if (Mod.State == ModState.Running)
                SaveHistory();

            if (Mod.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion

        #region Commands
        [CommandHandler("raise", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "/raise str/end/coord/focus/self, /raise enlighten, /raise offense, and /raise defense..", "/raise <target> <amount>")]
        public static void HandleAttribute(Session session, params string[] parameters)
        {
            Player player = session.Player;
            RaiseTarget target;
            if (parameters.Length < 1 || !Enum.TryParse<RaiseTarget>(parameters[0], true, out target))
            {
                //If a bad target was selected quit and list the valid commands
                session.Network.EnqueueSend(new GameMessageSystemChat($"You must specify what you wish to raise: /raise <{String.Join("|", Enum.GetNames(typeof(RaiseTarget)))}> [amount]", ChatMessageType.Broadcast));
                return;
            }

            int amt = 1;
            if (parameters.Length > 1)
            {
                if (!int.TryParse(parameters[1], out amt))
                {
                    session.Network.EnqueueSend(new GameMessageSystemChat($"You must specify or omit the amount to raise: /raise <{String.Join("|", Enum.GetNames(typeof(RaiseTarget)))}> [amount]", ChatMessageType.Broadcast));
                    return;
                }
            }
            //Check for bad amounts to level
            if (amt < 1 || amt > Settings.RaiseMax)
            {
                session.Network.EnqueueSend(new GameMessageSystemChat($"Provide an amount from 1-{Settings.RaiseMax}: /raise <{String.Join("|", Enum.GetNames(typeof(RaiseTarget)))}> [amount]", ChatMessageType.Broadcast));
                return;
            }

            //Check if the Rating has already been maxed normally
            var currentLevel = target.GetLevel(player);
            if (currentLevel < target.StartingLevel())
            {
                //If you wanted to gate it
                //session.Network.EnqueueSend(new GameMessageSystemChat($"You must raise {target} to {target.StartingLevel()} with Nalicana before using /raise on it.", ChatMessageType.Broadcast));
                //return;
                //Gift it
                session.Network.EnqueueSend(new GameMessageSystemChat($"You've been granted {target.StartingLevel()} {target} through Nalicana's benificence.", ChatMessageType.Broadcast));
                target.SetLevel(player, target.StartingLevel());
            }

            //Get the cost
            long cost = long.MaxValue;
            if (!target.TryGetCostToLevel(currentLevel, amt, out cost))
            {
                session.Network.EnqueueSend(new GameMessageSystemChat($"Error: Unable to find cost required to raise.  Report to admin.", ChatMessageType.Broadcast));
                return;
            }

            //Acceptable /raise target/amount
            if (target.TryGetAttribute(player, out var attribute))
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
                target.SetLevel(player, currentLevel + (int)amt);
                player.AvailableExperience -= cost;

                //Update the player
                session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableExperience, player.AvailableExperience ?? 0));
                session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, attribute));
                session.Network.EnqueueSend(new GameMessageSystemChat($"Your base {target} is now {attribute.Base}! Spent {cost:N0} xp.", ChatMessageType.Advancement));
                return;
            }

            //Handle luminance
            if (cost > player.AvailableLuminance || !player.SpendLuminance(cost))
            {
                var lumMult = (target == RaiseTarget.World ? Settings.WorldMult : Settings.RaitingMult);
                ChatPacket.SendServerMessage(session, $"Not enough Luminance, you require {lumMult} Luminance per point of {target}.", ChatMessageType.Broadcast);
                return;
            }
            //Update luminance available
            session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(player, PropertyInt64.AvailableLuminance, player.AvailableLuminance ?? 0));

            //If successful in spending luminance level the target
            target.SetLevel(player, currentLevel + (int)amt);

            //...and update player
            switch (target)
            {
                case RaiseTarget.World:
                    ChatPacket.SendServerMessage(session, $"You have raised your World Aug to {player.LumAugAllSkills}! Skills increased by {amt} for {cost:N0} Luminance.", ChatMessageType.Broadcast);
                    session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugAllSkills, player.LumAugAllSkills));
                    break;
                case RaiseTarget.Defense:
                    ChatPacket.SendServerMessage(session, $"Your Damage Reduction Rating has increased by {amt} to {player.LumAugDamageReductionRating} for {cost:N0} Luminance.", ChatMessageType.Broadcast);
                    session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageReductionRating, player.LumAugDamageReductionRating));
                    break;
                case RaiseTarget.Offense:
                    ChatPacket.SendServerMessage(session, $"Your Damage Rating has increased by {amt} to {player.LumAugDamageRating} for {cost:N0} Luminance.", ChatMessageType.Broadcast);
                    session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.LumAugDamageRating, player.LumAugDamageRating));
                    break;
            }
        }

        [CommandHandler("raiserefund", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0, "Refunds costs associated with /raise.")]
        public static void HandleRaiseRefund(Session session, params string[] parameters)
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
            Raise.RaiseRefundToPlayer(player);
            //player.LastRaisedRefundTimestamp = DateTime.Now.Ticks;
        }

        #endregion

        #region Patches
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Creature), nameof(Creature.GetDeathMessage), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
        //public static void PreDeathMessage(DamageHistoryInfo lastDamagerInfo, DamageType damageType, bool criticalHit, ref Creature __instance)
        //{
        //  ...
        //}
        #endregion
    }
}