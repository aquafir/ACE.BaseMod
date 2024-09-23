namespace Tower;

[CommandCategory(nameof(Feature.FloorBonus))]
[HarmonyPatchCategory(nameof(Feature.FloorBonus))]
public static class FloorBonus
{
    static FloorSettings Settings => PatchClass.Settings.Tower;

    public static float GetXpBonus(this Player player)
    {
        //Todo: decide about setting on portal
        if (!player.TryGetFloor(out var floor))
            return 1f;

        var delta = floor.Level - (player.Level ?? 1);
        if (delta == 0)
            return 1f;

        var fractionMaxBonus = delta / Settings.MaxXpBonusLevelRange;
        var bonus = Settings.MaxXpBonus * fractionMaxBonus;
        //player.SendMessage($"{delta} levels below target of {floor.Level} - {fractionMaxBonus} of max");
        return Math.Clamp(bonus, 1, Settings.MaxXpBonus);
    }

    public static float GetLootBonus(this Player player)
    {
        //Todo: decide about setting on portal
        if (!player.TryGetFloor(out var floor))
            return 1f;

        var delta = floor.Level - (player.Level ?? 1);
        if (delta == 0)
            return 1f;

        var fractionMaxBonus = delta / Settings.MaxLootBonusLevelRange;
        var bonus = Settings.MaxLootBonus * fractionMaxBonus;
        //player.SendMessage($"{delta} levels below target of {floor.Level} - {fractionMaxBonus} of max");
        return Math.Clamp(bonus, 1, Settings.MaxLootBonus);
    }


    [CommandHandler("bonus", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld)]
    public static void HandleBonus(Session session, params string[] parameters)
    {
        var player = session.Player;

        var floor = Settings.Floors.Where(x => x.Landblock == player.CurrentLandblock.Id.Landblock).FirstOrDefault();

        var lb = player.CurrentLandblock;

        if (floor is null)
            player.SendMessage($"Not in tower");
        else
            player.SendMessage($"{floor.Name} - Target level {floor.Level}\nXp Bonus: {player.GetXpBonus():P2}\nLoot Bonus: {player.GetLootBonus():P2}");
    }
}
