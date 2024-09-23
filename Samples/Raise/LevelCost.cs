namespace Raise;

public class LevelCost
{
    public double C { get; set; }
    public double Rate { get; set; }
    public double Coefficient { get; set; }
    public int Offset { get; set; }
    public GrowthType GrowthType { get; set; }

    public LevelCost(double C = 100, double Rate = 2, double Coefficient = 2, int Offset = 0, GrowthType GrowthType = GrowthType.Exponential)
    {
        this.C = C;
        this.Rate = Rate;
        this.Coefficient = Coefficient;
        this.Offset = Offset;
        this.GrowthType = GrowthType;
    }

    /// <summary>
    /// Get the cost needed to go from x to y
    /// </summary>
    public long GetTotalCost(long x, long n)
    {
        n += Offset;
        return GrowthType switch
        {
            GrowthType.Polynomial => Enumerable.Range((int)x, (int)n).Sum(x => GetCost(x)),
            _ => GetTotalCost(x + n) - GetTotalCost(x),
        };
    }

    /// <summary>
    /// Get total cost needed to go from 1 to x
    /// </summary>
    public long GetTotalCost(long n)
    {
        n += Offset;
        return GrowthType switch
        {
            GrowthType.Linear => (long)(
                (n + 1) * C + Rate / 2 * n * (n + 1)
            ),
            GrowthType.Exponential => (long)(
                Rate == 1 ? C * (n + 1) : C * ((1 - Math.Pow(Rate, n + 1)) / (1 - Rate))
            ),
            //Computed, simplified case not easy without growth rate
            GrowthType.Polynomial => Enumerable.Range(0, (int)n + 1).Select(x => GetCost(x)).Sum(),
        };
    }

    /// <summary>
    /// Gets the cost at step n
    /// </summary>
    public long GetCost(long n)
    {
        n += Offset;
        return GrowthType switch
        {
            //C(n)=C​+n*r
            GrowthType.Linear => (long)(C + n * Rate),
            //C(n)=C*r^n
            GrowthType.Exponential => (long)(C * Math.Pow(Rate, n)),
            //C(n)=C+a*n^r
            GrowthType.Polynomial => (long)(C + Coefficient * Math.Pow(n, Rate)),
        };
    }

    ///// <summary>
    ///// Get the max quantity purchaseable for X from a starting step n
    ///// </summary>
    //public long Purchaseable(long X, long n) => GrowthType switch
    //    {
    //        GrowthType.Linear => throw new NotImplementedException(),
    //        GrowthType.Exponential => (long)Math.Floor(Math.Log(Math.Pow(Rate, (double)n) - (X * (1 - Rate) / C), Rate) - n),
    //        GrowthType.Polynomial => throw new NotImplementedException(),
    //    };

    ///// <summary>
    ///// Get the level from total spent X
    ///// </summary>
    //public long CurrentLevel(long X) => GrowthType switch
    //{
    //    GrowthType.Linear => throw new NotImplementedException(),
    //    GrowthType.Exponential => Purchaseable(X, 0),
    //    GrowthType.Polynomial => throw new NotImplementedException(),
    //};
}

public enum GrowthType
{
    Linear,
    Exponential,
    Polynomial,
}