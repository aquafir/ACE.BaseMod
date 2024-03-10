using ACE.Server.Network.Structure;
using ACE.Server.Physics.Common;
using ACE.Server.WorldObjects;

namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureEx))]
public class Revenant : CreatureEx
{
    public Revenant(Biota biota) : base(biota) { }
    public Revenant(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

    public Revenant(Weenie weenie, Player target) : base(weenie, GuidManager.NewDynamicGuid())
    {
        PermaTarget = target;
    }

    //ObjectGuid PermaTarget;
    Player? PermaTarget;

    //Mutate from the original weenie
    protected override void Initialize()
    {
        Name = "Stalking " + Name;
        base.Initialize();
    }

    public override void Heartbeat(double currentUnixTime)
    {
        //As long as it exists it tracks the player
        Stalk();
        base.Heartbeat(currentUnixTime);
    }

    public override bool FindNextTarget()
    { 
        //Ignore regular behavior
        return AttackTarget != null;
    }

    public void Stalk()
    {
        //Destroy when the player no longer accessible
        if (PermaTarget is null || PermaTarget.UnderLifestoneProtection)
        {
            Destroy();
            return;
        }

        //Target if proximal
        if (PermaTarget.CurrentLandblock == CurrentLandblock)
        {
            AttackTarget = PermaTarget;
            return;
        }

        //Teleport if distant
        if(Weenie is null || IsDead)
        {
            Destroy();
            return;
        }
        var next = new Revenant(this.Weenie, PermaTarget);
        next.Location = new(PermaTarget.Location);
        next.PermaTarget = PermaTarget;

        //Have the copy try to follow, destroying the original if successful
        if(next.EnterWorld())
            Destroy();
        else
        {
            GuidManager.RecycleDynamicGuid(next.Guid);
            next.Destroy();
        }
    }
}