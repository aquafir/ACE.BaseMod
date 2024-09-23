namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Duelist))]
public class Duelist : CreatureEx
{
    public Duelist(Biota biota) : base(biota) { }
#if REALM
    public Duelist(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Duelist(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Dueling " + Name;
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);
    }

    const float HalfAngle = 40.0f / 2;
    public override uint TakeDamage(WorldObject source, DamageType damageType, float amount, bool crit = false)
    {
        if (source is not Player p)
            return base.TakeDamage(source, damageType, amount, crit);

        var angle = GetAngle(p);        //Creature to player angle
        if (Math.Abs(angle) > HalfAngle)
            return base.TakeDamage(source, damageType, amount, crit);

        p.SendMessage($"{Name} avoided {amount} damage by facing {angle}/{HalfAngle}");
        return 0;
    }
}