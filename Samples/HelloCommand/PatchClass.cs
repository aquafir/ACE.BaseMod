
namespace HelloCommand;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    private const string HELLO_COMMAND = "hello";
    private const string BYE_COMMAND = "bye";

    public override async Task OnStartSuccess()
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
            ModManager.Log($"Created command: {info.Attribute.Command}");
        }
        if (!CommandManager.TryAddCommand(info, false))
        {
            //If you don't ask a command to be overridden it will fail if that command already exists
        }
    }

    public override void Stop()
    {
        base.Stop();
        CommandManager.TryRemoveCommand(HELLO_COMMAND);
    }

    //ACE-style command using attribute
    [CommandHandler(BYE_COMMAND, AccessLevel.Player, CommandHandlerFlag.None, -1, "Manage mods the lazy way")]
    public static void HandleListMods(Session session, params string[] parameters)
    {
        //If Meta.json has RegisterCommands set to true, ACE will attempt to load/unload automatically
        //Mod.Container.RegisterCommandHandlers(true);
        Console.WriteLine("Bye.");
        session?.LogOffPlayer(true);
    }
}

public class Settings { }