namespace ACE.Shared.Helpers;
public static class PlayerTargetExtensions
{
    public static void HandleActionTargetedMissileAttack(this Player player, Position position, uint attackHeight, float accuracyLevel)
    {
        if (player.CombatMode != CombatMode.Missile)
        {
            if (player.LastCombatMode == CombatMode.Missile)
                player.CombatMode = CombatMode.Missile;
            else
            {
                player.OnAttackDone();
                return;
            }
        }

        if (player.IsBusy || player.Teleporting || player.suicideInProgress)
        {
            player.SendWeenieError(WeenieError.YoureTooBusy);
            player.OnAttackDone();
            return;
        }

        if (player.IsJumping)
        {
            player.SendWeenieError(WeenieError.YouCantDoThatWhileInTheAir);
            player.OnAttackDone();
            return;
        }

        if (player.PKLogout)
        {
            player.SendWeenieError(WeenieError.YouHaveBeenInPKBattleTooRecently);
            player.OnAttackDone();
            return;
        }

        var weapon = player.GetEquippedMissileWeapon();
        var ammo = player.GetEquippedAmmo();

        // sanity check
        accuracyLevel = Math.Clamp(accuracyLevel, 0.0f, 1.0f);

        //Ammunition not needed for thrown items
        if (weapon == null || weapon.IsAmmoLauncher && ammo == null)
        {
            player.OnAttackDone();
            return;
        }

        player.AttackHeight = (AttackHeight)attackHeight;
        player.AttackQueue.Add(accuracyLevel);

        if (player.MissileTarget == null)
            player.AccuracyLevel = accuracyLevel;  // verify

        // get world object of target guid
        //var target = player.CurrentLandblock?.GetObject(player.targetGuid) as Creature;
        //if (target == null || target.Teleporting)
        //{
        //    //log.Warn($"{Name}.HandleActionTargetedMissileAttack({targetGuid:X8}, {AttackHeight}, {accuracyLevel}) - couldn't find creature target guid");
        //    player.OnAttackDone();
        //    return;
        //}

        if (player.Attacking || player.MissileTarget != null) //&& MissileTarget.IsAlive)
            return;

        //if (!player.CanDamage(target))
        //{
        //    player.SendTransientError($"You cannot attack {target.Name}");
        //    player.OnAttackDone();
        //    return;
        //}

        //log.Info($"{Name}.HandleActionTargetedMissileAttack({targetGuid:X8}, {attackHeight}, {accuracyLevel})");

        //player.AttackTarget = target;
        //player.MissileTarget = target;

        var attackSequence = ++player.AttackSequence;

        // record stance here and pass it along
        // accounts for odd client behavior with swapping bows during repeat attacks
        var stance = player.CurrentMotionState.Stance;

        // turn if required
        //var rotateTime = player.Rotate(position);
        var actionChain = new ActionChain();

        var delayTime = 0f; //rotateTime;
        if (player.NextRefillTime > DateTime.UtcNow.AddSeconds(delayTime))
            delayTime = (float)(player.NextRefillTime - DateTime.UtcNow).TotalSeconds;

        actionChain.AddDelaySeconds(delayTime);

        // do missile attack
        //actionChain.AddAction(this, () => LaunchMissile(target, attackSequence, stance));
        actionChain.AddAction(player, () => player.LaunchCustomMissile(position, attackSequence, stance));
        actionChain.EnqueueChain();
    }

