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

    public OfflineProgressSettings OfflineProgress { get; set; } = new();
    public HardcoreSettings Hardcore { get; set; } = new();

    public BankSettings Bank { get; set; } = new();
    public LootSettings Loot { get; set; } = new();
    public SpeedrunSettings Speedrun { get; set; } = new();

    public FloorSettings Tower { get; set; } = new();
//    public List<TowerFloor> Floors { get; set; } = new()
//    {
////016C : MP non instance
////5369 : Housing non instance
////5950 : Base non instance
////5F44 : Jungle Base non instance
//        new("Floor 1", 0, 0x019E, 0x0159, 10),
//        new("Floor 2", 1, 0x0159, 0x013B, 20),
//        new("Floor 3", 2, 0x013B, 0x6345, 30),
//        new("Floor 4", 3, 0x6345, 0x009D, 40),
//        new("Floor 5", 4, 0x009D, 0x03A5, 50),
//        new("Floor 6", 5, 0x03A5, 0x5F44, 60),
//        new("Floor 7", 6, 0x01A3, 0x016A, 70),
//        new("Floor 7.5", 7, 0x016A, 0x0110, 80),
//        new("Floor 8", 8, 0x0110, 0x0010, 90),
//        new("Floor 9", 9, 0x0010, 0x013C, 100),
//        new("Floor 10", 10, 0x013C, 0x001A, 110),
//        new("Floor 11", 11, 0x001A, 0x002D, 120),
//        new("Floor 12", 12, 0x002D, 0x0000, 130),
//    };


    public MeleeMagicSettings MeleeMagic { get; set; } = new();
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

//public record struct TowerFloor(string Name, int Index, ushort Landblock, uint Level);
//public record struct BankItem(string Name, uint Id, int Prop);


