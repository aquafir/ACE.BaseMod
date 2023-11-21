namespace CustomLoot;

/// <summary>
/// Patch settings are used to serialized patches
/// </summary>
public class MutatorSettings
{
    public const string NAMESPACE = $"CustomLoot.Mutators";
    //public string PatchType { get; set; }
    public Mutation PatchType { get; set; }
    public bool Enabled { get; set; } = true;
    //If settings are missing Odds or Targets
    public string Odds { get; set; } 
    public string Targets { get; set; } 

    //public MutatorSettings() { }

    //public MutatorSettings(string patchType, bool enabled = true)
    //{
    //    PatchType = patchType;
    //    Enabled = enabled;
    //}

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

        //Nullable odds?
        mutator.TargetTypes = PatchClass.Settings.TargetGroups.TryGetValue(settings.Targets, out var targets) ? targets.ToHashSet() : null;
        mutator.Odds = PatchClass.Settings.Odds.TryGetValue(settings.Odds, out var mutatorOdds) ? mutatorOdds : null;

        return mutator;
    }
}