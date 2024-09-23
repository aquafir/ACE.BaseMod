namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Banisher))]
public class Banisher : CreatureEx
{
    public Banisher(Biota biota) : base(biota) { }
#if REALM
    public Banisher(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Banisher(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Banishing " + Name;
    }

    public override void OnCollideObject(WorldObject target)
    {
        if (target is CombatPet pet && pet.IsAlive)
        {
            var player = pet.P_PetOwner;
            player?.SendMessage($"{Name} has banished your pet on impact!");
            pet?.Die();
        }
        base.OnCollideObject(target);
    }

    //public override void OnDamageTarget(WorldObject target, CombatType attackType, bool critical)
    //{
    //    if (target is CombatPet pet && pet.IsAlive)
    //    {
    //        var player = pet.P_PetOwner;
    //        player?.SendMessage($"{Name} has banished your pet on attack!");
    //        pet?.Die();
    //    }
    //    base.OnDamageTarget(target, attackType, critical);
    //}

    public override void Heartbeat(double currentUnixTime)
    {
        if (AttackTarget != null && AttackTarget is Player player && player.CurrentActivePet is not null && player.CurrentActivePet.IsAlive)
        {
            player?.CurrentActivePet?.Die();
            player?.SendMessage($"{Name} has banished your pet");
        }

        base.Heartbeat(currentUnixTime);
    }
}