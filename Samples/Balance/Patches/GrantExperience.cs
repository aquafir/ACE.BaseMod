namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(GrantExperience))]
public class GrantExperience : AngouriMathPatch
{
    #region Fields / Props   
    public override string Formula { get; set; } = "P(0 if t = 0, x * 3/n)";
    protected override Dictionary<string, MType> Variables { get; } = new()
    {
        ["x"] = MType.Long, // xp amount
        ["t"] = MType.Int,  // type
        ["n"] = MType.Int,  // # connections
    };

    //Function parsed from formula used in patches
    static Func<long, int, int, long> func;
    #endregion

    #region Start / Stop
    public override void Start()
    {
        //If you can parse the formulas patch the corresponding category
        if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
            Mod.Instance.Harmony.PatchCategory(nameof(GrantExperience));
        else
            throw new Exception($"Failure parsing formula: {Formula}");
    }
    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GrantXP), new Type[] { typeof(long), typeof(XpType), typeof(ShareType) })]
    public static bool PreGrantXP(long amount, XpType xpType, ShareType shareType, ref Player __instance)
    {
        if (func is not null)
            amount = func(amount, (int)xpType, __instance.ActiveConnections());

        //Return true to execute original
        return true;
    }
    #endregion
}