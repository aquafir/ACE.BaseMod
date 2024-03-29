namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Banisher))]
public class Banisher : CreatureEx
{
    public Banisher(Biota biota) : base(biota) { }
    public Banisher(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Banishing " + Name;
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        if(AttackTarget != null && AttackTarget is Player player && player.CurrentActivePet is not null)
        {
            //player?.CurrentActivePet?.TakeDamage(this, DamageType.Nether, 999999);
            player?.CurrentActivePet?.Die();
            player?.SendMessage($"{Name} has banished your pet");
        }

        base.Heartbeat(currentUnixTime);
    }
}