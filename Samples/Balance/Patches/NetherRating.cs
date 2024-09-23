using ACE.Server.WorldObjects.Managers;

namespace Balance.Patches;

[HarmonyPatch]
[HarmonyPatchCategory(nameof(NetherRating))]
public class NetherRating : AngouriMathPatch
{
    #region Fields / Props   
    //x = regular rating, n = number of debuffs
    public override string Formula { get; set; } = "P(60 if x > 60, x)";
    protected override Dictionary<string, MType> Variables { get; } = new()
    {
        ["x"] = MType.Double,
        ["n"] = MType.Int,
    };

    //Function parsed from formula used in patches
    static Func<double, int, int> func;
    #endregion

    #region Start / Stop
    public override void Start()
    {
        //If you can parse the formulas patch the corresponding category
        if (Formula.TryGetFunction(out func, Variables.TypesAndNames()))
            Mod.Instance.Harmony.PatchCategory(nameof(NetherRating));
        else
            throw new Exception($"Failure parsing formula: {Formula}");

    }
    #endregion

    #region Patches
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnchantmentManager), nameof(EnchantmentManager.GetNetherDotDamageRating), new Type[] { })]
    public static bool PreGetNetherDotDamageRating(ref EnchantmentManager __instance, ref int __result)
    {
        if (func is null)
            return true;

        var type = EnchantmentTypeFlags.Int | EnchantmentTypeFlags.SingleStat | EnchantmentTypeFlags.Additive;
        var netherDots = __instance.GetEnchantments_TopLayer(type, (uint)PropertyInt.NetherOverTime);
        var netherCount = netherDots.Count;

        // this function produces a similar value to the original ACE function,
        // but is using the actual retail calculation method
        var totalBaseDamage = 0.0f;
        foreach (var netherDot in netherDots)
        {
            // normally we could just use netherDot.StatModValue here,
            // but in case WorldObject has a non-default HeartbeatInterval,
            // we want this value to still be based on the damage per default heartbeat interval
            totalBaseDamage += __instance.GetDamagePerTick(netherDot, 5.0);
        }

        var rating = func(Math.Round(totalBaseDamage / 8.0f), netherCount);   // thanks to Xenocide for this formula!                       
        __result = rating;

        //Override
        return false;
    }
    #endregion
}