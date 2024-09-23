namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeLeech))]
[HarmonyPatchCategory(nameof(Feature.FakeLeech))]
internal class FakeLeech
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    public static void PostDamageTarget(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    {
        if (__result is null || !__result.HasDamage)
            return;

        var dmg = __result.Damage;
        var vital = __instance.Health;
        uint leech = (uint)(__instance.GetCachedFake(FakeFloat.ItemLeechHealth) * dmg);
        if (leech > 0)
        {
            __instance.UpdateVitalDelta(vital, leech);
            __instance.SendMessage($"You leeched {leech} health.");
        }

        leech = (uint)(__instance.GetCachedFake(FakeFloat.ItemLeechMana) * dmg);
        if (leech > 0)
        {
            vital = __instance.Mana;
            __instance.UpdateVitalDelta(vital, leech);
            __instance.SendMessage($"You leeched {leech} mana.");
        }

        leech = (uint)(__instance.GetCachedFake(FakeFloat.ItemLeechStamina) * dmg);
        if (leech > 0)
        {
            vital = __instance.Stamina;
            __instance.UpdateVitalDelta(vital, leech);
            __instance.SendMessage($"You leeched {leech} stamina.");
        }
    }
}
