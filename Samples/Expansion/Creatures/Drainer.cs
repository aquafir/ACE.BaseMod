namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Drainer))]
public class Drainer : CreatureEx
{
    public Drainer(Biota biota) : base(biota) { }
#if REALM
    public Drainer(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Drainer(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    DamageType drainType = DamageType.Stamina;
    protected override void Initialize()
    {
        base.Initialize();

        //Some chance to drain mana instead
        if (ThreadSafeRandom.Next(0, 100) > 50)
            drainType = DamageType.Mana;

        var prefix = drainType == DamageType.Mana ? "Stultifying " : "Atrophying ";
        Name = prefix + Name;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DamageEvent), "DoCalculateDamage", new Type[] { typeof(Creature), typeof(Creature), typeof(WorldObject) })]
    public static void PostDoCalculateDamage(Creature attacker, Creature defender, WorldObject damageSource, ref DamageEvent __instance, ref float __result)
    {
        if (attacker is not Drainer d || defender is not Player p) return;
        if (!__instance.HasDamage) return;

        var vital = d.drainType switch
        {
            DamageType.Mana => p.Mana,
            _ => p.Stamina,
        };

        var amount = -p.UpdateVitalDelta(vital, -(int)__instance.Damage);
        if (amount > 0)
            p.SendMessage($"{d.Name} has leeched {amount} {d.drainType.GetName()}");
    }
}