namespace Tower;

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

        //Check for PK deaths
        if (__instance.IsPKDeath(lastDamager) || __instance.IsPKLiteDeath(lastDamager))
        {
            //if (Settings.StayWhite)
            //{
            //    player.PlayerKillerStatus = PlayerKillerStatus.NPK;
            //    player.PkLevel = PKLevel.NPK;
            //    player.SendMessage($"You have been forced to NPK status.");
            //}

            if (Settings.IgnorePK)
            {
                player.SendMessage($"PK deaths not counted against hardcore players.");
                return;
            }

            if (lastDamager.TryGetPetOwnerOrAttacker() is Player killer)
            {
                var levelDiff = Math.Abs((__instance.Level ?? 0) - (killer.Level ?? 0));
                if (levelDiff > Settings.PkMaxLevelDifference)
                {
                    player.SendMessage($"PK deaths only count against hardcore players if under {Settings.PkMaxLevelDifference}, your death had a level difference of {levelDiff}.");
                    return;
                }
            }
        }

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
            player.QuarantinePlayer(Settings.QuarantineLoc);
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

        foreach (var item in Settings.Items)
            player.TryCreateItems(item);
    }

    [CommandHandler("hc", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HP(Session session, params string[] parameters)
    {
        var player = session.Player;

        if (player.IsHardcore())
        {
            player.SendMessage($"You have {player.Lives()} lives remaining.");
        }
        else if (player.Level <= Settings.MaxLevel)
            player.ApplyHardcore();
        else
            player.SendMessage($"Only players before level {Settings.MaxLevel} may become hardcore.");
    }


    [CommandHandler("t1", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HP2(Session session, params string[] parameters)
    {
        var player = session.Player;

        Debugger.Break();
        var r = "0x02FA0100 -2.282979 0.158116 0.517504 -0.900291 0.000000 0.000000 0.435289".TryParsePosition(out var pos);
    }

}