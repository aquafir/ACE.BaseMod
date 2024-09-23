[CommandCategory(nameof(AlternateLeveling))]
public class AlternateLevelingCommands
{
    const int spacing = -20;
    [CommandHandler("levels", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleLevels(Session session, params string[] parameters)
    {
        var player = session.Player;

        var sb = new StringBuilder();

        sb.Append($"\n====================Attributes====================");
        sb.Append($"\n{"Level",spacing:N0}{"Next",spacing:N0}{"Total",spacing:N0}{"Name",spacing}");
        foreach (var attr in Enum.GetValues<PropertyAttribute>().OrderBy(x => x.ToString()))
        {
            //Skip skills you can't level?
            if (!player.TryGetAttributeCost(attr, out var cost))
                continue;
            var total = player.GetCost(attr);
            var level = player.GetLevel(attr);
            sb.Append($"\n{attr,spacing}{level,spacing}{cost,spacing}{total,spacing}");
        }

        sb.Append($"\n\n====================Vitals====================");
        sb.Append($"\n{"Level",spacing:N0}{"Next",spacing:N0}{"Total",spacing:N0}{"Name",spacing}");
        foreach (var attr in Enum.GetValues<PropertyAttribute2nd>().OrderBy(x => x.ToString()))
        {
            //Skip skills you can't level?
            if (!attr.ToString().StartsWith("Max") || !player.TryGetVitalCost(attr, out var cost))
                continue;
            var total = player.GetCost(attr);
            var level = player.GetLevel(attr);
            sb.Append($"\n{attr,spacing}{level,spacing}{cost,spacing}{total,spacing}");
        }

        sb.Append($"\n\n====================Skills====================");
        sb.Append($"\n{"Level",spacing:N0}{"Next",spacing:N0}{"Total",spacing:N0}{"Name",spacing}");
        foreach (var attr in Enum.GetValues<Skill>().OrderBy(x => x.ToString()))
        {
            //Skip skills you can't level?
            if (!player.TryGetSkillCost(attr, out var cost))
                continue;
            var total = player.GetCost(attr);
            var level = player.GetLevel(attr);
            sb.Append($"\n{attr,spacing}{level,spacing}{cost,spacing}{total,spacing}");
        }

        player.SendMessage(sb.ToString());
    }

    [CommandHandler("refund", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleRefund(Session session, params string[] parameters)
    {
        var player = session.Player;

        long refund = 0;

        var sb = new StringBuilder();
        foreach (var attr in Enum.GetValues<PropertyAttribute>().OrderBy(x => x.ToString()))
        {
            var total = player.GetCost(attr);
            if (total <= 0)
                continue;

            var level = player.GetLevel(attr);
            player.SetLevel(attr, 0);
            player.SetCost(attr, 0);
            player.SendUpdated(attr);
            refund += total;

            sb.Append($"\nRefund {level} levels of {attr} for {total:N0}");
        }

        foreach (var attr in Enum.GetValues<PropertyAttribute2nd>().OrderBy(x => x.ToString()))
        {
            var total = player.GetCost(attr);
            if (total <= 0)
                continue;

            var level = player.GetLevel(attr);
            player.SetLevel(attr, 0);
            player.SetCost(attr, 0);
            player.SendUpdated(attr);
            refund += total;

            sb.Append($"\nRefund {level} levels of {attr} for {total:N0}");
        }

        foreach (var attr in Enum.GetValues<Skill>().OrderBy(x => x.ToString()))
        {
            var total = player.GetCost(attr);
            if (total <= 0)
                continue;

            var level = player.GetLevel(attr);
            player.SetLevel(attr, 0);
            player.SetCost(attr, 0);
            player.SendUpdated(attr);
            refund += total;

            sb.Append($"\nRefund {level} levels of {attr} for {total:N0}");
        }

        if (refund == 0)
        {
            player.SendMessage($"Nothing to refund.");
            return;
        }

        player.AvailableExperience += refund;
        player.SendMessage(sb.ToString());
    }
}