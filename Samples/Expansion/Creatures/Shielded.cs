﻿namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureEx))]
public class Shielded : CreatureEx
{
    public Shielded(Biota biota) : base(biota) { }
    public Shielded(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

                Name = "Shielded " + Name;
    }

    //Custom behavior
    const int reshieldTicks = 5;
    int ticks = reshieldTicks;
    const int maxShields = 3;
    int shields = maxShields;
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

        if (ticks-- > 0) return;
        
        ticks = reshieldTicks;
        shields = maxShields;
    }

    public override uint TakeDamage(WorldObject source, DamageType damageType, float amount, bool crit = false)
    {
        //Unshielded take normal damage
        if(shields < 1) return base.TakeDamage(source, damageType, amount, crit);

        //Otherwise neutralize
        shields--;
        if (source is Player p)
            p.SendMessage($"{Name} shielded ({shields}) {amount} damage");

        return 0;
    }
}