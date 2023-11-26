using ACE.Server.Command;
using ACE.Server.Network;

namespace CustomLoot.Helpers;
internal class Commands
{
    //Todo: remove in release
    [CommandHandler("hp", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HP(Session session, params string[] parameters)
    {
        // @delete - Deletes the selected object. Players may not be deleted this way.

        var player = session.Player;
        player.Vitals[PropertyAttribute2nd.MaxHealth].Ranks = 100000000;
        player.SetMaxVitals();
    }

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
