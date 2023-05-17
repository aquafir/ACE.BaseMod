namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(GrantExperience))]
public class GrantExperience : AngouriMathPatch
{
    #region Fields / Props   
    //Named property used to indicate patch and enable in settings
    //[JsonPropertyName($"{nameof(GrantExperience)} Enable")]
    public override bool Enabled { get; set; } = true;

    //Formula and the variables used
    //x = Xp amount, t = type, n = number of active connections
    [JsonPropertyName($"Experience Formula")]
    public override string Formula { get; set; } = "P(0 if t = 0, x * 3/n)";
    [JsonInclude]
    public override  Dictionary<string, MType>  Variables { get; } = new()
    {
        ["x"] = MType.Long,
        ["t"] = MType.Int,
        ["n"] = MType.Int,
    };

    //Function parsed from formula used in patches
    static Func<long, int, int, long> func;
    #endregion

    #region Start / Stop
    public override void Start()
    {
        //If you can parse the formulas patch the corresponding category
        if (Formula.TryGetFunction<long, int, int, long>(out func, Variables.TypesAndNames()))
            Mod.Harmony.PatchCategory(nameof(GrantExperience));
        else
            throw new Exception($"Failure parsing formula: {Formula}");

    }

    public override void Shutdown()
    {
        func = null;
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