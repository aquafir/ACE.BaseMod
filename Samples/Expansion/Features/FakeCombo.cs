using ACE.Server.WorldObjects.Managers;

namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeCombo))]
[HarmonyPatchCategory(nameof(Feature.FakeCombo))]
public static class FakeCombo
{
    static float comboInterval = 10f;

    //Todo: Also have to reset on breakers besides time

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EmoteManager), nameof(EmoteManager.OnResistSpell), new Type[] { typeof(Creature) })]
    public static void PostOnResistSpell(Creature attacker, ref EmoteManager __instance)
    {
        if (__instance.WorldObject is Player player) { }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.Die), new Type[] { typeof(DamageHistoryInfo), typeof(DamageHistoryInfo) })]
    public static void PostDie(DamageHistoryInfo lastDamager, DamageHistoryInfo topDamager, ref Creature __instance)
    {
        if (!lastDamager.IsPlayer || lastDamager.TryGetPetOwnerOrAttacker() is not Player player)
            return;

        player.UpdateKillCombo();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    public static void PostDamageTarget(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    {
        if (__result is null || !__result.HasDamage)

            __instance.UpdateHitCombo();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.OnEvade), new Type[] { typeof(WorldObject), typeof(CombatType) })]
    public static void PostOnEvade(WorldObject attacker, CombatType attackType, ref Player __instance)
    {
        __instance.UpdateEvadeCombo();
    }



    //Could make generic if there's the same conditions
    public static void UpdateKillCombo(this Player player)
    {
        var time = Time.GetUnixTime();
        var last = player.GetProperty(FakeFloat.TimestampLastKill) ?? 0;
        var change = time - last;
        var combo = 1 + (player.GetProperty(FakeInt.ComboKill) ?? 0);
        player.SetProperty(FakeFloat.TimestampLastKill, time);

        if (change > comboInterval)
        {
            player.SendMessage($"You lost your combo of {combo} kills after {change:0.0} seconds.");
            player.SetProperty(FakeInt.ComboKill, 1);
        }
        else
        {
            player.SendMessage($"Combo! {combo} kills within {comboInterval:0.0} seconds. {change:0.0}");
            player.SetProperty(FakeInt.ComboKill, combo);

            if (combo % 10 == 0)
                player.PlayParticleEffect(PlayScript.HealthUpRed, player.Guid);
        }
    }
    public static void UpdateHitCombo(this Player player)
    {
        var time = Time.GetUnixTime();
        var last = player.GetProperty(FakeFloat.TimestampLastHit) ?? 0;
        var change = time - last;
        var combo = 1 + (player.GetProperty(FakeInt.ComboHit) ?? 0);
        player.SetProperty(FakeFloat.TimestampLastHit, time);

        if (change > comboInterval)
        {
            player.SendMessage($"You lost your combo of {combo} hits after {change:0.0} seconds.");
            player.SetProperty(FakeInt.ComboHit, 1);
        }
        else
        {
            player.SendMessage($"Combo! {combo} hits within {comboInterval:0.0} seconds. {change:0.0}");
            player.SetProperty(FakeInt.ComboHit, combo);

            if (combo % 10 == 0)
                player.PlayParticleEffect(PlayScript.HealthUpYellow, player.Guid);
        }
    }
    public static void UpdateEvadeCombo(this Player player)
    {
        var time = Time.GetUnixTime();
        var last = player.GetProperty(FakeFloat.TimestampLastEvade) ?? 0;
        var change = time - last;
        var combo = 1 + (player.GetProperty(FakeInt.ComboEvade) ?? 0);
        player.SetProperty(FakeFloat.TimestampLastEvade, time);

        if (change > comboInterval)
        {
            player.SendMessage($"You lost your combo of {combo} evasions after {change:0.0} seconds.");
            player.SetProperty(FakeInt.ComboEvade, 1);
        }
        else
        {
            player.SendMessage($"Combo! {combo} evasions within {comboInterval:0.0} seconds. {change:0.0}");
            player.SetProperty(FakeInt.ComboEvade, combo);

            if (combo % 10 == 0)
                player.PlayParticleEffect(PlayScript.HealthUpBlue, player.Guid);
        }
    }
    public static void UpdateResistCombo(this Player player)
    {
        var time = Time.GetUnixTime();
        var last = player.GetProperty(FakeFloat.TimestampLastResist) ?? 0;
        var change = time - last;
        var combo = 1 + (player.GetProperty(FakeInt.ComboResist) ?? 0);
        player.SetProperty(FakeFloat.TimestampLastResist, time);

        if (change > comboInterval)
        {
            player.SendMessage($"You lost your combo of {combo} resists after {change:0.0} seconds.");
            player.SetProperty(FakeInt.ComboResist, 1);
        }
        else
        {
            player.SendMessage($"Combo! {combo} resists within {comboInterval:0.0} seconds. {change:0.0}");
            player.SetProperty(FakeInt.ComboResist, combo);

            if (combo % 10 == 0)
                player.PlayParticleEffect(PlayScript.WeddingBliss, player.Guid);
        }
    }
}
