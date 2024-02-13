namespace Expansion.Enums;


public enum EquipmentSetGroup
{
    Armor,
    Cloak,
}

public static class EquipmentSetHelper
{
    public static EquipmentSet[] SetOf(this EquipmentSetGroup type) => type switch
    {
        EquipmentSetGroup.Armor => new[]
        {
            EquipmentSet.Soldiers,
            EquipmentSet.Adepts,
            EquipmentSet.Archers,
            EquipmentSet.Defenders,
            EquipmentSet.Tinkers,
            EquipmentSet.Crafters,
            EquipmentSet.Hearty,
            EquipmentSet.Dexterous,
            EquipmentSet.Wise,
            EquipmentSet.Swift,
            EquipmentSet.Hardened,
            EquipmentSet.Reinforced,
            EquipmentSet.Interlocking,
            EquipmentSet.Flameproof,
            EquipmentSet.Acidproof,
            EquipmentSet.Coldproof,
            EquipmentSet.Lightningproof,
        },
        EquipmentSetGroup.Cloak => new[]
        {
            EquipmentSet.CloakAlchemy,
            EquipmentSet.CloakArcaneLore,
            EquipmentSet.CloakArmorTinkering,
            EquipmentSet.CloakAssessPerson,
            EquipmentSet.CloakLightWeapons,
            EquipmentSet.CloakMissileWeapons,
            EquipmentSet.CloakCooking,
            EquipmentSet.CloakCreatureEnchantment,
            EquipmentSet.CloakFinesseWeapons,
            EquipmentSet.CloakDeception,
            EquipmentSet.CloakFletching,
            EquipmentSet.CloakHealing,
            EquipmentSet.CloakItemEnchantment,
            EquipmentSet.CloakItemTinkering,
            EquipmentSet.CloakLeadership,
            EquipmentSet.CloakLifeMagic,
            EquipmentSet.CloakLoyalty,
            EquipmentSet.CloakMagicDefense,
            EquipmentSet.CloakMagicItemTinkering,
            EquipmentSet.CloakManaConversion,
            EquipmentSet.CloakMeleeDefense,
            EquipmentSet.CloakMissileDefense,
            EquipmentSet.CloakSalvaging,
            EquipmentSet.CloakHeavyWeapons,
            EquipmentSet.CloakTwoHandedCombat,
            EquipmentSet.CloakVoidMagic,
            EquipmentSet.CloakWarMagic,
            EquipmentSet.CloakWeaponTinkering,
            EquipmentSet.CloakAssessCreature,
            EquipmentSet.CloakDirtyFighting,
            EquipmentSet.CloakDualWield,
            EquipmentSet.CloakRecklessness,
            EquipmentSet.CloakShield,
            EquipmentSet.CloakSneakAttack,
            EquipmentSet.CloakSummoning,
        },
        _ => throw new NotImplementedException(),
    };
}

