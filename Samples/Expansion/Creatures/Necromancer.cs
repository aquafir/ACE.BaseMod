namespace Expansion.Creatures;

//[HarmonyPatchCategory(nameof(CreatureExType.Necromancer))]
public class Necromancer : CreatureEx
{
    public Necromancer(Biota biota) : base(biota) { }
    public Necromancer(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

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