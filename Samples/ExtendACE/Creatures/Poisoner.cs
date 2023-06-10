namespace ExtendACE.Creatures;

[HarmonyPatch]
public class Poisoner : CreatureEx
{
    public Poisoner(Biota biota) : base(biota) { }
    public Poisoner(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

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