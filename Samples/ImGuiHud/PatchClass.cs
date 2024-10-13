namespace ImGuiHud;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;
        if (Settings.AutostartGui)
            await StartGui();
    }

    public override void Stop()
    {
        base.Stop();

        StopGui();
    }

    static SampleOverlay Overlay;
    static async Task StartGui()
    {
        try
        {
            StopGui();
            Overlay = new();
            await Overlay.Run();
        }
        catch (Exception ex)
        {
            ModManager.Log(ex.Message, ModManager.LogLevel.Error);
        }
    }
    static void StopGui()
    {
        try
        {
            Overlay?.Close();
            Overlay?.Dispose();
        }
        catch (Exception ex)
        {
            ModManager.Log(ex.Message, ModManager.LogLevel.Error);
        }
    }

    [CommandHandler("gui", AccessLevel.Admin, CommandHandlerFlag.None, 0)]
    public static void HandleGui(Session session, params string[] parameters)
    {
        Task.Run(async () => StartGui());
    }
}
