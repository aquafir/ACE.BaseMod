namespace ACE.Shared.Helpers;

public static class PropertyExtensions
{
    public static double? Normalize(this bool? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this int? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this long? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this ulong? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this uint? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this float? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this double? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;

    public static double? Normalize(this bool value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this int value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this long value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this ulong value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this double value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this float value) => (double?)Convert.ToDouble(value);
    public static double? Normalize(this uint value) => (double?)Convert.ToDouble(value);


    public static void IncProp(this WorldObject wo, PropertyFloat property, double value) => wo.IncProperty(property, value);
    public static void IncProp(this WorldObject wo, PropertyInt property, int value) => wo.SetProperty(property, value + (wo.GetProperty(property) ?? 0));
    public static void IncProp(this WorldObject wo, PropertyInt64 property, long value) => wo.SetProperty(property, value + (wo.GetProperty(property) ?? 0));
    public static void ScaleProp(this WorldObject wo, PropertyFloat property, double scale) => wo.SetProperty(property, (scale * (wo.GetProperty(property) ?? 0)));
    public static void ScaleProp(this WorldObject wo, PropertyInt property, double scale) => wo.SetProperty(property, (int)(scale * (wo.GetProperty(property) ?? 0)));
    public static void ScaleProp(this WorldObject wo, PropertyInt64 property, double scale) => wo.SetProperty(property, (long)(scale * (wo.GetProperty(property) ?? 0)));

    public static void IncProp(this WorldObject wo, FakeFloat property, double value) => wo.SetProperty(property, value + (wo.GetProperty(property) ?? 0));
    public static void IncProp(this WorldObject wo, FakeInt property, int value) => wo.SetProperty(property, value + (wo.GetProperty(property) ?? 0));
    public static void IncProp(this WorldObject wo, FakeInt64 property, long value) => wo.SetProperty(property, value + (wo.GetProperty(property) ?? 0));
    public static void ScaleProp(this WorldObject wo, FakeFloat property, double scale) => wo.SetProperty(property, (scale * (wo.GetProperty(property) ?? 0)));
    public static void ScaleProp(this WorldObject wo, FakeInt property, double scale) => wo.SetProperty(property, (int)(scale * (wo.GetProperty(property) ?? 0)));
    public static void ScaleProp(this WorldObject wo, FakeInt64 property, double scale) => wo.SetProperty(property, (long)(scale * (wo.GetProperty(property) ?? 0)));


    //public static void SetPropBits(this WorldObject wo, PropertyInt property, bool bitValue = true) => wo.SetProperty(property, value + (wo.GetProperty(property) ?? 0));
}