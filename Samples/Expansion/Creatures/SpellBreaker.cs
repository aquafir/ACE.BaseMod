namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.SpellBreaker))]
public class SpellBreaker : CreatureEx
{
    public SpellBreaker(Biota biota) : base(biota) { }
#if REALM
    public SpellBreaker(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public SpellBreaker(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Breaking " + Name;
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DamageEvent), "DoCalculateDamage", new Type[] { typeof(Creature), typeof(Creature), typeof(WorldObject) })]
    public static void PostDoCalculateDamage(Creature attacker, Creature defender, WorldObject damageSource, ref DamageEvent __instance, ref float __result)
    {
        if (attacker is not SpellBreaker c || defender is not Player p) return;
        if (!__instance.HasDamage) return;

        if (p.CombatMode == CombatMode.Magic && p.MagicState.IsCasting && p.MagicState.CastSpellParams is not null)
        {
            var used = p.MagicState.CastSpellParams.ManaUsed;
            p.FailCast(true);
            p.TryDamageDirect(used, out var taken, DamageType.Fire);
            p.SendMessage($"{c.Name} broke your spell burning you for {used}.");
        }
    }
}