﻿namespace ACE.Shared.Helpers;

public static class PropertyExtensions
{
    public static double? Normalize(this bool? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this int? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this long? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this uint? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this float? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this double? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;

    public static double? Normalize(this bool value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this int value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this long value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this double value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this float value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this uint value) => (double?)Convert.ToDouble(value);
}