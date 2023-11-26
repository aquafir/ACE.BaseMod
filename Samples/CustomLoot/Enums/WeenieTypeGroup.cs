namespace CustomLoot.Enums;

public enum WeenieTypeGroup
{
    Placeholder
}

public static class WeenieTypeGroupHelper
{
    public static string DefaultTargets(this Mutation mutator) => mutator switch
    {
        Mutation.Slayer => nameof(WeenieTypeGroup.Placeholder),
        _ => nameof(WeenieTypeGroup.Placeholder)
    };

    public static WeenieType[] SetOf(this WeenieTypeGroup type) => type switch
    {

        _ => throw new NotImplementedException(),
    };
}
