using ACE.Server.Physics.Common;

#if !REALM
using Position = ACE.Entity.Position;
#endif

namespace ACE.Shared.Helpers;

public static class PositionExtensions
{
    const float PI = (float)Math.PI;

    /// <summary>
    /// Translate a new Position without changing the angle
    /// </summary>
    public static Position Shifted(this Position origin, Vector3 shift)
    {
        Entity.Position pos = new(origin.LandblockId.Raw, shift + origin.Pos, origin.Rotation);

#if REALM
        Position newPos = new(pos, origin.Instance);
        newPos.SetLandblockId(new LandblockId(newPos.GetCell()));
        return newPos;
#else
        return pos;
#endif
    }
    public static Position Shifted(this Position origin, float dx, float dy, float dz = 0) => origin.Shifted(new(dx, dy, dz));

    public static Position AdjustZ(this Position origin, float objScale = 1f) =>
        origin.SetPositionZ(origin.PositionZ + 0.005f * objScale);

    /// <summary>
    /// Returns a new Position rotated to a heading, yaw, and pitch in radians
    /// </summary>
    public static Position RotatedTo(this Position origin, float heading, float yaw = 0, float pitch = 0)
    {
        Quaternion rot = Quaternion.CreateFromYawPitchRoll(yaw, pitch, heading);
        Entity.Position pos = new(origin.LandblockId.Raw, origin.Pos, rot);

#if REALM
        return new(pos, origin.Instance);
#else
        return pos;
#endif
    }
    /// <summary>
    /// Returns a new Position rotated relate to its current heading by a number of radians
    /// </summary>
    public static Position RotatedBy(this Position origin, float heading, float yaw = 0, float pitch = 0)
    {
        //Working
        float qw = origin.RotationW; // north
        float qz = origin.RotationZ; // south
        var rotation = new Quaternion(0, 0, qz, qw) * Quaternion.CreateFromYawPitchRoll(yaw, pitch, heading);
        Entity.Position pos = new(origin.LandblockId.Raw, origin.Pos, rotation);
#if REALM
        return new(pos, origin.Instance);
#else
        return pos;
#endif
    }
    public static Position RotatedTo(this Position origin, Position target)
        => origin.RotatedTo(origin.GetAngle(target));
    public static Position RotatedTo(this WorldObject origin, WorldObject target)
        => origin.Location.RotatedTo(origin.GetAngle(target));


    /// <summary>
    /// Returns the 2D angle between current direction and position from an input target, rotated by -PI/2 to face it
    /// </summary>
    public static float GetAngle(this Position origin, Position target)
    {
        var indoors = origin.Indoors == target.Indoors;
        var a = indoors ? origin.ToGlobal() : origin.Pos;
        var b = indoors ? target.ToGlobal() : target.Pos;

        return a.GetAngle(b) - PI / 2;    //Sub Pi/2 to face North instead of East?
    }
    public static float GetAngleDegrees(this Position origin, Position target) => origin.GetAngle(target) * (180.0f / PI);
    public static float GetAngle(this WorldObject origin, WorldObject target)
        => origin.Location.GetAngle(target.Location);

    //Use GetOffset for now
    //public static Vector3 GetOffset3d(this Position origin, Position target)
    //{
    //    if (origin.Indoors == target.Indoors)
    //        return Vector3.Normalize(target.ToGlobal() - origin.ToGlobal());
    //    return Vector3.Normalize(target.Pos - origin.Pos);
    //}

    /// <summary>
    /// Returns the 2D angle between current direction and position from an input target
    /// </summary>
    public static Vector3 GetDirection(this Position origin, Position target)
    {
        if (origin.Indoors == target.Indoors)
            return origin.ToGlobal().GetDirection(target.ToGlobal());

        return origin.Pos.GetDirection(target.Pos);
    }
    /// <summary>
    /// Returns a normalized 2D vector from self to target
    /// </summary>
    public static Vector3 GetDirection(this Vector3 self, Vector3 target)
    {
        var target2D = new Vector3(self.X, self.Y, 0);
        var self2D = new Vector3(target.X, target.Y, 0);

        return Vector3.Normalize(target - self);
    }

