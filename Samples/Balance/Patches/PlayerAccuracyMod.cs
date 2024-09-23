namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(PlayerAccuracyMod))]
public class PlayerAccuracyMod : AngouriMathPatch
{
    #region Fields / Props   
    public override string Formula { get; set; } = ".6 + x";
    protected override Dictionary<string, MType> Variables { get; } = new()
    {
        ["x"] = MType.Float,    // accuracy level
        ["n"] = MType.Int,      // # connections
    };

    static Func<float, int, float> func;
    #endregion

    #region Start / Stop
    public override void Start()
    {
        //If you can parse the formulas patch the corresponding category
        if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
            Mod.Instance.Harmony.PatchCategory(nameof(PlayerAccuracyMod));
        else
            throw new Exception($"Failure parsing formula: {Formula}");
    }
    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetAccuracyMod), new Type[] { typeof(WorldObject) })]
    public static bool PreGetAccuracyMod(WorldObject weapon, ref Player __instance, ref float __result)
    {
        if (weapon != null && weapon.IsRanged)
            __result = func(__instance.AccuracyLevel, __instance.ActiveConnections());
        else
            __result = 1.0f;

        return false;
    }

    #endregion
}