namespace ExtendACE;

public enum PropertyAttributeEx : ushort
{
}
public enum PropertyAttribute2ndEx : ushort
{
}
public enum PropertyBoolEx : ushort
{
}
public enum PropertyDataIDEx : ushort
{
}
public enum PropertyInstanceIDEx : ushort
{
}
public enum PropertyIntEx : ushort
{
}
public enum PropertyInt64Ex : ushort
{
}
public enum PropertyStringEx : ushort
{
}
public enum PropertyFloatEx : ushort
{
    Cull = 10000,
}


public static class EnumHelpers
{
    //public static double? GetProperty(this WorldObject wo, PropertyAttributeEx property) => wo.GetProperty((PropertyAttribute)property);
    //public static void SetProperty(this WorldObject wo, PropertyAttributeEx property, double value) => wo.SetProperty((PropertyAttribute)property, value);
    //public static void IncProperty(this WorldObject wo, PropertyAttributeEx property, double value) => wo.IncProperty((PropertyAttribute)property, value);
    //public static double? GetProperty(this WorldObject wo, PropertyAttribute2ndEx property) => wo.GetProperty((PropertyAttribute2nd)property);
    //public static void SetProperty(this WorldObject wo, PropertyAttribute2ndEx property, double value) => wo.SetProperty((PropertyAttribute2nd)property, value);
    //public static void IncProperty(this WorldObject wo, PropertyAttribute2ndEx property, double value) => wo.IncProperty((PropertyAttribute2nd)property, value);
    public static uint? GetProperty(this WorldObject wo, PropertyDataIDEx property) => wo.GetProperty((PropertyDataId)property);
    public static void SetProperty(this WorldObject wo, PropertyDataIDEx property, uint value) => wo.SetProperty((PropertyDataId)property, value);
    //public static void IncProperty(this WorldObject wo, PropertyDataIDEx property, uint value) => wo.IncProperty((PropertyDataId)property, value);
    public static uint? GetProperty(this WorldObject wo, PropertyInstanceIDEx property) => wo.GetProperty((PropertyInstanceId)property);
    public static void SetProperty(this WorldObject wo, PropertyInstanceIDEx property, uint value) => wo.SetProperty((PropertyInstanceId)property, value);
    //public static void IncProperty(this WorldObject wo, PropertyInstanceIDEx property, uint value) => wo.IncProperty((PropertyInstanceId)property, value);
    public static int? GetProperty(this WorldObject wo, PropertyIntEx property) => wo.GetProperty((PropertyInt)property);
    public static void SetProperty(this WorldObject wo, PropertyIntEx property, int value) => wo.SetProperty((PropertyInt)property, value);
    public static void IncProperty(this WorldObject wo, PropertyIntEx property, int value)
    {
        var valueOrDefault = wo.GetProperty(property).GetValueOrDefault();
        wo.SetProperty(property, valueOrDefault + value);
    }
    public static long? GetProperty(this WorldObject wo, PropertyInt64Ex property) => wo.GetProperty((PropertyInt64)property);
    public static void SetProperty(this WorldObject wo, PropertyInt64Ex property, long value) => wo.SetProperty((PropertyInt64)property, value);
    public static void IncProperty(this WorldObject wo, PropertyInt64Ex property, long value)
    {
        var valueOrDefault = wo.GetProperty(property).GetValueOrDefault();
        wo.SetProperty(property, valueOrDefault + value);
    }
    public static string? GetProperty(this WorldObject wo, PropertyStringEx property) => wo.GetProperty((PropertyString)property);
    public static void SetProperty(this WorldObject wo, PropertyStringEx property, string value) => wo.SetProperty((PropertyString)property, value);
    public static void IncProperty(this WorldObject wo, PropertyStringEx property, string value)
    {
        var valueOrDefault = wo.GetProperty(property) ?? "";
        wo.SetProperty(property, valueOrDefault + value);
    }
    public static double? GetProperty(this WorldObject wo, PropertyFloatEx property) => wo.GetProperty((PropertyFloat)property);
    public static void SetProperty(this WorldObject wo, PropertyFloatEx property, double value) => wo.SetProperty((PropertyFloat)property, value);
    public static void IncProperty(this WorldObject wo, PropertyFloatEx property, double value) => wo.IncProperty((PropertyFloat)property, value);

    public static void ScaleProperty(this WorldObject wo, PropertyInt property, float amount) => wo.SetProperty(property, (int)(amount * wo.GetProperty(property) ?? 0));
    public static void ScaleProperty(this WorldObject wo, PropertyFloat property, float amount) => wo.SetProperty(property, (double)(amount * wo.GetProperty(property) ?? 0));
    public static void ScaleProperty(this WorldObject wo, PropertyInt64 property, float amount) => wo.SetProperty(property, (long)(amount * wo.GetProperty(property) ?? 0));

    public static void ScaleAttributeBase(this Creature wo, float amount, params PropertyAttribute[] properties) =>
        Array.ForEach<PropertyAttribute>(properties, (property) =>
        {
            if (property != PropertyAttribute.Undef)
                wo.Attributes[property].StartingValue = (uint)(wo.Attributes[property].StartingValue * amount);
        });
    public static void ScaleAttributeBase(this Creature wo, float amount, params PropertyAttribute2nd[] properties) =>
        Array.ForEach<PropertyAttribute2nd>(properties, (property) =>
        {
            if (property != PropertyAttribute2nd.Undef)
                wo.Vitals[property].StartingValue = (uint)(wo.Vitals[property].StartingValue * amount);
        });

    //Indexer approach that could be used in ACE.  Helper may be an option in the future
    //public bool? this[PropertyBool key]
    //{
    //    get => GetProperty(key);
    //    set => SetProperty(key, value.Value);
    //}
}

