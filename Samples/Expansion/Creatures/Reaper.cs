namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureEx))]
public class Reaper : CreatureEx
{
    public Reaper(Biota biota) : base(biota) { }
    public Reaper(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

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