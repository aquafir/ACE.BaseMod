namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Comboer))]
public class Comboer : CreatureEx
{
    public Comboer(Biota biota) : base(biota) { }
#if REALM
    public Comboer(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Comboer(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Frenetic " + Name;
    }

    const int threshold = 5;
    int combo = 0;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DamageEvent), "DoCalculateDamage", new Type[] { typeof(Creature), typeof(Creature), typeof(WorldObject) })]
    public static void PostDoCalculateDamage(Creature attacker, Creature defender, WorldObject damageSource, ref DamageEvent __instance, ref float __result)
    {
        if (attacker is not Comboer c || defender is not Player p)
            return;

        c.combo++;
        if (c.combo < threshold)
            return;
        if (c.combo < threshold * 2)
            c.TryCastSpell_WithRedirects(new Spell(1785), p);
        else
            c.TryCastSpell_WithRedirects(new Spell(6191), p);

        if (c.combo % threshold == 0)
            p.SendMessage($"{c.Name} hit {c.combo} times");
    }

    public override uint TakeDamage(WorldObject source, DamageType damageType, float amount, bool crit = false)
    {
        combo = 0;
        return base.TakeDamage(source, damageType, amount, crit);
    }
}