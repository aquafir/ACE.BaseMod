namespace CustomLoot.Enums;

//!!!!Very important to avoid using properties ACE or other Mods uses!!!!
public enum FakeBool
{
    GrowthItem = 10000
}
public enum FakeDID
{
    Placeholder = 10000
}
public enum FakeFloat
{
    ItemXpBoost = 10000,
    ItemLeechHealth = 10001,
    ItemLeechMana = 10002,
    ItemLeechStamina = 10003,
}
public enum FakeIID
{
    PetDevice = 10000
}
public enum FakeInt
{
    LivingWeenieType = 10000,
    GrowthTier = 10001,
}
public enum FakeInt64
{
    Placeholder = 10000
}
public enum FakeString
{
    Placeholder = 10000
}


public static class FakePropertyHelper
{
    //public static void SetProperty(this WorldObject wo, FakePropertyInt64 property, long value) => wo.SetProperty(property.Prop(), value);
    public static void SetProperty(this WorldObject wo, FakeBool property, bool value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeBool property) => wo.RemoveProperty(property.Prop());
    public static bool? GetProperty(this WorldObject wo, FakeBool property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeInt property, int value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeInt property) => wo.RemoveProperty(property.Prop());
    public static int? GetProperty(this WorldObject wo, FakeInt property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeInt64 property, long value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeInt64 property) => wo.RemoveProperty(property.Prop());
    public static long? GetProperty(this WorldObject wo, FakeInt64 property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeFloat property, double value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeFloat property) => wo.RemoveProperty(property.Prop());
    public static double? GetProperty(this WorldObject wo, FakeFloat property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeString property, string value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeString property) => wo.RemoveProperty(property.Prop());
    public static string? GetProperty(this WorldObject wo, FakeString property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeDID property, uint value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeDID property) => wo.RemoveProperty(property.Prop());
    public static uint? GetProperty(this WorldObject wo, FakeDID property) => wo.GetProperty(property.Prop());
    public static void SetProperty(this WorldObject wo, FakeIID property, uint value) => wo.SetProperty(property.Prop(), value);
    public static void RemoveProperty(this WorldObject wo, FakeIID property) => wo.RemoveProperty(property.Prop());
    public static uint? GetProperty(this WorldObject wo, FakeIID property) => wo.GetProperty(property.Prop());
    public static PropertyBool Prop(this FakeBool prop) => (PropertyBool)prop;
    public static PropertyDataId Prop(this FakeDID prop) => (PropertyDataId)prop;
    public static PropertyFloat Prop(this FakeFloat prop) => (PropertyFloat)prop;
    public static PropertyInstanceId Prop(this FakeIID prop) => (PropertyInstanceId)prop;
    public static PropertyInt Prop(this FakeInt prop) => (PropertyInt)prop;
    public static PropertyInt64 Prop(this FakeInt64 prop) => (PropertyInt64)prop;
    public static PropertyString Prop(this FakeString prop) => (PropertyString)prop;
}
