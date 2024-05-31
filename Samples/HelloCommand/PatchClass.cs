namespace HelloCommand;

[HarmonyPatch]
public class PatchClass
{
    private const string HELLO_COMMAND = "hello";
    private const string BYE_COMMAND = "bye";

    #region Start/Shutdown
    public static void Start()
    {
        //Create a command with an attribute and delegate

        var attribute = new CommandHandlerAttribute(HELLO_COMMAND, AccessLevel.Admin, CommandHandlerFlag.None, 0, "Says hello", "No parameters");
        CommandHandler handler = (s, p) =>
        {
            Console.WriteLine("Hello, world!");
            s?.Player?.SendMessage($"Hello, {s.Player.Name}");
        };
        var info = new CommandHandlerInfo()
        {
            Attribute = attribute,
            Handler = handler
        };
        if (CommandManager.TryAddCommand(info))
        {
#if REALM
            ModManager.Log($"Created command: {info.Attribute.Command}");
#else
    ModManager.Log($"Non-realm command: {info.Attribute.Command}");
#endif
        }
        if (!CommandManager.TryAddCommand(info, false))
        {
            //If you don't ask a command to be overridden it will fail if that command already exists
        }

        //If Meta.json has RegisterCommands set to true, ACE will attempt to load/unload automatically
        //Mod.Container.RegisterCommandHandlers(true);
    }

    //ACE-style command using attribute
    [CommandHandler(BYE_COMMAND, AccessLevel.Player, CommandHandlerFlag.None, -1, "Manage mods the lazy way")]
    public static void HandleListMods(Session session, params string[] parameters)
    {
        Console.WriteLine("Bye.");
        session?.LogOffPlayer(true);
    }

    public static void Shutdown()
    {
        //This was added manually so it has to be manually removed
        CommandManager.TryRemoveCommand(HELLO_COMMAND);

        //Commands with an attribute can be automatically unregistered
        //Mod.Container.UnregisterCommandHandlers();      //Commands added 
    }
#endregion
}