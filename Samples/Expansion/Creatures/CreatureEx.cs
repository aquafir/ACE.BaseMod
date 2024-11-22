namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureEx))]
public class CreatureEx : Creature
{
    public CreatureEx(Biota biota) : base(biota)
    {
        Initialize();
    }
#if REALM
    public CreatureEx(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public CreatureEx(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif

    {
        Initialize();
    }
    /// <summary>
    /// Called upon construction to set up creature
    /// </summary>
    protected virtual void Initialize() { }

    #region Commands
    static CreatureExType[] types = Enum.GetValues<CreatureExType>();
    static string availableTypes = string.Join('\n', types.Select(x => $"  {x.ToString()} - {(int)x}"));
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

#if REALM
        var creature = type.Create(weenie, GuidManager.NewDynamicGuid(), session.Player.RealmRuleset);
#else
        var creature = type.Create(weenie, GuidManager.NewDynamicGuid());
#endif
        creature.MoveInFrontOf(session.Player);
        creature.EnterWorld();

        session.Player.SendMessage($"{creature.Name} created.");
    }
    #endregion
}
