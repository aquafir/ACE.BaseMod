namespace Balance;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(AngouriMathPatch))]
public abstract class AngouriMathPatch : IPatchable
{
    #region Fields / Props
    public virtual string Formula { get; set; } = "x";
    protected virtual Dictionary<string, MType> Variables { get; } = new()
    {
        ["x"] = MType.Double
    };

    //static Func<double, int> func;
    #endregion

    #region Start/Stop
    public virtual void Start()
    {
        throw new NotImplementedException();

        //If you can parse the formulas patch the corresponding category
        //var formula = Formula;
        //if (formula.TryGetFunction<double, int>(out func, Variables.TypesAndNames()))
        //{
        //    Mod.Instance.Harmony.PatchCategory(nameof(AngouriMathPatch));
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
        //Clean up objects.  If you do something like null func you'll have to check for nulls when using it in patches
    }
    #endregion
}
