namespace Expansion;

public static class Range
{
    public static IntRange Int(int min, int max) => new()
    {
        Min = min,
        Max = max
    };

    public static FloatRange Float(float min, float max) => new()
    {
        Min = min,
        Max = max
    };
}

public class IntRange
{
    public int Min { get; set; }
    public int Max { get; set; }

    public int Roll() => ThreadSafeRandom.Next(Min, Max);
}

public class FloatRange
{
    public float Min { get; set; }
    public float Max { get; set; }

    public double Roll() => ThreadSafeRandom.Next(Min, Max);
}