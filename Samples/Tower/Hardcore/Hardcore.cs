namespace Tower.Hardcore;

[CommandCategory(nameof(Feature.Hardcore))]
[HarmonyPatchCategory(nameof(Feature.Hardcore))]
public static class Hardcore
{
    static HardcoreSettings Settings = PatchClass.Settings.Hardcore;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.OnDeath), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
    public static void PostOnDeath(DamageHistoryInfo lastDamager, DamageType damageType, bool criticalHit, ref Player __instance, ref DeathMessage __result)
    {
        //Ignore non-hardcore
        if (__instance is not Player player)
            return;

        if (player.GetProperty(FakeBool.Hardcore) != true)
            return;

        //Check death interval
        var current = Time.GetUnixTime();
        var lastDeath = player.GetProperty(FakeFloat.TimestampLastPlayerDeath) ?? current;
        var lapsed = current - lastDeath;
        player.SetProperty(FakeFloat.TimestampLastPlayerDeath, Time.GetUnixTime());

        if (lapsed > Settings.SecondsBetweenDeathAllowed)
        {
            player.SendMessage($"You died after {lapsed / 3600:0.0} hours and are given a free death.");
            return;
        }

        var lives = player.GetProperty(FakeInt.HardcoreLives) ?? 0;
        lives--;
        if (lives >= 0)
        {
            player.SendMessage($"You have {lives} lives remaining.");
            player.SetProperty(FakeInt.HardcoreLives, lives);
            return;
        }

        //Handle perma-death
        PlayerManager.BroadcastToChannelFromConsole(Channel.Advocate1, $"{player?.Name} has met an untimely demise at the hands of {lastDamager?.Name ?? ""}!");
        if (Settings.QuarantineOnDeath)
        {
            player.QuarantinePlayer();
            //{ 0x010D, "Admin Waiting Room?" },
            //{ 0x010E, "Admin Waiting Room? #2" },
            //{ 0x010F, "Admin Waiting Room? #3" },
        }
        else
            player.PermaDeath();
    }

    public static bool IsHardcore(this Player player) => player.GetProperty(FakeBool.Hardcore) == true;
    public static int Lives(this Player player) => player.GetProperty(FakeInt.HardcoreLives) ?? 0;

    /// <summary>
    /// Add Hardcore property and related mode setup
    /// </summary>
    public static void ApplyHardcore(this Player player)
    {
        if (player is null)
            return;

        //Make it fancy
        for (var i = 0; i < 20; i++)
            player.PlayParticleEffect(Enum.GetValues<PlayScript>().Random(), player.Guid, i * .02f);

        player.SetProperty(FakeInt.HardcoreLives, Settings.HardcoreStartingLives);
        player.SetProperty(FakeFloat.TimestampLastPlayerDeath, Time.GetUnixTime());
        player.SetProperty(FakeBool.Hardcore, true);
        player.SendMessage($"\nYou are now hardcore with {Settings.HardcoreStartingLives} remaining.");
    }

    [CommandHandler("hc", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HP(Session session, params string[] parameters)
    {
        var player = session.Player;

        if(player.IsHardcore())
        {
            player.SendMessage($"You have {player.Lives()} lives remaining.");
        }
        else if(player.Level <= Settings.MaxLevel)
            player.ApplyHardcore();
        else 
            player.SendMessage($"Only players before level {Settings.MaxLevel} may become hardcore.");
    }
}