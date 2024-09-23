namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureExType.Splitter))]
public class Splitter : CreatureEx
{
    public Splitter(Biota biota) : base(biota) { }
#if REALM
    public Splitter(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Splitter(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
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