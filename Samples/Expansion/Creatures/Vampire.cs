namespace Expansion.Creatures;

[HarmonyPatch]
public class Vampire : CreatureEx
{
    public Vampire(Biota biota) : base(biota) { }
    public Vampire(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Vampiric " + Name;
    }

    //Custom behavior
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DamageEvent), "DoCalculateDamage", new Type[] { typeof(Creature), typeof(Creature), typeof(WorldObject) })]
    public static void PostDoCalculateDamage(Creature attacker, Creature defender, WorldObject damageSource, ref DamageEvent __instance, ref float __result)
    {
        if (attacker is not Vampire v || defender is not Player p) return;
        if (!__instance.HasDamage) return;

        var amount = attacker.UpdateVitalDelta(attacker.Health, (int)__instance.Damage);
        if (amount > 0)
            p.SendMessage($"{v.Name} has leeched {amount} health,");
    }
}
