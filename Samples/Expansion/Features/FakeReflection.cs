namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeReflection))]
[HarmonyPatchCategory(nameof(Feature.FakeReflection))]
internal class FakeReflection
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.TakeDamage), new Type[] { typeof(WorldObject), typeof(DamageEvent) })]
    public static void PostTakeDamage(WorldObject source, DamageEvent damageEvent, ref Player __instance, ref int __result)
    {
        //if(damageEvent.Attacker)

        var flat = __instance.GetCachedFake(FakeInt.ItemReflectFlat);
        var percent = (int)(__instance.GetCachedFake(FakeFloat.ItemReflectPercent) * __result);
        var total = flat + percent;

        if (total < 1)
            return;

        __instance.SendMessage($"You reflected {flat} flat and {percent} percent damage of the {__result} taken at {source.Name}");
        damageEvent.Attacker.TakeDamage(__instance, DamageType.Health, total);
    }
}