    /// <summary>
    /// Launches a missile attack from player to target
    /// </summary>
    public static void LaunchCustomMissile(this Player player, Position target, int attackSequence, MotionStance stance, bool subsequent = false)
    {
        if (player.AttackSequence != attackSequence)
            return;

        var weapon = player.GetEquippedMissileWeapon();
        if (weapon == null || player.CombatMode == CombatMode.NonCombat)
        {
            player.OnAttackDone();
            return;
        }

        var ammo = weapon.IsAmmoLauncher ? player.GetEquippedAmmo() : weapon;
        if (ammo == null)
        {
            player.OnAttackDone();
            return;
        }

        var launcher = player.GetEquippedMissileLauncher();

        //Check custom range
        if (!player.TargetInRange(target))
        {
            // this must also be sent to actually display the transient message
            player.SendWeenieError(WeenieError.MissileOutOfRange);

            // this prevents the accuracy bar from refilling when 'repeat attacks' is enabled
            player.OnAttackDone();

            return;
        }

        var actionChain = new ActionChain();

        if (subsequent && !player.IsFacing(target))
        {
            var rotateTime = 0f;// player.Rotate(target);
            actionChain.AddDelaySeconds(rotateTime);
        }

        // launch animation
        // point of no return beyond this point -- cannot be cancelled
        actionChain.AddAction(player, () => player.Attacking = true);

        if (subsequent)
        {
            // client shows hourglass, until attack done is received
            // retail only did this for subsequent attacks w/ repeat attacks on
            player.Session.Network.EnqueueSend(new GameEventCombatCommenceAttack(player.Session));
        }

        var projectileSpeed = player.GetProjectileSpeed();

        // get z-angle for aim motion
        var aimVelocity = player.GetAimVelocity(target, projectileSpeed);

        var aimLevel = Player.GetAimLevel(aimVelocity);

        // calculate projectile spawn pos and velocity
        var localOrigin = player.GetProjectileSpawnOrigin(ammo.WeenieClassId, aimLevel);

        var velocity = player.CalculateProjectileVelocity(localOrigin, target, projectileSpeed, out Vector3 origin, out Quaternion orientation);

        //Console.WriteLine($"Velocity: {velocity}");

        if (velocity == Vector3.Zero)
        {
            // pre-check succeeded, but actual velocity calculation failed
            player.SendWeenieError(WeenieError.MissileOutOfRange);

            // this prevents the accuracy bar from refilling when 'repeat attacks' is enabled
            player.Attacking = false;
            player.OnAttackDone();
            return;
        }

        var launchTime = player.EnqueueMotionPersist(actionChain, aimLevel);

        // launch projectile
        actionChain.AddAction(player, () =>
        {
            // handle self-procs
            player.TryProcEquippedItems(player, player, true, weapon);

            var sound = player.GetLaunchMissileSound(weapon);
            player.EnqueueBroadcast(new GameMessageSound(player.Guid, sound, 1.0f));

            // stamina usage
            // TODO: ensure enough stamina for attack
            // TODO: verify formulas - double/triple cost for bow/xbow?
            var staminaCost = player.GetAttackStamina(player.GetAccuracyRange());
            player.UpdateVitalDelta(player.Stamina, -staminaCost);

            var projectile = player.LaunchProjectile(launcher, ammo, target, origin, orientation, velocity);
            player.UpdateAmmoAfterLaunch(ammo);
        });

        // ammo remaining?
        if (!ammo.UnlimitedUse && (ammo.StackSize == null || ammo.StackSize <= 1))
        {
            actionChain.AddAction(player, () =>
            {
                player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, "You are out of ammunition!"));
                player.SetCombatMode(CombatMode.NonCombat);
                player.Attacking = false;
                player.OnAttackDone();
            });

