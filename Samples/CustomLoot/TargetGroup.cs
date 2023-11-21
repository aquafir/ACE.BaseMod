namespace CustomLoot;

public enum TargetGroup
{
    Equipables,
    Wearables,
    ArmorClothing,
    Accessories,
    Armor,
    Cloaks,
    Clothing,
    Consumable,
    Jewelry,
    Pet,
    Weapon,
}

public static class TargetGroupHelper
{
    public static HashSet<TreasureItemType_Orig> SetOf(this TargetGroup type) => type switch
    {
        TargetGroup.Equipables => new()
        {
            TreasureItemType_Orig.Armor,
            TreasureItemType_Orig.Clothing,
            TreasureItemType_Orig.Cloak,
            TreasureItemType_Orig.Jewelry,
            TreasureItemType_Orig.Weapon,

        },
        TargetGroup.Wearables => new()
        {
            TreasureItemType_Orig.Armor,
            TreasureItemType_Orig.Clothing,
            TreasureItemType_Orig.Cloak,
            TreasureItemType_Orig.Jewelry,

        },
        TargetGroup.ArmorClothing => new()
        {
            TreasureItemType_Orig.Armor,
            TreasureItemType_Orig.Clothing,
        },
        TargetGroup.Accessories => new()
        {
            TreasureItemType_Orig.Cloak,
            TreasureItemType_Orig.Jewelry,
        },
        TargetGroup.Armor => new()
        {
            TreasureItemType_Orig.Armor,
        },
        TargetGroup.Clothing => new()
        {
            TreasureItemType_Orig.Clothing,
        },
        TargetGroup.Cloaks => new()
        {
            TreasureItemType_Orig.Cloak,
        },
        TargetGroup.Consumable => new()
        {
            TreasureItemType_Orig.Consumable,
        },
        TargetGroup.Pet => new()
        {
            TreasureItemType_Orig.PetDevice,
        },
        TargetGroup.Jewelry => new()
        {
            TreasureItemType_Orig.Jewelry,
        },
        TargetGroup.Weapon => new()
        {
            TreasureItemType_Orig.Weapon,
        },
        _ => new(),
    };
}
