using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoot.Enums;


public enum SpellGroup
{
    SurgeTargetSelf,
}

public static class SpellGroupHelper
{
    public static HashSet<TreasureItemType_Orig> SetOf(this SpellGroup type) => type switch
    {
        SpellGroup.Equipables => new()
        {
            TreasureItemType_Orig.Armor,
            TreasureItemType_Orig.Clothing,
            TreasureItemType_Orig.Cloak,
            TreasureItemType_Orig.Jewelry,
            TreasureItemType_Orig.Weapon,

        },
        
        _ => new(),
    };
}
