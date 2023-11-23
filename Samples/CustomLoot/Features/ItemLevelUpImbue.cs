namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.ItemLevelUpImbue))]
internal class ItemLevelUpImbue
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.OnItemLevelUp), new Type[] { typeof(WorldObject), typeof(int) })]
    public static bool PreOnItemLevelUp(WorldObject item, int prevItemLevel, ref Player __instance)
    {
        //Return false to override
        //return false;

        Debugger.Break();
        if (item.ImbuedEffect != ImbuedEffectType.Undef)
            return true;

            if (ImbueGroup.AllRending.SetOf().TryGetRandom(out var imbuedEffectType))
                item.ImbuedEffect = imbuedEffectType;

        //Return true to execute original
        return true;
    }
}
