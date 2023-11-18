using ACE.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatFilter;

public static class Helper
{
    public static bool IsShadowBanned(this Player player) => player.GetProperty(PatchClass.Settings.ShadowBanProperty) ?? false;
    public static void SetShadowBanned(this Player player, bool value = true) => player.SetProperty(PatchClass.Settings.ShadowBanProperty, value);


    public static int ChatInfractionCount(this Player player) => player.GetProperty(PatchClass.Settings.ChatInfractionsProperty) ?? 0;
    public static void SetChatInfractionCount(this Player player, int count) => player.SetProperty(PatchClass.Settings.ChatInfractionsProperty, count);
    public static void IncreaseChatInfractionCount(this Player player, int count) => player.SetProperty(PatchClass.Settings.ChatInfractionsProperty, player.ChatInfractionCount() + count);
    public static bool GagPlayer(this Player player)
    {
        if (player == null || !PatchClass.Settings.GagPlayer)
            return false;

        var gagSeconds = PatchClass.Settings.GagBaseTime + PatchClass.Settings.GagTimePerInfraction * player.ChatInfractionCount();
        player.SetProperty(PropertyBool.IsGagged, true);
        player.SetProperty(PropertyFloat.GagTimestamp, Time.GetUnixTime());
        player.SetProperty(PropertyFloat.GagDuration, gagSeconds);

        player.SaveBiotaToDatabase();

        if(PatchClass.Settings.BroadcastGag)
            PlayerManager.BroadcastToAuditChannel(player, $"{player.Name} has been gagged {player.Name} for {gagSeconds/60} minute(s) for their {player.ChatInfractionCount()} infraction.");

        return true;
    }

    public static bool BanPlayerAccount(this Player player)
    {
        if (player == null || !PatchClass.Settings.BanAccount)
            return false;

        var account = player.Account;
        var session = player.Session;

        if (account is null) return false;

        var bannedOn = DateTime.UtcNow;
        var banSeconds = PatchClass.Settings.BanBaseTime + PatchClass.Settings.BanTimePerInfraction * player.ChatInfractionCount();
        var banExpires = DateTime.UtcNow.AddSeconds(banSeconds);

        var bannedBy = 0u;
        if (session != null)
        {
            bannedBy = session.AccountId;
        }

        var reason = $"{player.Name} has been gagged {player.Name} for {banSeconds / 60} minute(s) for their {player.ChatInfractionCount()} infraction.";
        var accountName = account.AccountName;
        account.BannedTime = bannedOn;
        account.BanExpireTime = banExpires;
        account.BannedByAccountId = bannedBy;
        if (!string.IsNullOrWhiteSpace(reason))
            account.BanReason = reason;

        DatabaseManager.Authentication.UpdateAccount(account);

        // Boot the player
        player.Session.LogOffPlayer(true);       

        if (PatchClass.Settings.BroadcastBan)
            PlayerManager.BroadcastToAuditChannel(player, reason);

        return true;
    }

   
}
