using Tinkering.Aetheria;
using Tinkering.Bank;
using Tinkering.Floor;
using Tinkering.Hardcore;
using Tinkering.Loot;
using Tinkering.MeleeMagic;
using Tinkering.Offline;
using Tinkering.Speedrun;

namespace Tinkering;

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
        };

    public AetheriaSettings Aetheria { get; set; } = new();
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
    AetheriaLoot,
    Chaos,
}
