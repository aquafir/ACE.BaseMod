namespace Tower;

[CommandCategory(nameof(Feature.MarketplaceOverride))]
[HarmonyPatchCategory(nameof(Feature.MarketplaceOverride))]
public class Commands
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionTeleToMarketPlace))]
    public static bool PreHandleActionTeleToMarketPlace(ref Player __instance)
    {
        __instance?.SendMessage($"This command has been disabled.");

        return false;
    }

    //[CommandHandler("mp", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    //public static void Handle(Session session, params string[] parameters)
    //{
    //    Debugger.Break();
    //}

    //internal static void Init()
    //{
    //    //Not actually handled this way...
    //    //if(PatchClass.Settings.Features.Contains(Feature.MarketplaceOverride))
    //    //    CommandManager.TryAddCommand(Handle, "mp", AccessLevel.Player, CommandHandlerFlag.RequiresWorld);
    //}
}
