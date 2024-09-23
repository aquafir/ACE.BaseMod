namespace Expansion.Features;

[CommandCategory(nameof(Feature.DamageOverTimeConversion))]
[HarmonyPatchCategory(nameof(Feature.DamageOverTimeConversion))]
internal class DamageOverTimeConversion
{
    const int MAX_MITIGATION = 100;
    const int MIN_DAMAGE = 5;
    const int TICKS = 5;
    const double INTERVAL = 2;

    //Physical sources
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.TakeDamage), new Type[] { typeof(WorldObject), typeof(DamageType), typeof(float), typeof(BodyPart), typeof(bool), typeof(AttackConditions) })]
    public static void PreTakeDamage(WorldObject source, DamageType damageType, ref float _amount, BodyPart bodyPart, bool crit, AttackConditions attackConditions, Player __instance, ref int __result)
    {
        DeferDamage(__instance, ref _amount, crit, source, damageType);
    }

    //SpellProjectile
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpellProjectile), nameof(SpellProjectile.DamageTarget), new Type[] { typeof(Creature), typeof(float), typeof(bool), typeof(bool), typeof(bool) })]
    public static void PreDamageTarget(Creature target, float damage, bool critical, bool critDefended, bool overpower, ref SpellProjectile __instance)
    {
        if (target is not Player player)
            return;

        DeferDamage(player, ref damage, critical && !critDefended, __instance.ProjectileSource, __instance.Spell.DamageType, __instance);
    }

    public static void DeferDamage(Player player, ref float damage, bool crit, WorldObject source, DamageType damageType, SpellProjectile spell = null)
    {
        //Todo: rounding?
        if (damage < MIN_DAMAGE || player.UnderLifestoneProtection)
            return;

        var name = source?.Name ?? "Unknown";
        var dot = (int)Math.Min(damage, MAX_MITIGATION);
        var perTick = dot / TICKS;
        damage -= dot;

        player.SendMessage($"Taking {dot} damage as DoT");

        var actionChain = new ActionChain();
        for (var i = 0; i < TICKS; i++)
        {
            actionChain.AddDelaySeconds(2);
            actionChain.AddAction(player, () =>
            {
                player.UpdateVitalDelta(player.Health, -perTick);
                player.SendMessage($"You take {perTick}{(crit ? " critical" : "")} delayed {damageType} damage from {name}", ChatMessageType.CombatEnemy);
            });
        }
        actionChain.EnqueueChain();

        //Todo: choose between action chains or traditional route
        //HeartBeat_DamageOverTime finds relevant enchantments
        //Passes to ApplyDamageTick  (or ApplyHealingTick)
        //Enchantments found in WorldObject.Biota.PropertiesEnchantmentRegistry.GetEnchantmentsTopLayer
        //StatModKey PropertyInt.DamageOverTime
        //__instance.Biota.PropertiesEnchantmentRegistry.Add(new()
    }
}

