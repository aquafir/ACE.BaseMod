using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower;
public static class FloorExtensions
{
    private static Dictionary<ushort, TowerFloor> floorLookup = null;
    /// <summary>
    /// Called on plugin startup to populate a lookup for floors of the Tower
    /// </summary>
    public static void Init()
    {
        floorLookup = PatchClass.Settings.Floors.ToDictionary(x => x.Landblock, x => x);
    }
    /// <summary>
    /// Try to find the floor a player is on
    /// </summary>
    public static bool TryGetFloor(this Player player, out TowerFloor floor) =>
        floorLookup.TryGetValue(player.CurrentLandblock.Id.Landblock, out floor);

    public static float GetXpBonus(this Player player)
    {
        //Todo: decide about setting on portal
        if (!player.TryGetFloor(out var floor))
            return 1f;

        var delta = floor.Level - (player.Level ?? 1);
        if (delta == 0)
            return 1f;

        var fractionMaxBonus = delta / PatchClass.Settings.MaxXpBonusLevelRange;
        var bonus = PatchClass.Settings.MaxXpBonus * fractionMaxBonus;
        //player.SendMessage($"{delta} levels below target of {floor.Level} - {fractionMaxBonus} of max");
        return Math.Clamp(bonus, 1, PatchClass.Settings.MaxXpBonus);
    }

    public static float GetLootBonus(this Player player)
    {
        //Todo: decide about setting on portal
        if (!player.TryGetFloor(out var floor))
            return 1f;

        var delta = floor.Level - (player.Level ?? 1);
        if (delta == 0)
            return 1f;

        var fractionMaxBonus = delta / PatchClass.Settings.MaxLootBonusLevelRange;
        var bonus = PatchClass.Settings.MaxLootBonus * fractionMaxBonus;
        //player.SendMessage($"{delta} levels below target of {floor.Level} - {fractionMaxBonus} of max");
        return Math.Clamp(bonus, 1, PatchClass.Settings.MaxLootBonus);
    }


    [CommandHandler("bonus", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld)]
    public static void HandleBonus(Session session, params string[] parameters)
    {
        var player = session.Player;

        var floor = PatchClass.Settings.Floors.Where(x => x.Landblock == player.CurrentLandblock.Id.Landblock).FirstOrDefault();

        var lb = player.CurrentLandblock;

        if (floor is null)
            player.SendMessage($"Not in tower");
        else
            player.SendMessage($"{floor.Name} - Target level {floor.Level}\nXp Bonus: {player.GetXpBonus():P2}\nLoot Bonus: {player.GetLootBonus():P2}");
    }
}
