using ACE.Server.Network;

namespace ACE.Shared.Helpers;

public static class PositionExtensions
{
    //    public static Position Translate(this Position p, float distanceInFront, float radians = 0)
    //    {
    //        //Add rotation?
    //        //Quaternion.CreateFromYawPitchRoll()
    //        var pos = new Position();
    //        //pos.landblockId.Raw = p.LandblockId.Raw;
    //        pos.Rotation = p.Rotation;

    //        // Create a Quaternion representing the rotation
    //        Quaternion rotationQuaternion = Quaternion.CreateFromYawPitchRoll(radians, 0, 0);

    //        // Multiply a unit vector by distance/rotation
    //        Vector3 rotatedPosition = Vector3.Transform(Vector3.One * distanceInFront, rotationQuaternion);

    //        // Add the rotated position to the original position to obtain the translated position
    //        pos.Pos = p.Pos + rotatedPosition;

    //        return pos;
    //        //p.FindZ()
    ////        return new Position(p.LandblockId.Raw, p.PositionX + num2, p.PositionY + num3, p.PositionZ + num4, 0f, 0f, rotationZ, rotationW);
    //    }


    /// <summary>
    /// Try to parse a location string as a Position
    /// </summary>
    public static bool TryParsePosition(this string position, out Position p)
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

            p = new Position(cell, positionData[0], positionData[1], positionData[2], positionData[4], positionData[5], positionData[6], positionData[3]);
        }
        catch(Exception ex)
        {
            return false;
        }

        return true;
    }
}