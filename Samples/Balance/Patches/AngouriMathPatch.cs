namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(AngouriMathPatch))]
public abstract class AngouriMathPatch : IPatchable
{
    #region Fields / Props
    //Named property used to indicate patch and enable in settings
    [JsonPropertyName($"{nameof(AngouriMathPatch)} Enable")]
    public virtual bool Enabled { get; set; } = true;

    public virtual string Formula { get; set; } = "x";
    public virtual Dictionary<string, MType> Variables { get; } = new()
    {
        ["x"] = MType.Double
    };

    //static Func<double, int> func;
    #endregion

    #region Start/Stop
    public virtual void Start()
    {
        //If you can parse the formulas patch the corresponding category
        //var formula = Formula;
        //if (formula.TryGetFunction<double, int>(out func, Variables.TypesAndNames()))
        //{
        //    Mod.Harmony.PatchCategory(nameof(AngouriMathPatch));
        //}
        ////Otherwise skip patching
        //else
        //{
        //    throw new Exception($"Failure parsing formula: {formula}");
        //}
    }

    public virtual void Shutdown()
    {
        //Per-category patches don't exist in the same way.
        //Assume all patches are shut down then unpatched before restarting
        //Clean up objects
        //func = null;
    }
    #endregion
}
