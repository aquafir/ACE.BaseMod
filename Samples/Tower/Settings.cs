using Tower.Bank;
using Tower.Floor;
using Tower.Hardcore;
using Tower.Loot;
using Tower.MeleeMagic;
using Tower.Offline;
using Tower.Speedrun;

namespace Tower;

public class Settings
{
    public bool Verbose { get; set; } = true;
    public HashSet<Feature> Features { get; set; } = //Enum.GetValues<Feature>().ToHashSet();
        new()
        {
            //Feature.AutoBattle,
            Feature.AutoLoot,
            //Feature.Bank,
            Feature.Hardcore,
            Feature.MeleeMagic,
            //Feature.OfflineProgress,
            Feature.Speedrun,
        };

    public BankSettings Bank { get; set; } = new();
    public HardcoreSettings Hardcore { get; set; } = new();
    public LootSettings Loot { get; set; } = new();
    public MeleeMagicSettings MeleeMagic { get; set; } = new();
    public OfflineProgressSettings OfflineProgress { get; set; } = new();
    public SpeedrunSettings Speedrun { get; set; } = new();
    public FloorSettings Tower { get; set; } = new();
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
}
