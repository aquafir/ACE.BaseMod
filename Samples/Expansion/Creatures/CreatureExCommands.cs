using ACE.Server.Command.Handlers;
using ACE.Server.Command;
using ACE.Server.Managers;
using ACE.Server.Network;
using Microsoft.VisualBasic;

namespace Expansion.Creatures;

public static class CreatureExCommands
{
    static Expansion.Creatures.CreatureExType[] types = Enum.GetValues<Expansion.Creatures.CreatureExType>();
    static string availableTypes = String.Join('\n', types.Select(x => $"  {x.ToString()} - {(int)x}"));
    [CommandHandler("cex", AccessLevel.Developer, CommandHandlerFlag.RequiresWorld)]
    public static void HandleCreateEx(Session session, params string[] parameters)
    {
        //Check parameters
        if (parameters.Length != 2)
        {
            session.Player.SendMessage($"Available types are: \n{availableTypes}");
            return;
        }

        //Check valid CreatureEx type
        //Todo: decide on allowing partial name matches: types.Any(x => x.ToString().Contains(parameters[0], StringComparison.InvariantCultureIgnoreCase))
        var name = parameters[0];
        if (!Enum.TryParse<CreatureExType>(name, true, out var type))
        {
            if (uint.TryParse(name, out var index))
                type = (CreatureExType)index;

            if (!Enum.IsDefined(type) || type == CreatureExType.Unknown)
            {
                session.Player.SendMessage($"Available types are: \n{availableTypes}");
                return;
            }
        }

        var weenie = AdminCommands.GetWeenieForCreate(session, parameters[1]);
        if (weenie is null)
        {
            session.Player.SendMessage($"Provide a valid weenie ID: {parameters[1]}");
            return;
        }

        var creature = type.Create(weenie, GuidManager.NewDynamicGuid());
        creature.MoveInFrontOf(session.Player);
        creature.EnterWorld();

        session.Player.SendMessage($"{creature.Name} created.");
    }
}