    /// <summary>
    /// Returns the 2D angle between 2 vectors in radians.  
    /// Creature_Navigation method with normalized offset vectors and arccos fails over 180 degrees
    /// </summary>
    public static float GetAngle(this Vector3 a, Vector3 b)
    {
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;

        var rads = Math.Atan2(dy, dx);
        if (double.IsNaN(rads)) return 0.0f;

        return (float)rads;
    }
    /// <summary>
    /// Returns the 2D angle between 2 vectors
    /// </summary>
    public static float GetAngleDegrees(this Vector3 a, Vector3 b)
    {
        var rads = a.GetAngle(b);
        if (double.IsNaN(rads)) return 0.0f;

        var angle = rads * (180.0f / PI);
        return angle;
    }

    /// <summary>
    /// Try to parse a location string as a Position
    /// </summary>
    public static bool TryParsePosition(this string position, out Position p, uint instance = 0)
    {
        p = null;
        var parameters = position.Split(' ');

        try
        {
            uint cell;

            if (parameters[0].StartsWith("0x"))
            {
                string strippedcell = parameters[0].Substring(2);
                cell = (uint)int.Parse(strippedcell, System.Globalization.NumberStyles.HexNumber);
            }
            else
                cell = (uint)int.Parse(parameters[0], System.Globalization.NumberStyles.HexNumber);

            var positionData = new float[7];
            for (uint i = 0u; i < 7u; i++)
            {
                if (i > 2 && parameters.Length < 8)
                {
                    positionData[3] = 1;
                    positionData[4] = 0;
                    positionData[5] = 0;
                    positionData[6] = 0;
                    break;
                }

                if (!float.TryParse(parameters[i + 1].Trim(new Char[] { ' ', '[', ']' }), out var pos))
                    return false;

                positionData[i] = pos;
            }

#if REALM
            //TODO: Better way of defaulting this.  Currently meant to override
            p = new(cell, positionData[0], positionData[1], positionData[2], positionData[4], positionData[5], positionData[6], positionData[3],  instance);
#else
            p = new(cell, positionData[0], positionData[1], positionData[2], positionData[4], positionData[5], positionData[6], positionData[3]);
#endif
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// TODO: Figure this ACRealms stuff out
    /// </summary>
    public static bool TryTeleport(this Creature c, Position _newPosition, out SetPositionError result)
    {
        result = SetPositionError.OK;

        Position position = _newPosition.SetPositionZ(_newPosition.PositionZ + 0.005f * c.ObjScale.GetValueOrDefault(1f));
#if REALM
        if (c.Location.InstancedLandblock != position.InstancedLandblock)
        {
            //log.Error((object)$"{c.Name} tried to teleport from {c.Location} to a different landblock {instancedPosition}");
            result = SetPositionError.InvalidArguments;
            return false;
        }
#endif

        // force out of hotspots
        c.PhysicsObj.report_collision_end(forceEnd: true);

        // do the physics teleport
#if REALM
        SetPosition setPosition = new SetPosition(position.Instance);
        setPosition.Pos = new PhysicsPosition(position);
#else
        SetPosition setPosition = new SetPosition();
        setPosition.Pos = new ACE.Server.Physics.Common.Position(position);
#endif

        setPosition.Flags = SetPositionFlags.Placement | SetPositionFlags.Teleport | SetPositionFlags.Slide | SetPositionFlags.SendPositionEvent;

        result = c.PhysicsObj.SetPosition(setPosition);
        if (result != SetPositionError.OK)
            return false;

        // update ace location
        c.SyncLocation();

        // broadcast blip to new position
        c.SendUpdatePosition(adminMove: true);

        return true;
    }
    //May be needed no non-Realms?
    //public void SendUpdatePosition(bool adminMove = false)
    //{
    //    EnqueueBroadcast(new GameMessageUpdatePosition(this, adminMove));
    //    LastUpdatePosition = DateTime.UtcNow;
    //}


    /// <summary>
    /// Helper for compatibility with ACRealms
    /// </summary>
    public static Entity.Position SetPositionZ(this Entity.Position Position, float positionZ)
    {
        var pos = new Entity.Position(Position);
        pos.PositionZ = positionZ;
        return pos;
    }
}
