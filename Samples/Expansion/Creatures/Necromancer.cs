namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureExType.Necromancer))]
public class Necromancer : CreatureEx
{
    public Necromancer(Biota biota) : base(biota) { }
#if REALM
    public Necromancer(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Necromancer(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

    }
}