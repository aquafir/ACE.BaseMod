using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakePercentDamage))]
internal class FakePercentDamage
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    public static void PostDamageTarget(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    {
        if (!__result.HasDamage)
            return;
        var dmg = __result.Damage;
        var vital = target.Health;

       // uint damage = (uint)(Math.Min(__instance.GetCachedFake(FakeFloat.ItemPercentMaxHealthDamage) * dmg, __instance.GetCachedFake(FakeInt));

    }
}
