namespace Tower;
public class AetheriaSettings
{
    public PropertyDataId AetheriaLootGroupProperty { get; set; } = (PropertyDataId)49999;
    public Dictionary<uint, AetheriaDrop> Groups { get; set; } = new()
    {
        [0] = new(.01f, 2, 3, AetheriaColor.Blue),
        [1] = new(.01f, 2, 3, AetheriaColor.Yellow),
        [2] = new(.01f, 2, 3, AetheriaColor.Red),
        [3] = new(1f, 0, 10, AetheriaColor.Blue | AetheriaColor.Yellow | AetheriaColor.Red),
    };
}

public record struct AetheriaDrop(float Odds = .01f, int LevelMin = 1, int LevelMax = 5, AetheriaColor Color = AetheriaColor.Blue);