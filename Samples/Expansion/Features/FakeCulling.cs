namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeCulling))]
[HarmonyPatchCategory(nameof(Feature.FakeCulling))]
internal class FakeCulling
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    public static void PostDamageTarget(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    {
        if (__result is null || target is null)
            return;

        if (!__result.HasDamage || !target.IsAlive)
            return;

        //More cull = higher percent
        var cullPercent = __instance.GetCachedFake(FakeFloat.ItemPercentCull);

        if (cullPercent > target.Health.Percent)
        {
            __instance.SendMessage($"You have culled {target.Name} at {target.Health.Percent} of their health.");

            target.TakeDamage(__instance, DamageType.Health, target.Health.Current); ;
        }
    }
}
