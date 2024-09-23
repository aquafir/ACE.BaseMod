namespace Tower;

[CommandCategory(nameof(Feature.OfflineProgress))]
[HarmonyPatchCategory(nameof(Feature.OfflineProgress))]
public static class OfflineProgress
{
    static double MinTime = TimeSpan.FromHours(1).TotalSeconds;
    static double MaxTime = TimeSpan.FromHours(24 * 3).TotalSeconds;
    static PropertyBool ProgressOffline = (PropertyBool)39997;

    static OfflineProgressSettings Settings => PatchClass.Settings.OfflineProgress;

    static double GetLastClaimed(this Player player) => Time.GetUnixTime() - (player.LogoffTimestamp ?? Time.GetUnixTime());

    static void GiveOfflineProgress(this Player player)
    {
        if (!player.OfflineProgressEnabled())
            return;

        var highest = player.GetHighestCompletedFloor();
        if (highest is null)
            return;

        var lapsed = Math.Min(Settings.MaxTime.TotalSeconds, player.GetLastClaimed()).ToTimeSpan();

        //Require finishing a level to be eligible
        if (!Settings.RewardTiers.TryGetValue(highest.Index, out var tier))
            return;

        var xp = (long)(lapsed.TotalHours * tier.XpPerHour);
        var lootQty = lapsed.TotalHours * tier.LootPerHour;

        player.GrantXP(xp, XpType.Admin, ShareType.None);
        player.TryCreateItems($"{tier.LootWcid} {(int)lootQty}");

        //Don't display if not enough time has passed
        if (lapsed < Settings.MinDisplayTime)
            return;

        player.SendMessage($"Granted {xp:N0} xp and {lootQty:0.00} WCID {tier.LootWcid} after {lapsed.GetFriendlyString()} offline.");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.PlayerEnterWorld))]
    public static void PostPlayerEnterWorld(Player __instance)
    {
        //Only give rewards for completed towers
        var actionChain = new ActionChain();
        actionChain.AddDelaySeconds(7);
        actionChain.AddAction(__instance, () => __instance.GiveOfflineProgress());
        actionChain.EnqueueChain();
    }

    //Create a command to toggle the variable
    static bool OfflineProgressEnabled(this Player player) => player.GetProperty(ProgressOffline) ?? false;
    [CommandHandler("offline-toggle", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleToggle(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player is null) return;

        //Get the current property, defaulting to false if absent
        var toggle = player.OfflineProgressEnabled();

        //Toggle the prop
        toggle = !toggle;

        //Set it to the opposite and inform the player
        player.SetProperty(ProgressOffline, toggle);
        player.SendMessage($"Offline progress is {(toggle ? "disabled" : "enabled")}.");
    }

    [CommandHandler("offline-tiers", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleOfflineRewards(Session session, params string[] parameters)
    {
        var player = session.Player;

        if (!Settings.Enabled)
        {
            player.SendMessage($"Offline progress is disabled.");
            return;
        }

        var sb = new StringBuilder();
        sb.Append($"\nMax offline time rewarded is: {Settings.MaxTime.GetFriendlyString()}\n\n=====Tiers=====\n");
        foreach (var tier in Settings.RewardTiers)
            sb.Append($"{tier.Key,-4}{tier.Value.XpPerHour:N0} xp/hr, {tier.Value.LootPerHour:0.0} @ {tier.Value.LootWcid} loot/hr\n");

        player.SendMessage($"{sb}");
    }

    [CommandHandler("offline-test", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld)]
    public static void HandleOfflineRewardsTest(Session session, params string[] parameters)
    {
        var player = session.Player;

        var secondsPrior = TimeSpan.FromHours((double)ThreadSafeRandom.Next(.5f, 4f)).TotalSeconds;
        player.LogoffTimestamp = Time.GetUnixTime() - secondsPrior;
        player.GiveOfflineProgress();
    }
}
