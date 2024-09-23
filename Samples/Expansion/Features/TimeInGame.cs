namespace Expansion.Features;

[CommandCategory(nameof(Feature.TimeInGame))]
[HarmonyPatchCategory(nameof(Feature.TimeInGame))]
public static class TimeInGame
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.LogOut_Inner), new Type[] { typeof(bool) })]
    public static void PostLogOut_Inner(bool clientSessionTerminatedAbruptly, ref Player __instance)
    {
        //When a player logs off add the amount of time they spent in game to the total 
        __instance.SetProperty(FakeFloat.TimeInGame, __instance.TotalTimeInGame());

        //Use logoff with pk timer?  -- LogoffTimestamp
        //Use player.Age when TimeInGame missing?
    }

    //[CommandHandler("tig", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    //public static void HandleTimeInGame(Session session, params string[] parameters)
    //{
    //    var player = session.Player;

    //    var prev = player.PreviousTimeInGame();
    //    var tot = player.TotalTimeInGame();
    //    player.SendMessage($"Previous time: {prev}\nTotal time: {tot}");
    //}
}