namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureExType.))]
public class Suppresser : CreatureEx
{
    public Suppresser(Biota biota) : base(biota) { }
#if REALM
    public Suppresser(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Suppresser(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
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