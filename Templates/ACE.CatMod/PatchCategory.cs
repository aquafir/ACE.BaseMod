namespace ACE.CatMod;

[HarmonyPatchCategory(Settings.PatchCategoryName)]
public class PatchCategory
{
    #region Patches
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Creature), nameof(Creature.GetDeathMessage), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
    //public static void PreDeathMessage(DamageHistoryInfo lastDamagerInfo, DamageType damageType, bool criticalHit, ref Creature __instance)
    //{
    //  ...
    //}
    #endregion
}