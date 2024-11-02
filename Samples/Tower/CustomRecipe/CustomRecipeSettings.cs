
namespace Tower;
public class CustomRecipeSettings
{
    public bool UseRecipeRequirements { get; set; } = true;
    public MotionCommand Animation { get; set; } = MotionCommand.ClapHands;

    /// <summary>
    /// Maps WCID to custom handling
    /// </summary>
    public Dictionary<uint, CustomRecipe> Overrides { get; set; } = new()
    {
        //Ivory randomizes
        [30092] = new(CustomUseHandler.RandomColor, 0, .2),
        [30093] = new(CustomUseHandler.RandomColor, .3),
    };
}

public enum CustomUseHandler
{
    RandomColor,
    RandomElement,
}

public record struct CustomRecipe(CustomUseHandler Handler, double DestroysSource = 0, double Success = 1);