using ACE.Database;
using System.Runtime.CompilerServices;

namespace Ironman;

public static class IronmanPlayerCommands
{
    private const string NAME_SUFFIX = "-Im";
    static DateTime timestampLeaderboard = DateTime.MinValue;
    static DateTime timestampGrave = DateTime.MinValue;
    static string lastLeaderboard = "";
    static string lastGrave = "";
    static TimeSpan cacheInterval = TimeSpan.FromSeconds(60);

    private static readonly ConditionalWeakTable<Character, ShardDbContext> CharacterContexts = new ConditionalWeakTable<Character, ShardDbContext>();
    private static List<Character> GetCharacterList()
    {
        var context = new ShardDbContext();

        IQueryable<Character> query;
        query = context.Character.Where(r => r.IsDeleted && r.Name.EndsWith(NAME_SUFFIX));

        var results = query.ToList();

        for (int i = 0; i < results.Count; i++)
        {
            // Do we have a reference to this Character already?
            var existingChar = CharacterContexts.FirstOrDefault(r => r.Key.Id == results[i].Id);

            if (existingChar.Key != null)
                results[i] = existingChar.Key;
            else
            {
                // No reference, pull all the properties and add it to the cache
                //query.Include(r => r.CharacterPropertiesContractRegistry).Load();
                CharacterContexts.Add(results[i], context);
            }
        }

        return results;
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

        var sb = new StringBuilder();
        var players = PlayerManager.GetAllPlayers().Where(x => x.Name.EndsWith(NAME_SUFFIX));
        foreach (var player in players.OrderByDescending(x => x.Level))
        {
            if (player is not null)
                sb.Append($"\n  {player.Level,-8}{player.Name}");
        }

        timestampLeaderboard = DateTime.Now;
        lastLeaderboard = sb.ToString();

        session.Player.SendMessage($"{sb}");
    }

    [CommandHandler("grave", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleGrave(Session session, params string[] parameters)
    {
        var lapse = DateTime.Now - timestampGrave;
        if (lapse < cacheInterval)
        {
            session.Player.SendMessage($"{lastGrave}");
            return;
        }

        var characters = GetCharacterList();
        var sb = new StringBuilder();

        foreach (var account in characters.OrderBy(x => x.IsDeleted).GroupBy(x => x.AccountId))
        {
            var acct = DatabaseManager.Authentication.GetAccountById(account.Key);
            sb.Append($"\n======={acct.AccountName} ({account.Count()})=======");

            foreach (var character in account.Where(x => x.Name.EndsWith(NAME_SUFFIX)))
                sb.Append($"\n  {character.Name} - Dead");
        }

        timestampGrave = DateTime.Now;
        lastGrave = sb.ToString();

        session.Player.SendMessage($"{sb}");
    }

    [CommandHandler("plan", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandlePlan(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player is null || player.GetProperty(FakeBool.Ironman) != true)
            return;

        player.SendMessage($"\n{player.GetProperty(FakeString.IronmanPlan)}\n\n{player.GetProperty(FakeString.IronmanFullPlan)}");
    }

    [CommandHandler("ironman", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleIronman(Session session, params string[] parameters)
    {
        var player = session.Player;

        if (player is null) return;

        if (player.Level > 1)
        {

        }

        if (player.GetProperty(FakeBool.Ironman) != true)
        {
            player.SetProperty(FakeBool.Ironman, true);
            player.SendMessage($"{player.Name} is now Iron");
        }
        else
        {
            player.SetProperty(FakeBool.Ironman, false);
            player.SendMessage($"{player.Name} is no longer Iron");
        }
    }
}
