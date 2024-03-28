namespace ExtendACE.Creatures;

[HarmonyPatch]
public class Healer : CreatureEx
{
    public Healer(Biota biota) : base(biota) { }
    public Healer(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Mending " + Name;

    }

    //Custom behavior
    const int candidates = 3;
    const float range = 15f;
    const float healThreshold = .95f;
    const int heartbeats = 3;
    int ticks = heartbeats;
    const float healPercent = .2f;
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

        if (AttackTarget is not Player p) return;

        if (--ticks > 0) return;

        //Look up to number of candidates in range to heal
        foreach (var creature in p.GetNearbyCreatures(this, candidates, range))
        {
            var percentHealth = creature.PercentHealth();
            if (percentHealth > healThreshold) continue;

            int healAmount = (int)(creature.Health.MaxValue * healPercent);
            var actualHealAmount = (uint)creature.UpdateVitalDelta(creature.Health, healAmount);

            p.SendMessage($"{Name} has healed {creature.Name} for {actualHealAmount}.");
        }

        ticks = heartbeats;
    }
}