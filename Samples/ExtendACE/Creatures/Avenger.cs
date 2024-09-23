namespace ExtendACE.Creatures;

[HarmonyPatch]
public class Avenger : CreatureEx
{
    public Avenger(Biota biota) : base(biota) { }
    public Avenger(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

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