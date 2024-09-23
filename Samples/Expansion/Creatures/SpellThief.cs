namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.SpellThief))]
public class SpellThief : CreatureEx
{
    public SpellThief(Biota biota) : base(biota) { }
#if REALM
    public SpellThief(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public SpellThief(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Thaumocleptic " + Name;
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
        if (attacker is not SpellThief c || defender is not Player p) return;
        if (!__instance.HasDamage) return;

        //if (!p.EnchantmentManager.HasEnchantments) return;
        if (!p.TryGetRandomEnchantment(out var enchantment))
            return;

        var spell = new Spell(enchantment.SpellId);
        var result = c.EnchantmentManager.Add(spell, c, null);

        p.EnchantmentManager.Remove(enchantment);
        p.SendMessage($"{c.Name} stole your {spell.Name}.");
    }
}