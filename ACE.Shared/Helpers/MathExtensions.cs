using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Shared.Helpers;
public static class MathExtensions
{
    //Convenience for radian conversions
    public static float ToRadians(this float angle)
    {
        return (float)(Math.PI / 180.0f * angle);
    }

    public static float ToDegrees(this float rads)
    {
        return (float)(180.0f / Math.PI * rads);
    }

    public static float Clamp(this float f, float min, float max)
    {
        if (f < min)
            f = min;
        if (f > max)
            f = max;
        return f;
    }
}
