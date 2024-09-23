﻿using Expansion.Features;

namespace Expansion;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnWorldOpen()
    {
        MutatorHooks.SetupMutators();
        SetupFeatures();
    }

    public override void Stop()
    {
        base.Stop();

        MutatorHooks.ShutdownMutators();
        ModC.Harmony.UnpatchAll();
    }

    /// <summary>
    /// Adds additional features to ACE that may be needed by custom loot
    /// </summary>
    private void SetupFeatures()
    {
        //Add enabled Feature patches
        foreach (var feature in Settings.Features)
        {
            ModC.Harmony.PatchCategory(feature.ToString());

            if (Settings.Verbose)
                ModManager.Log($"Enabled feature: {feature}");
        }

        //Add enabled CreatureEx patches
        foreach (var feature in Settings.CreatureFeatures)
        {
            ModC.Harmony.PatchCategory(feature.ToString());

            if (Settings.Verbose)
                ModManager.Log($"Enabled CreatureEx feature: {feature}");
        }

        //Add commands of enabled features
        ModC.RegisterFeatureEnumCommands(Settings.Features);
        //var commandRegex = string.Join("|", Settings.Features);
        //ModC.Container.RegisterCommandCategory(commandRegex);
    }
}
