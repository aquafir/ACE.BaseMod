using AutoLoot.Loot;

namespace AutoLoot.Helpers;
internal class Commands
{
    //Dump tests here
    [CommandHandler("t1", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleTest(Session session, params string[] parameters)
    {
        var player = session.Player;

        var targetID = player.CurrentAppraisalTarget ?? 0;
        var item = player.FindObject(targetID, Player.SearchLocations.Everywhere, out _, out _, out _);
        if (item is null)
            return;

        //Get some profile
        var profile = Profile.SampleProfile;

        //Set up whatever needs to (Regexs, etc.)
        profile.Initialize();

        //Match an item
        Rule match = null;
        var action = Profile.SampleProfile.Evaluate(item, ref match);

        if (action == Loot.Action.None)
            player.SendMessage($"{item.Name} did not match the profile.");
        else
            player.SendMessage($"{item.Name}: {action} @ {match.Name}");
    }

    //Dump tests here
    [CommandHandler("t2", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleTest2(Session session, params string[] parameters)
    {
        var player = session.Player;

        var targetID = player.CurrentAppraisalTarget ?? 0;
        var item = player.FindObject(targetID, Player.SearchLocations.Everywhere, out _, out _, out _);
        if (item is null)
            return;

        var sb = new StringBuilder("\n");
        var watch = Stopwatch.StartNew();
        var numProfiles = 500;

        //Get some profile
        var profile = RandomHelper.RandomProfile(numProfiles);
        profile.Initialize();
        var time = watch.ElapsedMilliseconds;
        sb.Append($"\nGenerated {numProfiles} in {time}ms");
        watch.Restart();

        //Match an item
        Rule match = null;
        var action = profile.Evaluate(item, ref match);
        time = watch.ElapsedMilliseconds;
        sb.Append($"\nEvaluated in {time}ms");

        if (action == Loot.Action.None)
            sb.Append($"\n{item.Name} did not match the profile.");
        else
            sb.Append($"{item.Name}: {action} @ {match.Name}");

        player.SendMessage(sb.ToString());
    }
    [CommandHandler("t3", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleTest3(Session session, params string[] parameters)
    {
        var player = session.Player;

        var targetID = player.CurrentAppraisalTarget ?? 0;
        var item = player.FindObject(targetID, Player.SearchLocations.Everywhere, out _, out _, out _);
        if (item is null)
            return;

        var sb = new StringBuilder("\n");
        var watch = Stopwatch.StartNew();
        var numProfiles = 500;

        //Get some profile
        var profile = RandomHelper.RandomProfile(numProfiles, 1, true, false);
        profile.Initialize();
        var time = watch.ElapsedMilliseconds;
        sb.Append($"\nGenerated {numProfiles} in {time}ms");
        watch.Restart();

        //Match an item
        Rule match = null;
        var action = profile.Evaluate(item, ref match);
        time = watch.ElapsedMilliseconds;
        sb.Append($"\nEvaluated in {time}ms");

        if (action == Loot.Action.None)
            sb.Append($"\n{item.Name} did not match the profile.");
        else
            sb.Append($"{item.Name}: {action} @ {match.Name}");

        player.SendMessage(sb.ToString());
    }

    [CommandHandler("t4", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleTest4(Session session, params string[] parameters)
    {
        var player = session.Player;

        var targetID = player.CurrentAppraisalTarget ?? 0;
        var item = player.FindObject(targetID, Player.SearchLocations.Everywhere, out _, out _, out _);
        if (item is null)
            return;

        var sb = new StringBuilder("\n");
        var watch = Stopwatch.StartNew();
        var numProfiles = 500;

        //Get some profile
        var profile = RandomHelper.RandomProfile(numProfiles, 1, false);
        profile.Initialize();
        var time = watch.ElapsedMilliseconds;
        sb.Append($"\nGenerated {numProfiles} in {time}ms");
        watch.Restart();

        //Match an item
        Rule match = null;
        var action = profile.Evaluate(item, ref match);
        time = watch.ElapsedMilliseconds;
        sb.Append($"\nEvaluated in {time}ms");

        if (action == Loot.Action.None)
            sb.Append($"\n{item.Name} did not match the profile.");
        else
            sb.Append($"{item.Name}: {action} @ {match.Name}");

        player.SendMessage(sb.ToString());
    }

    //Get rid of items if you need to
    [CommandHandler("clean", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void Clean(Session session, params string[] parameters)
    {
        // @delete - Deletes the selected object. Players may not be deleted this way.

        var player = session.Player;

        foreach (var item in player.Inventory)
        {
            player.TryRemoveFromInventoryWithNetworking(item.Key, out var i, Player.RemoveFromInventoryAction.None);
        }
    }
}
