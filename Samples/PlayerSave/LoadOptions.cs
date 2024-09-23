namespace PlayerSave;

public class LoadOptions
{
    //Character options
    public bool IncludeContracts { get; set; } = true;
    public bool IncludeTitles { get; set; } = true;
    public bool IncludeSquelch { get; set; } = true;
    public bool IncludeSpellbar { get; set; } = true;
    public bool IncludeShortcuts { get; set; } = true;
    public bool IncludeQuests { get; set; } = true;
    public bool IncludeFriendList { get; set; } = true;
    public bool IncludeFillComp { get; set; } = true;

    //Biota options
    public bool IncludeEnchantments { get; set; } = false;
    public bool ResetAttribute { get; set; } = false;
    public bool ResetSkills { get; set; } = false;
    public bool ResetPositions { get; set; } = true;

    //Possession options
    public bool IncludeInventory { get; set; } = true;
    public bool IncludeWielded { get; set; } = true;

    public static PropertiesPosition DefaultPosition => new PropertiesPosition
    {
        //Marketplace
        ObjCellId = 0x016C01BC,
        PositionX = 49.206001f,
        PositionY = -31.934999f,
        PositionZ = 0.005000f,
        RotationW = 0.707107f,
        RotationX = 0.0f,
        RotationY = 0.0f,
        RotationZ = -0.707107f
    };

    //Todo: may remove and just use initial values for default
    public static LoadOptions Default => new LoadOptions
    {
        IncludeContracts = true,
        IncludeTitles = true,
        IncludeSquelch = true,
        IncludeSpellbar = true,
        IncludeShortcuts = true,
        IncludeQuests = true,
        IncludeFriendList = true,
        IncludeFillComp = true,
        IncludeInventory = true,
        IncludeWielded = true,
    };
}