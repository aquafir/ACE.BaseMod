namespace CustomLoot;

/// <summary>
/// Patch settings are used to serialized patches
/// </summary>
public class MutatorSettings
{
    public const string NAMESPACE = $"CustomLoot.Mutators";
    //public string PatchType { get; set; }
    public Mutation PatchType { get; set; }
    public bool Enabled { get; set; } = false;
    public MutationEvent Events { get; set; } = MutationEvent.Loot;
    public string? Odds { get; set; }
    public string? TreasureTargets { get; set; }
    public string? WeenieTypeTargets { get; set; }
    //public MutatorSettings() { }

    public MutatorSettings(Mutation patchType, bool enabled = true)
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
        var type = Type.GetType($"{MutatorSettings.NAMESPACE}.{settings.PatchType}");

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

        mutator.Event = settings.Events;

        //Nullable odds?
        mutator.TreasureTargets = S.Settings.TargetGroups.TryGetValue(settings.TreasureTargets ?? "", out var treasureTargets) ? treasureTargets.ToHashSet() : null;
        mutator.WeenieTypeTargets = S.Settings.WeenieTypeGroups.TryGetValue(settings.WeenieTypeTargets ?? "", out var weenieTargets) ? weenieTargets.ToHashSet() : null;
        mutator.Odds = S.Settings.Odds.TryGetValue(settings.Odds ?? "", out var mutatorOdds) ? mutatorOdds : null;

        ModManager.Log($"{mutator.TreasureTargets is null} - {mutator.WeenieTypeTargets is null} - {mutator.Odds is null}");

        return mutator;
    }
}