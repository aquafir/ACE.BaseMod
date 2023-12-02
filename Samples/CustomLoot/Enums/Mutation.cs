namespace CustomLoot.Enums;

/// <summary>
/// MATCH THE CLASS NAME OF THE MUTATOR
/// </summary>
public enum Mutation
{
    LocationLocked,
    GrowthItem,
    ProcOnAttack,
    ProcOnHit,
    Set,
    ShinyPet,
    Slayer,
    Resize,
    AutoScale,
}

[Flags]
public enum MutationEvent
{
    Loot                = 0x1,      //LootGenerationFactory.CreateRandomLootObjects_New
    Corpse              = 0x2,      //Creature.GenerateTreasure
    Generator           = 0x4,      //GeneratorProfile.TreasureGenerator
    Factory             = 0x8,      //WeenieFactory different creates.  Probably more expensive
    EnterWorld          = 0x10,     //WeenieObject.EnterWorld.  Has location/scale?
    Containers = Corpse | Generator,
    //Factory = 0x4,  //LootGenerationFactory.CreateRandomLootObjects
}

public static class MutationHelper
{
    public static MutationEvent DefaultEvents(this Mutation mutator) => mutator switch
    {
        Mutation.Resize => MutationEvent.EnterWorld,
        Mutation.AutoScale => MutationEvent.EnterWorld,
        _ => MutationEvent.Loot,
    };

    //public static TreasureItemType_Orig[] SetOf(this TargetGroup type) => type switch
    //{
    //    _ => throw new NotImplementedException(),
    //};
}
