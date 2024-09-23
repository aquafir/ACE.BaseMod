namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureExType.Bard))]
public class Bard : CreatureEx
{
    public Bard(Biota biota) : base(biota) { }
#if REALM
    public Bard(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Bard(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
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
