﻿namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureEx))]
public class Runner : CreatureEx
{
    public Runner(Biota biota) : base(biota) { }
    public Runner(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

                Name = "Panicked " + Name;
    }

    //Custom behavior
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);
    }

    public override void MoveTo(WorldObject target, float runRate = 1)
    {
        var newPos = new Position(target.Location);
        newPos.Pos = 2 * Location.Pos - target.Location.Pos;

        //Motion motion = 
        MoveToPosition(newPos);

        //base.MoveTo(target, runRate);

    }

    public Motion RunAway(WorldObject target, float runRate)
    {
        Motion motion = new Motion(this, target, MovementType.MoveToPosition);
        motion.MoveToParameters.MovementParameters |= MovementParams.CanCharge | MovementParams.FailWalk | MovementParams.UseFinalHeading | MovementParams.Sticky | MovementParams.MoveAway;
        motion.MoveToParameters.WalkRunThreshold = 1f;
        if (runRate > 0f)
        {
            motion.RunRate = runRate;
        }
        else
        {
            motion.MoveToParameters.MovementParameters &= ~MovementParams.CanRun;
        }

        return motion;
    }
}