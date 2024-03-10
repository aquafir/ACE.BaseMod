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
        var guid = GuidManager.NewDynamicGuid();
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
        //return base.FindNextTarget();
    }

    public void Stalk()
    {
        GuidManager.NewDynamicGuid();

        //Destroy when the player no longer accessible
        if (PermaTarget is null)
        {
            PlayerManager.BroadcastToChannelFromEmote(Channel.Admin ,$"{Name} lost its target.");
            Destroy();
            return;
        }

        //Target if proximal
        if (PermaTarget.CurrentLandblock == CurrentLandblock)
        {
            AttackTarget = PermaTarget;
            return;
        }

        //Teleport if distance
        var next = new Revenant(this.Weenie, PermaTarget);

        next.Location = new(PermaTarget.Location);
        next.PermaTarget = PermaTarget;

        Destroy();
        next.EnterWorld();
    }
}