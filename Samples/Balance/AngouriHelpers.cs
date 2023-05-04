namespace Balance;

public static class AngouriHelpers
{
    /// <summary>
    /// Terrrrrible way of shortening formulas by replacing "piecewise" with "P" and "provided" with "if"
    /// </summary>
    /// <param name="formula"></param>
    /// <returns></returns>
    public static string CompileFriendly(this string formula) => formula.Replace(" if ", " provided ").Replace("P(", "piecewise(");
}
