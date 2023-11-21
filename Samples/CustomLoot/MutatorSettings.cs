using System.IO;
using System.Xml.Linq;
using CustomLoot.Enums;
using CustomLoot.Mutators;

namespace CustomLoot;

/// <summary>
/// Patch settings are used to serialized patches
/// </summary>
public class MutatorSettings
{
    public const string NAMESPACE = $"CustomLoot.Mutators";
    public string PatchType { get; set; }
    public bool Enabled { get; set; } = true;
    public string Odds { get; set; } = nameof(OddsType.Always);
    public string Targets { get; set; } = nameof(TargetGroup.Wearables);

    //public MutatorSettings() { }

    public MutatorSettings(string patchType, bool enabled = true)
    {
        PatchType = patchType;
        Enabled = enabled;
    }
}

public static class MutatorHelpers
{
    /// <summary>
    /// Get a patch type and try to create an instance of the corresponding class
    /// </summary>
    public static Mutator CreateMutator(this MutatorSettings settings)
    {
        //Mutators match enum
        var type = System.Type.GetType($"{MutatorSettings.NAMESPACE}.{settings.PatchType}");

        if (type is null)
        {
            Debugger.Break();
            throw new Exception();
        }

        var patchInstance = Activator.CreateInstance(type);
        if (patchInstance is not Mutator mutator)
        {
            Debugger.Break();
            throw new Exception();
        }

        //Nullable odds?
        mutator.TargetTypes = PatchClass.Settings.TargetGroups.TryGetValue(settings.Targets, out var targets) ? targets : null;
        mutator.Odds = PatchClass.Settings.Odds.TryGetValue(settings.Odds, out var mutatorOdds) ? mutatorOdds : null;

        return mutator;
    }
}