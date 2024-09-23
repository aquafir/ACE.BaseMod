namespace Expansion.Enums;

public enum WeenieTypeGroup
{
    Container
}

public static class WeenieTypeGroupHelper
{
    public static string DefaultTargets(this Mutation mutator) => mutator switch
    {
        Mutation.Slayer => nameof(WeenieTypeGroup.Container),
        _ => nameof(WeenieTypeGroup.Container)
    };

    public static WeenieType[] SetOf(this WeenieTypeGroup type) => type switch
    {
        WeenieTypeGroup.Container => new WeenieType[] {
        WeenieType.Container,
        },
        _ => throw new NotImplementedException(),
    };
}
