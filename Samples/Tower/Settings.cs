namespace Tower;

public class Settings
{
    public bool Verbose { get; set; } = true;
    public HashSet<Feature> Features { get; set; } = //Enum.GetValues<Feature>().ToHashSet();
        new()
        {
            //Feature.AutoBattle,
            Feature.AetheriaLoot,
            Feature.AutoLoot,
            //Feature.Bank,
            Feature.Hardcore,
            Feature.MeleeMagic,
            //Feature.OfflineProgress,
            Feature.Speedrun,
            Feature.CustomRecipe,
        };

    public AetheriaSettings Aetheria { get; set; } = new();
    public BankSettings Bank { get; set; } = new();
    public HardcoreSettings Hardcore { get; set; } = new();
    public LootSettings Loot { get; set; } = new();
    public MeleeMagicSettings MeleeMagic { get; set; } = new();
    public OfflineProgressSettings OfflineProgress { get; set; } = new();
    public SpeedrunSettings Speedrun { get; set; } = new();
    public FloorSettings Tower { get; set; } = new();
    public PVPSettings PVP { get; set; } = new();
    public CustomRecipeSettings OnUse { get; set; } = new();
}


public enum Feature
{
    AutoLoot,
    Bank,
    Hardcore,
    MeleeMagic,
    OfflineProgress,
    Speedrun,
    //WIP
    FloorBonus,
    AutoBattle,
    AetheriaLoot,
    Chaos,
    PVP,
    CustomRecipe,
    MarketplaceOverride,
}
