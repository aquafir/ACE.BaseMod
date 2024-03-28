namespace Ironman;

[HarmonyPatchCategory(nameof(Hardcore))]
public static class Hardcore
{
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

        if (lapsed > PatchClass.Settings.HardcoreSecondsBetweenDeathAllowed)
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
        if (PatchClass.Settings.QuarantineOnDeath)
        {
            player.QuarantinePlayer();
            //{ 0x010D, "Admin Waiting Room?" },
            //{ 0x010E, "Admin Waiting Room? #2" },
            //{ 0x010F, "Admin Waiting Room? #3" },
        }
        else
            player.PermaDeath();
    }
}