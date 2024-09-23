namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Evader))]
public class Evader : CreatureEx
{
    public Evader(Biota biota) : base(biota) { }
#if REALM
    public Evader(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Evader(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Evasive " + Name;
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);
    }

    const int autoDodgeChance = 25;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DamageEvent), nameof(DamageEvent.GetEvadeChance), new Type[] { typeof(Creature), typeof(Creature) })]
    public static bool PreGetEvadeChance(Creature attacker, Creature defender, ref DamageEvent __instance, ref float __result)
    {
        if (defender is Evader && ThreadSafeRandom.Next(0, 100) > autoDodgeChance)
        {
            __result = 1f;

            if (attacker is Player p)
                p.SendMessage($"{defender.Name} skillfully dodges.");

            return false;
        }

        return true;
    }
}