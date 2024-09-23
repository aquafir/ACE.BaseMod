namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Stomper))]
public class Stomper : CreatureEx
{
    public Stomper(Biota biota) : base(biota) { }
#if REALM
    public Stomper(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Stomper(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Massive " + Name;
    }

    //Custom behavior
    const float maxSplash = .8f;
    const int targets = 5;
    const float range = 10;
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DamageEvent), "DoCalculateDamage", new Type[] { typeof(Creature), typeof(Creature), typeof(WorldObject) })]
    public static void PostDoCalculateDamage(Creature attacker, Creature defender, WorldObject damageSource, ref DamageEvent __instance, ref float __result)
    {
        if (attacker is not Stomper c || defender is not Player p)
            return;

        foreach (Player n in p.GetSplashTargets(c, TargetExclusionFilter.OnlyPlayer, range))
        {
            //Skip self
            if (n == p) continue;

            var distance = n.GetDistance(c);

            //Todo: fix GetNearby
            if (distance > range)
                continue;

            var fraction = 1 - distance / range;
            var damage = maxSplash * fraction * __instance.Damage; //__instance.DamageBeforeMitigation;
            var dType = __instance.DamageType;

            if (!n.TryDamageDirect(damage, out var taken, dType))
                continue;

            n.SendMessage($"{c.Name} splashed you with {taken} {dType.GetName()} damage\n{damage} damage = {fraction:P2}% of {__instance.Damage}\n{distance}/{range} away");
        }
    }
}