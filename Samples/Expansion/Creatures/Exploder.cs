namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Exploder))]
public class Exploder : CreatureEx
{
    public Exploder(Biota biota) : base(biota) { }
#if REALM
    public Exploder(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Exploder(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        //SetProperty(PropertyFloat.DefaultScale, 2.0);
        Name = "Exploding " + Name;

        //Set a faster heartbeat
        HeartbeatInterval = 5f;
        ReinitializeHeartbeats();

        base.Initialize();
    }

    //Custom behavior
    const int originalTicks = 3;
    int ticks = 0;
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

        //Check if you're in exploding distance
        if (GetDistanceToTarget() > 5)
        {
            ticks = originalTicks;
            return;
        }

        //If you are, try to get the player you're near
        if (AttackTarget is not Player player) return;

        //Warn the player
        if (--ticks > 0)
        {
            player.SendMessage("Tick...".Repeat(ticks));
            return;
        }

        //Damage up to 10 players within 5 units of the exploder, using the player as a hack for the distance
        var targets = player.GetSplashTargets(player, TargetExclusionFilter.OnlyPlayer, 20).Take(10);

        var damage = this.PercentHealth() * 1000;
        foreach (Player p in targets)
        {
            if (p.TryDamageDirect(damage, out var taken, DamageType.Fire, true))
            {
                p.SendMessage($"{Name} explodes dealing {damage}.");
                p.PlayAnimation(PlayScript.Explode);
                p.PlaySound(Sound.Explode);
            }
        }
        OnDeath();
        Die();
    }
}
