namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(SkillChance))]
public class SkillChance : AngouriMathPatch
{
    #region Fields / Props   
    //Originally 1.0 - (1.0 / (1.0 + Math.Exp(factor * (skill - difficulty))));
    public override string Formula { get; set; } = "1 - 1/(1+e^(f(x-d)))";
    protected override Dictionary<string, MType> Variables { get; } = new()
    {
        ["x"] = MType.Int,      // Skill
        ["d"] = MType.Int,      // Difficulty
        ["f"] = MType.Float,    // Factor
                                // 0-1 chance of success
    };

    static Func<int, int, float, double> func;
    #endregion

    #region Start / Stop
    public override void Start()
    {
        //If you can parse the formulas patch the corresponding category
        if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
            Mod.Instance.Harmony.PatchCategory(nameof(SkillChance));
        else
            throw new Exception($"Failure parsing formula: {Formula}");
    }
    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SkillCheck), nameof(SkillCheck.GetSkillChance), new Type[] { typeof(int), typeof(int), typeof(float) })]
    public static bool PreGetSkillChance(int skill, int difficulty, float factor, ref double __result)
    {
        var chance = func(skill, difficulty, factor);

        //Don't think the clamp is actually necessary for the way it is used
        __result = Math.Clamp(chance, 0, 1);

        return false;
    }

    #endregion
}