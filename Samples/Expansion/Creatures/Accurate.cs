namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Accurate))]
public class Accurate : CreatureEx
{
    public Accurate(Biota biota) : base(biota) { }
#if REALM
    public Accurate(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Accurate(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Accurate " + Name;
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);
    }

    const int autoHitChance = 25;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DamageEvent), nameof(DamageEvent.GetEvadeChance), new Type[] { typeof(Creature), typeof(Creature) })]
    public static bool PreGetEvadeChance(Creature attacker, Creature defender, ref DamageEvent __instance, ref float __result)
    {
        if (attacker is Accurate && ThreadSafeRandom.Next(0, 100) > autoHitChance)
        {
            __result = 0f;

            if (defender is Player p)
                p.SendMessage($"{attacker.Name} skillfully connects.");

            return false;
        }

        return true;
    }
}