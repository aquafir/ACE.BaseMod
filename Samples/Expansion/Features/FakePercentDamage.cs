namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakePercentDamage))]
[HarmonyPatchCategory(nameof(Feature.FakePercentDamage))]
public static class FakePercentDamage
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    public static void PostDamageTarget(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    {
        if (__result is null || !__result.HasDamage)
            return;

        var vital = target.Health;
        var mitigation = __result.Damage / __result.DamageBeforeMitigation;

        //Percent max
        var percent = __instance.GetCachedFake(FakeFloat.ItemPercentMaxHealthDamage);
        if (percent > 0)
        {

            var max = __instance.GetCachedFake(FakeInt.ItemPercentMaxHealthCap);
            if (max == 0)
                max = int.MaxValue;

            uint damage = (uint)Math.Min(max, percent * vital.MaxValue);
            var trueDamage = damage * mitigation;   //Flat or a ratio based on result?
            target.TakeDamage(__instance, DamageType.Health, trueDamage);
            //var damageEvent = new DamageEvent()
            //{
            //    DamageSource = __instance,
            //    CombatType = CombatType.Magic,
            //    Damage = trueDamage,
            //};
            //__instance.FakeDamage(damageEvent, target);
            __instance.SendMessage($"You hit {target.Name} for {trueDamage} percent-of-max health damage.");
        }

        //Percent current
        percent = __instance.GetCachedFake(FakeFloat.ItemPercentCurrentHealthDamage);
        if (percent > 0)
        {
            var max = __instance.GetCachedFake(FakeInt.ItemPercentCurrentHealthCap);
            if (max == 0)
                max = int.MaxValue;

            uint damage = (uint)Math.Min(max, percent * vital.Current);
            var trueDamage = damage * mitigation;   //Flat or a ratio based on result?
            target.TakeDamage(__instance, DamageType.Health, trueDamage);
            __instance.SendMessage($"You hit {target.Name} for {trueDamage} percent-of-current health damage.");
        }

        //Percent missing
        percent = __instance.GetCachedFake(FakeFloat.ItemPercentMissingHealthDamage);
        if (percent > 0)
        {
            var max = __instance.GetCachedFake(FakeInt.ItemPercentMissingHealthCap);
            if (max == 0)
                max = int.MaxValue;

            uint damage = (uint)Math.Min(max, percent * vital.Missing);
            var trueDamage = damage * mitigation;   //Flat or a ratio based on result?
            target.TakeDamage(__instance, DamageType.Health, trueDamage);
            __instance.SendMessage($"You hit {target.Name} for {trueDamage} percent-of-missing health damage.");
        }
    }

    //public static DamageEvent FakeDamage(this DamageEvent damageEvent) {
    //Todo: make fake damage from helper?
    //DamageRatingMod = Creature.AdditiveCombine(DamageRatingBaseMod, RecklessnessMod, SneakAttackMod, HeritageMod);
    // damage before mitigation
    //DamageBeforeMitigation = BaseDamage * AttributeMod * PowerMod * SlayerMod * DamageRatingMod;
    // calculate final output damage
    //Damage = DamageBeforeMitigation * ArmorMod * ShieldMod * ResistanceMod * DamageResistanceRatingMod;
    //}

    public static void FakeDamage(this Player player, DamageEvent damageEvent, Creature target)
    {
        if (damageEvent is null || !damageEvent.HasDamage)
            return;

        player.OnDamageTarget(target, damageEvent.CombatType, damageEvent.IsCritical);

        //if (target is Player targetPlayer)
        //{
        // //   targetPlayer.TakeDamage(this, damageEvent);
        //}
        //else
        target.TakeDamage(player, damageEvent.DamageType, damageEvent.Damage, damageEvent.IsCritical);

    }
}
