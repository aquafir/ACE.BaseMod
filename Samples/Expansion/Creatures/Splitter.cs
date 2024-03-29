namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureExType.Splitter))]
public class Splitter : CreatureEx
{
    public Splitter(Biota biota) : base(biota) { }
    public Splitter(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

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