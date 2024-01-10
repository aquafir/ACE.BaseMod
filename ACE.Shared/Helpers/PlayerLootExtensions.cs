using ACE.Server.Command.Handlers;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Shared.Helpers;

public static class PlayerLootExtensions
{
    /// <summary>
    /// Create items based on the Admin /ci command
    /// </summary>
    public static void CreateItems(this Player player, string command)//Session session, params string[] parameters)
    {
        if (player is null)
            return;
        var parameters = command.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var session = player.Session;
        
        var weenie = AdminCommands.GetWeenieForCreate(session, parameters[0], true);

        if (weenie == null)
            return;

        ushort stackSize = 0;
        int? palette = null;
        float? shade = null;

        //Parse stacksize if available
        if (parameters.Length > 1 && !ushort.TryParse(parameters[1], out stackSize) || stackSize == 0)
                return;

        //Parse palette
        if (parameters.Length > 2)
        {
            if (!int.TryParse(parameters[2], out int _palette))
                return;
            else
                palette = _palette;
        }

        //Parse shade
        if (parameters.Length > 3)
        {
            if (!float.TryParse(parameters[3], out float _shade))
                return;
            else
                shade = _shade;
        }

        var obj = AdminCommands.CreateObjectForCommand(session, weenie);

        if (obj == null)
            return;

        if (stackSize != 0 && obj.MaxStackSize != null)
        {
            stackSize = Math.Min(stackSize, (ushort)obj.MaxStackSize);

            obj.SetStackSize(stackSize);
        }

        if (palette != null)
            obj.PaletteTemplate = palette;

        if (shade != null)
            obj.Shade = shade;

        session.Player.TryCreateInInventoryWithNetworking(obj);

        //PlayerManager.BroadcastToAuditChannel(session.Player, $"{session.Player.Name} has created {obj.Name} (0x{obj.Guid:X8}) in their inventory.");
    }
}