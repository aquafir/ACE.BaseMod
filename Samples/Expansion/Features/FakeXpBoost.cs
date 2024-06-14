namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeXpBoost))]
[HarmonyPatchCategory(nameof(Feature.FakeXpBoost))]
internal class FakeXpBoost
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GrantXP), new Type[] { typeof(long), typeof(XpType), typeof(ShareType) })]
    public static bool PreGrantXP(ref long amount, XpType xpType, ShareType shareType, ref Player __instance)
    {
        if (xpType != XpType.Kill)
            return true;

        var bonus = __instance.GetCachedFake(FakeFloat.ItemXpBoost);
        var bonusAmount = (long)(bonus * amount);
        amount += bonusAmount;


        __instance.SendMessage($"Added {bonusAmount} xp from {1 + bonus} equipment bonus.");

        //Return true to execute original
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GrantLuminance), new Type[] { typeof(long), typeof(XpType), typeof(ShareType) })]
    public static bool PreGrantLuminance(long amount, XpType xpType, ShareType shareType, ref Player __instance)
    {
        if (xpType != XpType.Kill)
            return true;

        var bonus = __instance.GetCachedFake(FakeFloat.ItemLuminanceBoost);
        var bonusAmount = (long)(bonus * amount);
        amount += bonusAmount;


        __instance.SendMessage($"Added {bonusAmount} luminance from {1 + bonus} equipment bonus.");

        //Return true to execute original
        return true;
    }
}
