namespace Tower.Speedrun;

[CommandCategory(nameof(Feature.SpeedRun))]
[HarmonyPatchCategory(nameof(Feature.SpeedRun))]
public class Leaderboard
{
    static DateTime timestampLeaderboard = DateTime.MinValue;
    static string lastLeaderboard = "";
    static TimeSpan cacheInterval = TimeSpan.FromSeconds(60);

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


    [CommandHandler("dlb", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleDLB(Session session, params string[] parameters)
    {
        //How it was supposed to work
        //CrossModInteraction.PatchClass.DoStatefulWorkInAnotherMod();

        //...or get the ModContainer -> IHarmonyMod -> as the type of the desired mod -> do things
        var mod = ModManager.Mods.Where(x => x.FolderName == "CrossModInteraction").FirstOrDefault()?.Instance;
        if (mod is null)
            return;

        //These fail, with the latter giving an InvalidCastException
        //var typed = instance as CrossModInteraction.Mod;
        //var t = (CrossModInteraction.Mod)instance;

        //Horrible hacky way works
        dynamic pc = mod;
        ModManager.Log(pc.Patch.State);

        //Works
        var discord = ModManager.Mods.Where(x => x.FolderName == "Discord").FirstOrDefault()?.Instance;
        if (discord is null)
            return;

        dynamic relay = discord;
        relay = relay.Patch.Relay;
        relay.QueueMessageForDiscord("Test?");
    }

}
