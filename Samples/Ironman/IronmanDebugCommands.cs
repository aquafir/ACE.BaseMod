using ACE.Server.Command.Handlers;

namespace Ironman;

public static class IronmanDebugCommands
{
    [CommandHandler("clean", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld)]
    public static void HandleClean(Session session, params string[] parameters)
    {
        session.Player.WipeInventory();
    }

    [CommandHandler("ti", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld)]
    public static void HandleTI(Session session, params string[] parameters)
    {
        var player = session.Player;

        var obj = CommandHandlerHelper.GetLastAppraisedObject(session);
        if (player is null || obj == null) return;

        if (obj.GetProperty(FakeBool.Ironman) != true)
        {
            obj.SetProperty(FakeBool.Ironman, true);
            player.SendMessage($"{obj.Name} is now Iron");
        }
        else
        {
            obj.SetProperty(FakeBool.Ironman, false);
            player.SendMessage($"{obj.Name} is no longer Iron");
        }
    }

    [CommandHandler("t1", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld)]
    public static void HandleT1(Session session, params string[] parameters)
    {
        var player = session.Player;
        //if (player is null || player.GetProperty(FakeBool.Ironman) != true)
        //    return;

        //player.QuarantinePlayer();

        player.RollIronmanAttributes();

        //player.SendMessage($"\n{player.GetProperty(FakeString.IronmanPlan)}\n\n{player.GetProperty(FakeString.IronmanFullPlan)}");
    }

    [CommandHandler("t2", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld)]
    public static void HandleT2(Session session, params string[] parameters)
    {
        var player = session.Player;
        player.RollIronmanSkills();
    }

    [CommandHandler("t3", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld)]
    public static void HandleT3(Session session, params string[] parameters)
    {
        var player = session.Player;

        var t = $"{player.HeritageGroup} - {player.Gender} - {player.EyesPaletteDID}-->";
        player.RollIronmanAppearance();
        t += $"\n{player.HeritageGroup} - {player.Gender} - {player.EyesPaletteDID}";
        player.SendMessage(t);
    }

    [CommandHandler("t4", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld)]
    public static void HandleT4(Session session, params string[] parameters)
    {
        var player = session.Player;
        player.InitializeIronman();
    }

    [CommandHandler("t5", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld)]
    public static void HandleT5(Session session, params string[] parameters)
    {
        var player = session.Player;
        player.QuarantinePlayer("0x010D0100 [-1.705717 1.346221 0.005000] 1.000000 0.000000 0.000000 0.000000");
    }

    //0x010D0100
    [CommandHandler("t6", AccessLevel.Sentinel, CommandHandlerFlag.RequiresWorld)]
    public static void HandleT6(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player is null) return;

        //Check that the players in quarantine
        var lb = player.Location.LandblockId.Raw;
        if (lb != 0x010D0100) return;

        player.InitializeIronman();
        //Warp above Arwic LS
        //0xC6A9001C [80.191460 80.959000 61.028255] -0.148686 0.000000 0.000000 -0.988884

        //Warp to Holt
        if (PatchClass.Settings.StartingLocation.TryParsePosition(out var pos))
            player.Teleport(pos);

        player.SendMessage($"In quarantine, rerolling");
    }

}