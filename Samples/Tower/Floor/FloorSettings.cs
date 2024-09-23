namespace Tower;
public class FloorSettings
{
    public float MaxXpBonus { get; set; } = 5;
    public float MaxXpBonusLevelRange { get; set; } = 30;

    public float MaxLootBonus { get; set; } = 2;
    public float MaxLootBonusLevelRange { get; set; } = 30;

    public List<TowerFloor> Floors { get; set; } = new()
    {
        //016C : MP non instance
        //5369 : Housing non instance
        //5950 : Base non instance
        //5F44 : Jungle Base non instance
        new("Floor 1", 0, 0x019E, 0x0159, 10),
        new("Floor 2", 1, 0x0159, 0x013B, 20),
        new("Floor 3", 2, 0x013B, 0x6345, 30),
        new("Floor 4", 3, 0x6345, 0x009D, 40),
        new("Floor 5", 4, 0x009D, 0x03A5, 50),
        new("Floor 6", 5, 0x03A5, 0x5F44, 60),
        new("Floor 7", 6, 0x01A3, 0x016A, 70),
        new("Floor 7.5", 7, 0x016A, 0x0110, 80),
        new("Floor 8", 8, 0x0110, 0x0010, 90),
        new("Floor 9", 9, 0x0010, 0x013C, 100),
        new("Floor 10", 10, 0x013C, 0x001A, 110),
        new("Floor 11", 11, 0x001A, 0x002D, 120),
        new("Floor 12", 12, 0x002D, 0x0000, 130),
    };

}

public record TowerFloor(string Name, int Index, ushort Landblock, ushort ExitLandblock, uint Level);
