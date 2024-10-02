
namespace ACE.BaseMod;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnStartSuccess()
    {
        //Once the Mod has loaded do some things...
    }

    public override async Task OnWorldOpen()
    {
        //Once the server has fully started do some things...
        Settings = SettingsContainer.Settings;
    }
}

