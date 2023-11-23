using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Network;

namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakeLeech))]
internal class FakeLeech
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    public static void PostDamageTarget(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    {
        if (!__result.HasDamage)
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
