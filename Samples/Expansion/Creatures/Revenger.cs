namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureExType.Revenger))]
public class Revenger : CreatureEx
{
    public Revenger(Biota biota) : base(biota) { }
    public Revenger(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

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