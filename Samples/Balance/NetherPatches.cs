using ACE.Server.WorldObjects.Managers;
using AngouriMath.Extensions;

namespace Balance;

[HarmonyPatch]
public static class NetherPatches
{
    #region Fields / Props
    static Func<double, int, int>? func; //x = regular rating, n = number of debuffs

    #endregion

    #region Start / Stop
    public static void Start()
    {
        try
        {
            func = PatchClass.Settings.NetherRatingFormula.Compile<double, int, int>("x", "n");
        }catch(Exception e)
        {
            ModManager.Log("Failed to parse equation: " + PatchClass.Settings.NetherRatingFormula, ModManager.LogLevel.Error);
        }
    }
    public static void Shutdown()
    {
    }
    #endregion

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnchantmentManager), nameof(EnchantmentManager.GetNetherDotDamageRating), new Type[] { })]
    public static bool PreGetNetherDotDamageRating(ref EnchantmentManager __instance, ref int __result)
    {
        if (!PatchClass.Settings.NetherRatingOverride || func is null)
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


    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnchantmentManager), nameof(EnchantmentManager.GetNetherDotDamageRating), new Type[] { })]
    public static void VoidCap(EnchantmentManager __instance, ref int __result)
    {
        //Repeated work, but it will be cached and probably not a huge performance issue
        var type = EnchantmentTypeFlags.Int | EnchantmentTypeFlags.SingleStat | EnchantmentTypeFlags.Additive;
        var numDebuffs = __instance.GetEnchantments_TopLayer(type, (uint)PropertyInt.NetherOverTime).Count;



        var cap = Math.Min(PatchClass.Settings.NetherRatingFormula, Settings.NetherPerDebuffCap * numDebuffs);

        ModManager.Log($"{__result} capped to {Settings.NetherRatingCap} or {numDebuffs} * {Settings.NetherPerDebuffCap}");

        __result = Math.Min(cap, __result);
    }

}