//using Tower.Floor;

//namespace Tower;

//[CommandCategory(nameof(Feature.Speedrun))]
//[HarmonyPatchCategory(nameof(Feature.Speedrun))]
//public static class Leaderboard
//{
//    static DateTime timestampLeaderboard = DateTime.MinValue;
//    static string lastLeaderboard = "";
//    static TimeSpan cacheInterval = TimeSpan.FromSeconds(60);

//    static string Commands => string.Join(", ", Enum.GetNames<Leaderboards>());
//    static readonly string[] USAGES = new string[] {
//        $@"(?<verb>{Leaderboards.Level})$",
//        //First check amount first cause I suck with regex
//        $@"(?<verb>{Leaderboards.Speed}(?<name>.+)\s+(?<amount>(\*|\d+))$",
//        $@"(?<verb>{Leaderboards.Floor})\s+(?<floor>(\*|\d+))$",
//    };
//    //Join usages in a regex pattern
//    static string Pattern => string.Join("|", USAGES.Select(x => $"({x})"));
//    static Regex CommandRegex = new(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

//    public static bool TryParseCommand(this string[] parameters, out Leaderboards verb, out TowerFloor floor)
//    {
//        //Set defaults
//        verb = 0;
//        floor = null;

//        //Debugger.Break();
//        //Check for valid command
//        var match = CommandRegex.Match(string.Join(" ", parameters));
//        if (!match.Success)
//            return false;

//        //Parse verb
//        if (!Enum.TryParse(match.Groups["verb"].Value, true, out verb))
//            return false;

//        //Set name
//        name = match.Groups["name"].Value;

//        //Set recipient if available
//        recipient = match.Groups["recipient"].Value;

//        //Parse amount if available
//        if (int.TryParse(match.Groups["amount"].Value, out var parsedAmount))
//            amount = parsedAmount;
//        else if (match.Groups["amount"].Value == "*")
//        {
//            amount = int.MaxValue;
//            wildcardAmount = true;
//        }

//        return true;
//    }


//    enum Leaderboards
//    {
//        Unknown = 0,
//        Level,
//        Speed,
//        Floor,
//    }

//    [CommandHandler("leaderboard", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
//    public static void HandleLeaderboard(Session session, params string[] parameters)
//    {
//        var lapse = DateTime.Now - timestampLeaderboard;
//        if (lapse < cacheInterval)
//        {
//            session.Player.SendMessage($"{lastLeaderboard}");
//            return;
//        }

//        var player = session.Player;

//        var sb = new StringBuilder();
//        var players = PlayerManager.GetAllPlayers().OrderByDescending(x => x.Level).Take(10);
//        var rank = 1;
//        foreach (var p in players)
//        {
//            if (p is not null)
//                sb.Append($"\n{rank++}) {p.Name,-30}{p.Level}");
//        }

//        if (!players.Any(x => x.Guid == player.Guid))
//            sb.Append($"\n   {player.Name,-30}{player.Level}");

//        timestampLeaderboard = DateTime.Now;
//        lastLeaderboard = sb.ToString();

//        session.Player.SendMessage($"{sb}");
//    }
//}


////static readonly string[] USAGES = new string[] {
////        $@"(?<verb>{Leaderboards.List})$",
////        //First check amount first cause I suck with regex
////        $@"(?<verb>{Leaderboards.Give}|{Leaderboards.Take}) (?<name>.+)\s+(?<amount>(\*|\d+))$",
////        $@"(?<verb>{Leaderboards.Give}|{Leaderboards.Take}) (?<name>.+)$",
////        // /cash doesn't have named item
////        $@"(?<verb>{Leaderboards.Give})$",
////        $@"(?<verb>{Leaderboards.Send}) (?<recipient>.+) (?<name>.+)\s+(?<amount>(\*|\d+))$",
////        $@"(?<verb>{Leaderboards.Send}) (?<recipient>.+) (?<name>.+)$",
////    };
//////Join usages in a regex pattern
////static string Pattern => string.Join("|", USAGES.Select(x => $"({x})"));
////static Regex CommandRegex = new(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);