            actionChain.EnqueueChain();
            return;
        }

        // reload animation
        var animSpeed = player.GetAnimSpeed();
        var reloadTime = player.EnqueueMotionPersist(actionChain, stance, MotionCommand.Reload, animSpeed);

        // reset for next projectile
        player.EnqueueMotionPersist(actionChain, stance, MotionCommand.Ready);
        var linkTime = MotionTable.GetAnimationLength(player.MotionTableId, stance, MotionCommand.Reload, MotionCommand.Ready);
        //var cycleTime = MotionTable.GetCycleLength(MotionTableId, CurrentMotionState.Stance, MotionCommand.Ready);

        actionChain.AddAction(player, () =>
        {
            if (player.CombatMode == CombatMode.Missile)
                player.EnqueueBroadcast(new GameMessageParentEvent(player, ammo, ACE.Entity.Enum.ParentLocation.RightHand, ACE.Entity.Enum.Placement.RightHandCombat));
        });


        actionChain.AddDelaySeconds(linkTime);
        actionChain.AddAction(player, () =>
        {
            player.Attacking = false;
            //Repeat attacks?
            //if (player.GetCharacterOption(CharacterOption.AutoRepeatAttacks) && !player.IsBusy && !player.AttackCancelled)
            //{
            //    // client starts refilling accuracy bar
            //    player.Session.Network.EnqueueSend(new GameEventAttackDone(player.Session));

            //    player.AccuracyLevel = player.AttackQueue.Fetch();

            //    // can be cancelled, but cannot be pre-empted with another attack
            //    var nextAttack = new ActionChain();
            //    var nextRefillTime = player.AccuracyLevel;

            //    player.NextRefillTime = DateTime.UtcNow.AddSeconds(nextRefillTime);
            //    nextAttack.AddDelaySeconds(nextRefillTime);

            //    // perform next attack
            //    //Todo: repeat??
            //    nextAttack.AddAction(player, () => { player.LaunchCustomMissile(target, attackSequence, stance, true); });
            //    nextAttack.EnqueueChain();
            //}
            //else
            player.OnAttackDone();
        });

        actionChain.EnqueueChain();

        if (player.UnderLifestoneProtection)
            player.LifestoneProtectionDispel();
    }

    public static float Rotate(this Player player, Position pos)//WorldObject target)
    {
        if (pos is null) return 0f;

        player.TurnTo(pos);
        float angle = player.GetAngle(pos);
        float rotateDelay = player.GetRotateDelay(angle);
        ActionChain actionChain = new ActionChain();
        actionChain.AddDelaySeconds(rotateDelay);
        actionChain.AddAction(player, delegate
        {
            if (pos != null)
            {
                Vector3 self = player.Location.ToGlobal();
                Vector3 target2 = pos.ToGlobal();
                Vector3 direction = player.GetDirection(self, target2);
                player.Location.Rotate(direction);
            }
        });
        actionChain.EnqueueChain();
        return rotateDelay;
    }

    public static float GetAngle(this Creature creature, Position pos)
    {
        Vector3 currentDir = creature.Location.GetCurrentDir();
        Vector3 zero = Vector3.Zero;

        //If going from indoors to outdoors?
        //zero = ((creature.Location.Indoors != target.Location.Indoors) ? GetDirection(creature.Location.Pos, target.Location.Pos) : GetDirection(creature.Location.ToGlobal(), target.Location.ToGlobal()));
        zero = creature.GetDirection(creature.Location.ToGlobal(), ACE.Server.Entity.PositionExtensions.ToGlobal(pos));
        zero.Z = 0f;
        zero = Vector3.Normalize(zero);
        return Creature.GetAngle(currentDir, zero);
    }

    public static bool TargetInRange(this Player player, Position target)
    {
        // 2d or 3d distance?
        var dist = player.Location.DistanceTo(target);

        var maxRange = player.GetMaxMissileRange();

        return dist <= maxRange;
    }

    public static bool IsFacing(this Player player, Position target)
    {
        if (target == null) return false;

        var angle = player.GetAngle(target);
        //var dist = Math.Max(0, player.GetDistance(target));
        var dist = Math.Max(0, player.Location.DistanceTo(target));

        // rotation accuracy?
        var threshold = 5.0f;

        var minDist = 10.0f;

        if (dist < minDist)
            threshold += (minDist - dist) * 1.5f;

        return angle < threshold;
    }

    public static Vector3 GetAimVelocity(this Player player, Position target, float projectileSpeed)
    {
        var crossLandblock = player.Location.Landblock != target.Landblock;

        // eye level -> target point
        var origin = crossLandblock ? player.Location.ToGlobal(false) : player.Location.Pos;
        origin.Z += player.Height * Player.ProjSpawnHeight;

        var dest = crossLandblock ? target.ToGlobal(false) : target.Pos;
        dest.Z += 0;//target.Height / GetAimHeight(target);

        var dir = Vector3.Normalize(dest - origin);

        var velocity = player.GetProjectileVelocity(target, origin, dir, dest, projectileSpeed, out float time);

        return velocity;
    }

    public static Vector3 CalculateProjectileVelocity(this Player player, Vector3 localOrigin, Position target, float projectileSpeed, out Vector3 origin, out Quaternion rotation)
    {
        var sourceLoc = player.PhysicsObj.Position.ACEPosition();
        var targetLoc = target;//target.ACEPosition();

        var crossLandblock = sourceLoc.Landblock != targetLoc.Landblock;

        var startPos = crossLandblock ? sourceLoc.ToGlobal(false) : sourceLoc.Pos;
        var endPos = crossLandblock ? targetLoc.ToGlobal(false) : targetLoc.Pos;

        var dir = Vector3.Normalize(endPos - startPos);

        var angle = Math.Atan2(-dir.X, dir.Y);

        rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)angle);

        origin = sourceLoc.Pos + Vector3.Transform(localOrigin, rotation);

        startPos += Vector3.Transform(localOrigin, rotation);
        //Add some offset?
        endPos.Z += 0;  //target.Height / GetAimHeight(target);

        var velocity = player.GetProjectileVelocity(target, startPos, dir, endPos, projectileSpeed, out float time);

        return velocity;
    }

    public static Vector3 GetProjectileVelocity(this Player player, Position target, Vector3 origin, Vector3 dir, Vector3 dest, float speed, out float time, bool useGravity = true)
    {
        time = 0.0f;
        Vector3 s0;
        float t0;

        var gravity = useGravity ? -PhysicsGlobals.Gravity : 0.00001f;

        //Assume no velocity for target
        //var targetVelocity = target.PhysicsObj.CachedVelocity;
        //if (!targetVelocity.Equals(Vector3.Zero))
        //{
        //    if (this is Player player && !player.GetCharacterOption(CharacterOption.LeadMissileTargets))
        //    {
        //        // fall through
        //    }
        //    else
        //    {
        //        // use movement quartic solver
        //        if (!PropertyManager.GetBool("trajectory_alt_solver").Item)
        //        {
        //            var numSolutions = Trajectory.solve_ballistic_arc(origin, speed, dest, targetVelocity, gravity, out s0, out _, out time);

        //            if (numSolutions > 0)
        //                return s0;
        //        }
        //        else
        //            return Trajectory2.CalculateTrajectory(origin, dest, targetVelocity, speed, useGravity);
        //    }
        //}

        // use stationary solver
        if (!PropertyManager.GetBool("trajectory_alt_solver").Item)
        {
            Trajectory.solve_ballistic_arc(origin, speed, dest, gravity, out s0, out _, out t0, out _);

            time = t0;
            return s0;
        }
        else
            return Trajectory2.CalculateTrajectory(origin, dest, Vector3.Zero, speed, useGravity);
    }

    public static WorldObject LaunchProjectile(this Player player, WorldObject weapon, WorldObject ammo, Position target, Vector3 origin, Quaternion orientation, Vector3 velocity)
    {
        //Player player = this as Player;
        if (!velocity.IsValid())
        {
            player?.SendWeenieError(WeenieError.YourAttackMisfired);
            return null;
        }

        WorldObject worldObject = WorldObjectFactory.CreateNewWorldObject(ammo.WeenieClassId);
        worldObject.ProjectileSource = player;
        //worldObject.ProjectileTarget = target;
        worldObject.ProjectileLauncher = weapon;
        worldObject.ProjectileAmmo = ammo;
        worldObject.Location = new ACE.Entity.Position(player.Location);
        worldObject.Location.Pos = origin;
        worldObject.Location.Rotation = orientation;
        worldObject.SetProjectilePhysicsState(velocity);
        //worldObject.SetProjectilePhysicsState(null);
        if (!LandblockManager.AddObject(worldObject) || worldObject.PhysicsObj == null)
        {
            if (!worldObject.HitMsg)
            {
                player?.Session.Network.EnqueueSend(new GameMessageSystemChat("Your missile attack hit the environment.", ChatMessageType.Broadcast));
            }

            worldObject.Destroy();
            return null;
        }

        if (!player.IsProjectileVisible(worldObject))
        {
            worldObject.OnCollideEnvironment();
            worldObject.Destroy();
            return null;
        }

        PlayerKillerStatus value = player?.PlayerKillerStatus ?? PlayerKillerStatus.Unprotected;
        worldObject.EnqueueBroadcast(new GameMessagePublicUpdatePropertyInt(worldObject, PropertyInt.PlayerKillerStatus, (int)value));
        worldObject.EnqueueBroadcast(new GameMessageScript(worldObject.Guid, PlayScript.Launch, 0f));
        return worldObject;
    }
    /// <summary>
    /// Sets the physics state for a launched projectile
    /// </summary>
    public static void SetProjectilePhysicsState(this WorldObject obj, Vector3 velocity)
    {
        obj.InitPhysicsObj();

        obj.ReportCollisions = true;
        obj.Missile = true;
        obj.AlignPath = true;
        obj.PathClipped = true;
        obj.Ethereal = false;
        obj.IgnoreCollisions = false;

        var pos = obj.Location.Pos;
        var rotation = obj.Location.Rotation;
        obj.PhysicsObj.Position.Frame.Origin = pos;
        obj.PhysicsObj.Position.Frame.Orientation = rotation;

        if (obj.HasMissileFlightPlacement)
            obj.Placement = ACE.Entity.Enum.Placement.MissileFlight;
        else
            obj.Placement = null;

        obj.CurrentMotionState = null;

        obj.PhysicsObj.Velocity = velocity;
        //obj.PhysicsObj.ProjectileTarget = target.PhysicsObj;

        // Projectiles with RotationSpeed get omega values and "align path" turned off which
        // creates the nice swirling animation
        if ((obj.RotationSpeed ?? 0) != 0)
        {
            obj.AlignPath = false;
            obj.PhysicsObj.Omega = new Vector3((float)(Math.PI * 2 * obj.RotationSpeed), 0, 0);
        }

        obj.PhysicsObj.set_active(true);
    }


    //Used by spells?
    public static void SetProjectilePhysicsState(this WorldObject source, WorldObject target, bool useGravity = true)
    {
        if (source.PhysicsObj is null)
            source.InitPhysicsObj();

        if (useGravity)
            source.GravityStatus = true;

        source.CurrentMotionState = null;
        source.Placement = null;

        // TODO: Physics description timestamps (sequence numbers) don't seem to be getting updated

        //Console.WriteLine("SpellProjectile PhysicsState: " + PhysicsObj.State);

        var pos = source.Location.Pos;
        var rotation = source.Location.Rotation;
        source.PhysicsObj.Position.Frame.Origin = pos;
        source.PhysicsObj.Position.Frame.Orientation = rotation;

        var velocity = source.Velocity;
        //velocity = Vector3.Transform(velocity, Matrix4x4.Transpose(Matrix4x4.CreateFromQuaternion(rotation)));
        source.PhysicsObj.Velocity = velocity;

        if (target != null)
            source.PhysicsObj.ProjectileTarget = target.PhysicsObj;

        source.PhysicsObj.set_active(true);
    }



    public static WorldObject GetSelected(this Player player)
    {
        var objectId = ObjectGuid.Invalid;

        if (player.HealthQueryTarget.HasValue)
            objectId = new ObjectGuid(player.HealthQueryTarget.Value);
        else if (player.ManaQueryTarget.HasValue)
            objectId = new ObjectGuid(player.ManaQueryTarget.Value);
        else if (player.CurrentAppraisalTarget.HasValue)
            objectId = new ObjectGuid(player.CurrentAppraisalTarget.Value);

        if (objectId == ObjectGuid.Invalid)
            ChatPacket.SendServerMessage(player.Session, "Delete failed. Please identify the object you wish to delete first.", ChatMessageType.Broadcast);

        var wo = player.FindObject(objectId.Full, Player.SearchLocations.Everywhere, out _, out Container rootOwner, out bool wasEquipped);
        return wo;
    }

}
