namespace ACE.Shared.Helpers;

public static class ToggleExtensions
{
    //Todo: maybe event handlers?

    public static bool Toggle(this WorldObject wo, PropertyBool prop)
    {
        var value = !(wo.GetProperty(prop) ?? false);
        wo.SetProperty(prop, value);

        return value;
    }
    public static bool Toggle(this WorldObject wo, FakeBool prop)
    {
        var value = !(wo.GetProperty(prop) ?? false);
        wo.SetProperty(prop, value);

        return value;
    }
}
