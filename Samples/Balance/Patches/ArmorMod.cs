namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(ArmorMod))]
public class ArmorMod : AngouriMathPatch
{
    #region Fields / Props   
    public override string Formula { get; set; } = "P(200/3 / (x + 200/3) if x > 0, (1-x)/(200/3x) if x < 0, 1)";
    protected override Dictionary<string, MType> Variables { get; } = new()
    {
        ["x"] = MType.Float,    // armor level
                                // Converts AL from an additive linear value to a scaled damage multiplier
    };

    static Func<float, float> func;
    #endregion

    #region Start / Stop
    public override void Start()
    {
        //If you can parse the formulas patch the corresponding category
        if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
            Mod.Instance.Harmony.PatchCategory(nameof(ArmorMod));
        else
            throw new Exception($"Failure parsing formula: {Formula}");
    }
    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SkillFormula), nameof(SkillFormula.CalcArmorMod), new Type[] { typeof(float) })]
    public static bool PreCalcArmorMod(float armorLevel, ref float __result)
    {
        __result = func(armorLevel);

        //Return false to override
        return false;
    }
    #endregion
}