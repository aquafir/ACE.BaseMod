namespace ACE.Web;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;
        StartWeb();
    }

    #region Web
    private WebApp app = new();
    private void StartWeb()
    {
        try
        {
            app.Start();
        }
        catch (Exception ex)
        {
            ModManager.Log($"Error starting web app: {ex.Message}", ModManager.LogLevel.Error);
            Mod.State = ModState.Error;
        }
    }

    private void StopWeb()
    {
        try
        {
            app.Stop();
        }
        catch (Exception ex)
        {
            ModManager.Log($"Error shutting down web app: {ex.Message}", ModManager.LogLevel.Error);
            Mod.State = ModState.Error;
        }
    }
    #endregion
}

