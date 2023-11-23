namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakeXpBoost))]
internal class FakeXpBoost
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GrantXP), new Type[] { typeof(long), typeof(XpType), typeof(ShareType) })]
    public static bool PreGrantXP(ref long amount, XpType xpType, ShareType shareType, ref Player __instance)
    {
        if (xpType != XpType.Kill)
            return true;

        //Return false to override
        //return false;
        //double bonus = 0;
        //foreach(var item in __instance.EquippedObjects.Values)
        //    bonus += item.GetProperty(FakeFloat.ItemXpBoost) ?? 0;


        var bonus = __instance.GetCachedFake(FakeFloat.ItemXpBoost);
        var bonusAmount = (long)(bonus * amount);
        amount += bonusAmount;


        __instance.SendMessage($"Added {bonusAmount} xp from {1+bonus} equipment bonus.");

        //Return true to execute original
        return true;
    }
}
