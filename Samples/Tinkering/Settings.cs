namespace Tinkering;

public class Settings
{
    // Your settings here
    public int MaxTries { get; set; } = 3; //Sorta weird.  Have to do a - 1 in the check
    public float Scale { get; set; } = .5f;

    public int MaxImbueEffects { get; set; } = 2;


    public const string RecipeManagerCategory = "RecipeManagerPatch";
    public bool EnableRecipeManagerPatch { get; set; } = true;
}