
namespace Balance;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    private static List<AngouriMathPatch> enabledPatches = new();

    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;

        enabledPatches.Clear();
        bool defaultFormulaUsed = false;

        var sb = new StringBuilder("\n");
        foreach (var patchSettings in Settings.Formulas)
        {
            //Basic check for if the patch is supplying the default formula to the settings.  Settings saved if true
            if (string.IsNullOrWhiteSpace(patchSettings.Formula)) defaultFormulaUsed = true;

            var patch = patchSettings.CreatePatch();

            try
            {
                if (patchSettings.Enabled)
                {
                    patch.Start();
                    enabledPatches.Add(patch);
                }
                sb.AppendLine($"{patchSettings.PatchType} patched with:\n  {patch.Formula}");

            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to patch {patchSettings.PatchType}: {ex.Message}", ModManager.LogLevel.Error);
                sb.AppendLine($"Failed to patch {patchSettings.PatchType}:\n  {patch.Formula}");
            }
        }
        if (Settings.Verbose)
            ModManager.Log(sb.ToString());

        //TODO: 
        //if (defaultFormulaUsed)
            //SaveSettings();
    }

    public override void Stop()
    {
        base.Stop();

        //Shutdown/unpatch everything on settings change to support repatching by category
        foreach (var patch in enabledPatches)
        {
            patch.Shutdown();
        }
    }
}