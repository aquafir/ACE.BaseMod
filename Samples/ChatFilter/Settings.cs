namespace ChatFilter;

public class Settings
{
    // Your settings here

    //Users unaware they're muted
    public bool ShadowBan { get; set; }
    public PropertyBool ShadowBanPropery => (PropertyBool)12345;
}