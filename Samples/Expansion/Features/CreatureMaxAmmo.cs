namespace Expansion.Features;

[CommandCategory(nameof(Feature.CreatureMaxAmmo))]
[HarmonyPatchCategory(nameof(Feature.CreatureMaxAmmo))]
internal class CreatureMaxAmmo
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.RangeAttack))]
    public static void PostRangeAttack(ref Creature __instance)
    {
        var max = __instance.GetProperty(FakeInt.MaxAmmo) ?? 0;
        if (max <= 0)
            return;

        //Max number thrown regardless of hitting or not
        if (__instance.MonsterProjectile_OnCollideEnvironment_Counter++ >= max)
            __instance.TrySwitchToMeleeAttack();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.MonsterProjectile_OnCollideEnvironment))]
    public static bool PreMonsterProjectile_OnCollideEnvironment(ref Creature __instance)
    {
        return __instance.GetProperty(FakeInt.MaxAmmo) != null;
    }
}
