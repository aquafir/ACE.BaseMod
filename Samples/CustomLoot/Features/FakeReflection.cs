using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoot.Features;

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
        //Player
        //Your code here
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpellProjectile), nameof(SpellProjectile.OnCollideObject), new Type[] { typeof(WorldObject) })]
    public static bool PreOnCollideObject(WorldObject target, ref SpellProjectile __instance)
    {
        if (target is not Player player)
            return true;

        if (__instance.ProjectileSource is not Creature creature)
            return true;

        var reflectChance = player.GetCachedFake(FakeFloat.ItemReflectSpellChance);
            if (reflectChance > 0 && ThreadSafeRandom.Next(0f, 1.0f) < reflectChance)
        {
            var reflectedSpell = new Spell(__instance.Spell.Id);
            player.TryCastSpell_WithRedirects(reflectedSpell, creature);
            player.SendMessage($"You reflected {reflectedSpell.Name} with {reflectChance:0.0} chance at {creature.Name}");

            return false;
        }
        //Return false to override
        //return false;

        //Return true to execute original
        return true;
    }

}
