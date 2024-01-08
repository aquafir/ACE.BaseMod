using ACE.Entity;
using ACE.Server.Command;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages.Messages;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.Query.Internal.NavigationExpandingExpressionVisitor;

namespace CustomLoot.Helpers;
public static class Commands
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
        var player = session.Player;

        try
        {
            foreach (var item in player.Inventory.Values)
            {
                //player.TryRemoveFromInventoryWithNetworking(item.Key, out var i, Player.RemoveFromInventoryAction.None);
                //player.Session.Network.EnqueueSend(new GameMessageInventoryRemoveObject(i));
                player.DeleteItem(item);
            }

            foreach (var item in player.EquippedObjects.Values)
            {
                //player.TryRemoveFromInventoryWithNetworking(item.Key, out var i, Player.RemoveFromInventoryAction.None);
                //player.Session.Network.EnqueueSend(new GameMessageInventoryRemoveObject(i));
                player.DeleteItem(item);
            }
        }catch(Exception ex)
        {
            ModManager.Log($"{ex.Message}", ModManager.LogLevel.Error);
        }

    }


    public static void DeleteItem(this Player player, WorldObject wo)
    {
        wo.DeleteObject(player);
        player.Session.Network.EnqueueSend(new GameMessageDeleteObject(wo));

        //PlayerManager.BroadcastToAuditChannel(session.Player, $"{session.Player.Name} has deleted 0x{wo.Guid}:{wo.Name}");
    }
}
