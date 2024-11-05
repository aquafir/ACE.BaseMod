
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

    public Dictionary<uint, CustomRecipeEx> Overrides2 { get; set; } = new()
    {
        //Ivory randomizes
        [30092] = new(CustomUseHandler.Launch, Animations: [MotionCommand.Mock, MotionCommand.Nod, MotionCommand.Point, MotionCommand.PointDown]),
        [30093] = new(CustomUseHandler.RandomColor, .3),
    };


    public static MotionCommand[] value = new[] { MotionCommand.ClapHands };
}

public enum CustomUseHandler
{
    RandomColor,
    RandomElement,
    Launch,
}

public record struct CustomRecipe(CustomUseHandler Handler, double DestroysSource = 0, double Success = 1);

public record struct CustomRecipeEx(
    CustomUseHandler Handler,
    double DestroysSource = 0,
    double Success = 1,
    bool NotBusy = true,
    bool UseOnSelf = true,
    CombatMode Stance = CombatMode.NonCombat,
    List<MotionCommand> Animations = null
    );