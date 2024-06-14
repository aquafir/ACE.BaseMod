namespace Expansion.Features;

[CommandCategory(nameof(Feature.LifeMagicElementalMod))]
[HarmonyPatchCategory(nameof(Feature.LifeMagicElementalMod))]
internal class LifeMagicElementalMod
{
    const DamageType LIFE_DAMAGE = DamageType.Health | DamageType.Mana | DamageType.Stamina;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpellProjectile), nameof(SpellProjectile.CalculateDamage), new Type[] { typeof(WorldObject), typeof(Creature), typeof(bool), typeof(bool), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Ref })]
    public static void PreCalculateDamage(WorldObject source, Creature target, bool criticalHit, bool critDefended, bool overpower, ref SpellProjectile __instance, ref float? __result)
    {
        //Only apply to players
        if (source is not Player player)
            return;

        //Early check for life spell without getting wand?
        //if (!__instance.Spell.DamageType.HasAny(LIFE_DAMAGE))
        //    return;

        var caster = player.GetEquippedWand();
        if (caster is null || !caster.W_DamageType.HasFlag(__instance.Spell.DamageType))
            return;

        //Use elemental mod
        //var elementalDamageMod = weapon.ElementalDamageMod ?? 1.0f;
        var boost = (uint)(__instance.LifeProjectileDamage * (caster.ElementalDamageMod ?? 1.0f));

        //Debug message
        //var init = __instance.LifeProjectileDamage;
        //player.SendMessage($"Increased damage from {init} to {boost} from {caster.ElementalDamageMod ?? 1.0f:P2} mod");

        __instance.LifeProjectileDamage = boost;
    }
}

