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


public static class Pattern
{
    public static (float x, float y) SpiralOffset(int step, double radius = 2, double growth = 3, double angleChange = Math.PI / 8)
    {
        var t = step * angleChange;

        // Parametric equations for the spiral
        var x = (radius + growth * t) * Math.Cos(t);
        var y = (radius + growth * t) * Math.Sin(t);
        return ((float)x, (float)y);
    }
}
