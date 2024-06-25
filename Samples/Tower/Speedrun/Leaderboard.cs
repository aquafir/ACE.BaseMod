namespace Tower.Speedrun;

[CommandCategory(nameof(Feature.Speedrun))]
[HarmonyPatchCategory(nameof(Feature.Speedrun))]
public static class Leaderboard
{
    static DateTime timestampLeaderboard = DateTime.MinValue;
    static string lastLeaderboard = "";
    static TimeSpan cacheInterval = TimeSpan.FromSeconds(60);

    static string Commands => string.Join(", ", Enum.GetNames<Leaderboards>());


    enum Leaderboards
    {
        Level,
        Speed,
        Floor,
    }

    [CommandHandler("leaderboard", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleLeaderboard(Session session, params string[] parameters)
    {
        var lapse = DateTime.Now - timestampLeaderboard;
        if (lapse < cacheInterval)
        {
            session.Player.SendMessage($"{lastLeaderboard}");
            return;
        }

        var player = session.Player;

        var sb = new StringBuilder();
        var players = PlayerManager.GetAllPlayers().OrderByDescending(x => x.Level).Take(10);
        var rank = 1;
        foreach (var p in players)
        {
            if (p is not null)
                sb.Append($"\n{rank++}) {p.Name,-30}{p.Level}");
        }

        if (!players.Any(x => x.Guid == player.Guid))
            sb.Append($"\n   {player.Name,-30}{player.Level}");

        timestampLeaderboard = DateTime.Now;
        lastLeaderboard = sb.ToString();

        session.Player.SendMessage($"{sb}");
    }
}
