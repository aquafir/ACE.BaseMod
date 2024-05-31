using ACE.Server.Command;
using ACE.Server.Managers;

namespace Expansion.Features;

[HarmonyPatchCategory(nameof(Feature.TimeInGame))]
public static class TimeInGame
{
    public static double PreviousTimeInGame(this Player player) => player.GetProperty(FakeFloat.TimeInGame) ?? 0;
    public static double TimeInGameFromLogin(this Player player) => Time.GetUnixTime() - (player.GetProperty(PropertyFloat.LoginTimestamp) ?? 0);
    public static double TotalTimeInGame(this Player player) => player.PreviousTimeInGame() + player.TimeInGameFromLogin();          

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.LogOut_Inner), new Type[] { typeof(bool) })]
    public static void PostLogOut_Inner(bool clientSessionTerminatedAbruptly, ref Player __instance)
    {
        //When a player logs off add the amount of time they spent in game to the total 
        //Use logoff with pk timer?  -- LogoffTimestamp
        __instance.SetProperty(FakeFloat.TimeInGame, __instance.TotalTimeInGame());
    }

    [CommandHandler("tig", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleTimeInGame(Session session, params string[] parameters)
    {
        var player = session.Player;

        var prev = player.PreviousTimeInGame();
        var tot = player.TotalTimeInGame();
        player.SendMessage($"Previous time: {prev}\nTotal time: {tot}");
    }
}