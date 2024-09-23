namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Stunner))]
public class Stunner : CreatureEx
{
    public Stunner(Biota biota) : base(biota) { }
#if REALM
    public Stunner(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Stunner(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        Name = "Debilitating " + Name;
    }

    //Custom behavior
    const int interval = 3;
    private int count = interval;

    //private float motionLength = MotionTable.GetAnimationLength((uint)stunMotion, stance, command);
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

        if (count-- > 0)
            return;

        if (AttackTarget is not Player p || GetDistanceToTarget() > 8)
            return;

        count = interval;

        //p.SendMessage($"You have been stunned by {Name}.");

        ////Get stun duration
        //float motionLength = MotionTable.GetAnimationLength(p.MotionTableId, stance, command, speed);

        ////Add current time before queued stun will play
        //p.GetCurrentMotionState(out var pStance, out var pMotion);
        //if (pStance != MotionStance.Invalid && pMotion != MotionCommand.Ready)
        //    motionLength += MotionTable.GetAnimationLength(p.MotionTableId, stance, command);

        //var actionChain = new ActionChain();
        //actionChain.AddAction(p, () =>
        //{
        //    //Todo figure out what to actually disable
        //    p.EnqueueBroadcastMotion(stunMotion);

        //    p.OnAttackDone();
        //    p.FailCast(false);
        //});
        //actionChain.AddDelaySeconds(motionLength);
        //actionChain.AddAction(p, () => p.SendMessage("The stun has worn off."));
        //actionChain.EnqueueChain();
    }
}