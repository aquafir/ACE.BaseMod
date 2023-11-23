namespace CustomLoot.Enums;

public enum ImbueGroup
{
    AllRending,
    ElementalRending,
    PhysicalRending,
    Rending,
}

public static class ImbueGroupHelper
{
    //public static string DefaultTargets(this Mutation mutator) => mutator switch
    //{
    //    Mutation.Slayer => nameof(ImbueGroup.Weapon),
    //    _ => nameof(ImbueGroup.Wearables)
    //};

    public static ImbuedEffectType[] SetOf(this ImbueGroup type) => type switch
    {
        ImbueGroup.Rending => new[]
        {
            ImbuedEffectType.SlashRending,
            ImbuedEffectType.PierceRending,
            ImbuedEffectType.BludgeonRending,
            ImbuedEffectType.AcidRending,
            ImbuedEffectType.ColdRending,
            ImbuedEffectType.ElectricRending,
            ImbuedEffectType.FireRending,
            ImbuedEffectType.NetherRending,
        },
        ImbueGroup.AllRending => new[]
        {
            ImbuedEffectType.NetherRending |
            ImbuedEffectType.SlashRending | ImbuedEffectType.PierceRending | ImbuedEffectType.BludgeonRending |
            ImbuedEffectType.AcidRending | ImbuedEffectType.ColdRending | ImbuedEffectType.ElectricRending | ImbuedEffectType.FireRending,
        },
        ImbueGroup.ElementalRending => new[]
        {
            ImbuedEffectType.AcidRending | ImbuedEffectType.ColdRending | ImbuedEffectType.ElectricRending | ImbuedEffectType.FireRending,
        },
        ImbueGroup.PhysicalRending => new[]
        {
            ImbuedEffectType.SlashRending | ImbuedEffectType.PierceRending | ImbuedEffectType.BludgeonRending 
        },
        _ => throw new NotImplementedException(),
    };
}
