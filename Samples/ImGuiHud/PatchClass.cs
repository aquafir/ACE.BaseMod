namespace ImGuiHud;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnStartSuccess()
    {
        if (Settings.AutostartGui)
            StartGui();
    }

    public override void Stop()
    {
        base.Stop();

        StopGui();
    }

    static SimpleOverlay Overlay;
    static async void StartGui()
    {
        try
        {
            StopGui();
            Overlay = new();
            Overlay.Run();
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
    private static async Task<SimpleOverlay> StartOverlay()
    {
        Overlay = new();
        await Overlay.Run();
        return Overlay;
    }

    [CommandHandler("gui", AccessLevel.Admin, CommandHandlerFlag.None, 0)]
    public static void HandleGui(Session session, params string[] parameters)
    {
        Task.Run(async () => StartGui());
    }
}
