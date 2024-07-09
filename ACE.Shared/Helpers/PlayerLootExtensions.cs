using ACE.Server.Command.Handlers;

namespace ACE.Shared.Helpers;

public static class PlayerLootExtensions
{
    /// <summary>
    /// Create items based on the Admin /ci command
    /// </summary>
    public static bool TryCreateItems(this Player player, string command)
    {
        if (player is null)
            return false;
        var parameters = command.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var session = player.Session;

        var weenie = AdminCommands.GetWeenieForCreate(session, parameters[0], true);

        if (weenie == null)
            return false;

        uint amount = 1;
        int? palette = null;
        float? shade = null;

        //Try to parse quantity and fail on a failure or invalid quantity
        if (parameters.Length >= 2 && (!uint.TryParse(parameters[1], out amount) || amount < 1))
            return false;

        //Parse palette
        //if (parameters.Length > 2)
        //{
        //    if (!int.TryParse(parameters[2], out int _palette))
        //        return false;
        //    else
        //        palette = _palette;
        //}

        ////Parse shade
        //if (parameters.Length > 3)
        //{
        //    if (!float.TryParse(parameters[3], out float _shade))
        //        return false;
        //    else
        //        shade = _shade;
        //}

        var obj = AdminCommands.CreateObjectForCommand(session, weenie);
        if (obj == null)
            return false;

        //Check burden
        var burden = player.GetAvailableBurden();
        if (amount * (obj.EncumbranceVal ?? 0) > burden)
            return false;

        //Check available spaces
        var freeSpace = player.GetFreeInventorySlots();
        var stackSize = obj.MaxStackSize ?? 1;

        var neededSpace = Math.Ceiling((double)amount / stackSize);
        if (freeSpace < neededSpace)
            return false;

        //While there is still a remaining amount to add keep creating up to the max stack size of the item
        while (amount > 0)
        {
            //Set size and update remaining
            var stack = Math.Min(amount, stackSize);
            obj.SetStackSize((ushort)stack);    //Use uint for large stacks like pyreals and convert to shorts for native support
            amount -= stack;

            //Try to add
            if (!session.Player.TryCreateInInventoryWithNetworking(obj))
            {
                ModManager.Log($"Failed to add remaining {amount} of {obj.Name} to {player.Name}", ModManager.LogLevel.Error);
                return false;
            }

            //Create next WO
            if (amount > 0)
                obj = AdminCommands.CreateObjectForCommand(session, weenie);
        }

        //if (palette != null)
        //    obj.PaletteTemplate = palette;

        //if (shade != null)
        //    obj.Shade = shade;

        return true;
        //PlayerManager.BroadcastToAuditChannel(session.Player, $"{session.Player.Name} has created {obj.Name} (0x{obj.Guid:X8}) in their inventory.");
    }
}