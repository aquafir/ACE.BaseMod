using SharpGen.Runtime;
using System.Runtime.CompilerServices;

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




    [HarmonyPrefix]
    [HarmonyPatch(typeof(DisposeBase), "InvokeDisposeEventHandler", new Type[] { typeof(bool), typeof(ReaderWriterLockSlim), typeof(ConditionalWeakTable<DisposeBase, DisposeEventHandler>) })]
    public static bool PreInvokeDisposeEventHandler(bool disposing, ReaderWriterLockSlim rwLock, ConditionalWeakTable<DisposeBase, DisposeEventHandler> table, ref DisposeBase __instance)
    {
        rwLock.EnterReadLock();
        DisposeEventHandler value;
        try
        {
            table.TryGetValue(__instance, out value);
        }
        catch(Exception ex)
        {
            ModManager.Log(ex.GetFullMessage(), ModManager.LogLevel.Error);
            rwLock?.ExitReadLock();
            return false;
        }
        finally
        {
            rwLock?.ExitReadLock();
        }
        value?.Invoke(__instance, disposing); return false;
    }

}
