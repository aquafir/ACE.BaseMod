using static AngouriMath.Entity;
using static AngouriMath.Entity.Number;
using Complex = AngouriMath.Entity.Number.Complex;

namespace Balance;

public static class AngouriHelpers
{
    /// <summary>
    /// Terrrrrible way of shortening formulas by replacing "piecewise" with "P" and "provided" with "if"
    /// </summary>
    /// <param name="formula"></param>
    /// <returns></returns>
    public static string CompileFriendly(this string formula) => formula.Replace(" if ", " provided ").Replace("P(", "piecewise(");

    /* Generated with:
//Providing variable names
    var t = $$"""
    public static bool TryGetFunction<T1, TResult>(this string formula, out Func<T1, TResult> func, params string[] names)
    {
        func = null;

        if (names.Length != 1)
            return false;

        try
        {
            func = formula.CompileFriendly().Compile<T1, TResult>(names[0]);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
""";
//With type/Name IEnumerable
t = $$"""
    public static bool TryGetFunction<T1, TResult>(this string formula, out Func<T1, TResult> func, List<(Type, Variable)> typesAndNames)
    {
        try
        {
            func = formula.Compile<Func<T1, TResult>>(new CompilationProtocol(), typeof(TResult), typesAndNames);
            return true;
        }
        catch (Exception ex)
        {
            func = null;
            return false;
        }
    }
""";
//Enum of Angouri-supported types
t = $$"""
    public static bool TryGetFunction<T1, TResult>(this string formula, out Func<T1, TResult> func, List<(MType, Variable)> typesAndNames) =>
        formula.TryGetFunction<T1, TResult>(out func, typesAndNames.Select(x => (x.Item1.GetAngouriType(), x.Item2)).ToList());
""";

var sb = new StringBuilder();
for(var i = 1; i < 9; i++) {
    sb.AppendLine(t);
    t = Regex.Replace(t, "T" + i, $"T{i}, T{i+1}");
    t = Regex.Replace(t, "!= " + i, "!= " + (i+1));
    t = Regex.Replace(t, @$"names\[{i-1}\]", $"names[{i-1}], names[{i}]");
}

sb.ToString()
    */
    //Variable names in parameters
    private static bool TryGetFunction<T1, TResult>(this string formula, out Func<T1, TResult> func, params string[] names)
    {
        func = null;

        if (names.Length != 1)
            return false;

        try
        {
            func = formula.CompileFriendly().Compile<T1, TResult>(names[0]);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, TResult>(this string formula, out Func<T1, T2, TResult> func, params string[] names)
    {
        func = null;

        if (names.Length != 2)
            return false;

        try
        {
            func = formula.CompileFriendly().Compile<T1, T2, TResult>(names[0], names[1]);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, TResult>(this string formula, out Func<T1, T2, T3, TResult> func, params string[] names)
    {
        func = null;

        if (names.Length != 3)
            return false;

        try
        {
            func = formula.CompileFriendly().Compile<T1, T2, T3, TResult>(names[0], names[1], names[2]);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, TResult>(this string formula, out Func<T1, T2, T3, T4, TResult> func, params string[] names)
    {
        func = null;

        if (names.Length != 4)
            return false;

        try
        {
            func = formula.CompileFriendly().Compile<T1, T2, T3, T4, TResult>(names[0], names[1], names[2], names[3]);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, T5, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, TResult> func, params string[] names)
    {
        func = null;

        if (names.Length != 5)
            return false;

        try
        {
            func = formula.CompileFriendly().Compile<T1, T2, T3, T4, T5, TResult>(names[0], names[1], names[2], names[3], names[4]);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, T5, T6, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, T6, TResult> func, params string[] names)
    {
        func = null;

        if (names.Length != 6)
            return false;

        try
        {
            func = formula.CompileFriendly().Compile<T1, T2, T3, T4, T5, T6, TResult>(names[0], names[1], names[2], names[3], names[4], names[5]);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, params string[] names)
    {
        func = null;

        if (names.Length != 7)
            return false;

        try
        {
            func = formula.CompileFriendly().Compile<T1, T2, T3, T4, T5, T6, T7, TResult>(names[0], names[1], names[2], names[3], names[4], names[5], names[6]);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, params string[] names)
    {
        func = null;

        if (names.Length != 8)
            return false;

        try
        {
            func = formula.CompileFriendly().Compile<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(names[0], names[1], names[2], names[3], names[4], names[5], names[6], names[7]);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    //Variable names through array: (typeof(double), "x"), ...
    private static bool TryGetFunction<T1, TResult>(this string formula, out Func<T1, TResult> func, List<(Type, Variable)> typesAndNames)
    {
        try
        {
            func = formula.CompileFriendly().Compile<Func<T1, TResult>>(new CompilationProtocol(), typeof(TResult), typesAndNames);
            return true;
        }
        catch (Exception ex)
        {
            func = null;
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, TResult>(this string formula, out Func<T1, T2, TResult> func, List<(Type, Variable)> typesAndNames)
    {
        try
        {
            func = formula.CompileFriendly().Compile<Func<T1, T2, TResult>>(new CompilationProtocol(), typeof(TResult), typesAndNames);
            return true;
        }
        catch (Exception ex)
        {
            func = null;
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, TResult>(this string formula, out Func<T1, T2, T3, TResult> func, List<(Type, Variable)> typesAndNames)
    {
        try
        {
            func = formula.CompileFriendly().Compile<Func<T1, T2, T3, TResult>>(new CompilationProtocol(), typeof(TResult), typesAndNames);
            return true;
        }
        catch (Exception ex)
        {
            func = null;
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, TResult>(this string formula, out Func<T1, T2, T3, T4, TResult> func, List<(Type, Variable)> typesAndNames)
    {
        try
        {
            func = formula.CompileFriendly().Compile<Func<T1, T2, T3, T4, TResult>>(new CompilationProtocol(), typeof(TResult), typesAndNames);
            return true;
        }
        catch (Exception ex)
        {
            func = null;
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, T5, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, TResult> func, List<(Type, Variable)> typesAndNames)
    {
        try
        {
            func = formula.CompileFriendly().Compile<Func<T1, T2, T3, T4, T5, TResult>>(new CompilationProtocol(), typeof(TResult), typesAndNames);
            return true;
        }
        catch (Exception ex)
        {
            func = null;
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, T5, T6, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, T6, TResult> func, List<(Type, Variable)> typesAndNames)
    {
        try
        {
            func = formula.CompileFriendly().Compile<Func<T1, T2, T3, T4, T5, T6, TResult>>(new CompilationProtocol(), typeof(TResult), typesAndNames);
            return true;
        }
        catch (Exception ex)
        {
            func = null;
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, List<(Type, Variable)> typesAndNames)
    {
        try
        {
            func = formula.CompileFriendly().Compile<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>(new CompilationProtocol(), typeof(TResult), typesAndNames);
            return true;
        }
        catch (Exception ex)
        {
            func = null;
            return false;
        }
    }
    private static bool TryGetFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, List<(Type, Variable)> typesAndNames)
    {
        try
        {
            func = formula.CompileFriendly().Compile<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(new CompilationProtocol(), typeof(TResult), typesAndNames);
            return true;
        }
        catch (Exception ex)
        {
            func = null;
            return false;
        }
    }
    //Use enum of Angouri-supported types
    public static bool TryGetFunction<T1, TResult>(this string formula, out Func<T1, TResult> func, List<(MType, Variable)> typesAndNames) =>
        formula.TryGetFunction(out func, typesAndNames.Select(x => (x.Item1.GetAngouriType(), x.Item2)).ToList());
    public static bool TryGetFunction<T1, T2, TResult>(this string formula, out Func<T1, T2, TResult> func, List<(MType, Variable)> typesAndNames) =>
        formula.TryGetFunction(out func, typesAndNames.Select(x => (x.Item1.GetAngouriType(), x.Item2)).ToList());
    public static bool TryGetFunction<T1, T2, T3, TResult>(this string formula, out Func<T1, T2, T3, TResult> func, List<(MType, Variable)> typesAndNames) =>
        formula.TryGetFunction(out func, typesAndNames.Select(x => (x.Item1.GetAngouriType(), x.Item2)).ToList());
    public static bool TryGetFunction<T1, T2, T3, T4, TResult>(this string formula, out Func<T1, T2, T3, T4, TResult> func, List<(MType, Variable)> typesAndNames) =>
        formula.TryGetFunction(out func, typesAndNames.Select(x => (x.Item1.GetAngouriType(), x.Item2)).ToList());
    public static bool TryGetFunction<T1, T2, T3, T4, T5, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, TResult> func, List<(MType, Variable)> typesAndNames) =>
        formula.TryGetFunction(out func, typesAndNames.Select(x => (x.Item1.GetAngouriType(), x.Item2)).ToList());
    public static bool TryGetFunction<T1, T2, T3, T4, T5, T6, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, T6, TResult> func, List<(MType, Variable)> typesAndNames) =>
        formula.TryGetFunction(out func, typesAndNames.Select(x => (x.Item1.GetAngouriType(), x.Item2)).ToList());
    public static bool TryGetFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, List<(MType, Variable)> typesAndNames) =>
        formula.TryGetFunction(out func, typesAndNames.Select(x => (x.Item1.GetAngouriType(), x.Item2)).ToList());
    public static bool TryGetFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this string formula, out Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, List<(MType, Variable)> typesAndNames) =>
        formula.TryGetFunction(out func, typesAndNames.Select(x => (x.Item1.GetAngouriType(), x.Item2)).ToList());
    static Type GetAngouriType(this MType angouriType) => angouriType switch
    {
        MType.Bool => typeof(bool),
        MType.Int => typeof(int),
        MType.Long => typeof(long),
        MType.Float => typeof(float),
        MType.Double => typeof(double),
        MType.Complex => typeof(Complex),
        MType.BigInt => typeof(BigInteger),
        _ => throw new NotImplementedException()
    };

    /// <summary>
    /// Converts a serializable variable dictionary to the tuple used by AngouriMath
    /// </summary>
    public static List<(MType, Variable)> TypesAndNames(this Dictionary<string, MType> variables) => variables.Select(x => (x.Value, (Variable)x.Key)).ToList();
}

/// <summary>
/// Types with built-in AngouriMath support
/// </summary>
public enum MType
{
    Bool,
    Int,
    Long,
    Float,
    Double,
    Complex,
    BigInt
};