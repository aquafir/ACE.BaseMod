namespace Expansion.Enums;

/// <summary>
/// MATCH THE CLASS NAME OF THE MUTATOR
/// </summary>
public enum Mutation
{
    AutoScale,
    Enlightened,
    GrowthItem,
    IronmanLocked,
    LocationLocked,
    ProcOnAttack,
    ProcOnHit,
    Resize,
    Set,
    ShinyPet,
    Slayer,
    TowerLocked,
    RandomColor,
    SampleMutator,
}

[Flags]
public enum MutationEvent
{
    Loot        = 0x1,      //LootGenerationFactory.CreateRandomLootObjects_New
    Corpse      = 0x2,      //Creature.GenerateTreasure
    Generator   = 0x4,      //GeneratorProfile.TreasureGenerator
    Factory     = 0x8,      //WeenieFactory different creates.  Probably more expensive
    EnterWorld  = 0x10,     //WeenieObject.EnterWorld.  Has location/scale?
    Containers  = Corpse | Generator,
    Inventory   = 0x20,     //Player.TryCreateInInventoryWithNetworking / TryCreateForGive
    EmoteGive   = 0x40,     //Player_Inventory.GiveFromEmote
    VendorBuy   = 0x80,     //Vendor.BuyItems_ValidateTransaction / FinalizeBuyTransaction
    //VendorStock = 0x100,    //Vendor.AddDefaultItem might work?
    All         = Containers | Loot | Factory | EnterWorld | Inventory | EmoteGive | VendorBuy //| VendorStock
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
