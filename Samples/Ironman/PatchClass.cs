
namespace Ironman;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;
        PatchFlaggingCategories();
        PatchRestrictionCategories();
    }

    private void PatchFlaggingCategories()
    {
        foreach (var p in Settings.FlagItemEvents)
            ModC.Harmony.PatchCategory(p);
    }
    private void PatchRestrictionCategories()
    {
        foreach (var p in Settings.Restrictions)
            ModC.Harmony.PatchCategory(p);
    }

    [CommandHandler("di", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleDie(Session session, params string[] parameters)
    {
        session.Player.TakeDamage(null, DamageType.Fire, 1000000);
    }
}
