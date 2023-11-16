namespace CustomLoot;

public class Sets
{
    #region SpellId sets
    //Ring spells that also show on cloaks?
    public static readonly List<SpellId> cloakRingSpells = new()
    {
        SpellId.AcidRing,           // Searing Disc
        SpellId.BladeRing,          // Horizon's Blades
        SpellId.FlameRing,          // Cassius' Ring of Fire
        SpellId.ForceRing,          // Nuhmudira's Spines
        SpellId.FrostRing,          // Halo of Frost
        SpellId.LightningRing,      // Eye of the Storm
        SpellId.ShockwaveRing,      // Tectonic Rifts
        SpellId.NetherRing,         // Clouded Soul
    };

    //Just cloak spells?
    public static readonly List<SpellId> cloakSpecificSpells = new()
        {
            SpellId.CloakAllSkill,      // Cloaked in Skill
            SpellId.CloakMagicDLower,   // Shroud of Darkness (Magic)
            SpellId.CloakMeleeDLower,   // Shroud of Darkness (Melee)
            SpellId.CloakMissileDLower, // Shroud of Darkness (Missile)
        };
    #endregion

    #region Aetheria Sets
    public static Dictionary<SpellId, bool> SurgeTargetSelf = new Dictionary<SpellId, bool>()
        {
            { SpellId.AetheriaProcDamageBoost,     true },
            { SpellId.AetheriaProcDamageOverTime,  false },
            { SpellId.AetheriaProcDamageReduction, true },
            { SpellId.AetheriaProcHealDebuff,      false },
            { SpellId.AetheriaProcHealthOverTime,  true },
        };

    public static Dictionary<Sigil, EquipmentSet> SigilToEquipmentSet = new Dictionary<Sigil, EquipmentSet>()
        {
            { Sigil.Defense, EquipmentSet.AetheriaDefense },
            { Sigil.Destruction, EquipmentSet.AetheriaDestruction },
            { Sigil.Fury, EquipmentSet.AetheriaFury },
            { Sigil.Growth, EquipmentSet.AetheriaGrowth },
            { Sigil.Vigor, EquipmentSet.AetheriaVigor }
        }; 
    #endregion

    #region EquipmentSets
    //Standard armor sets
    public static readonly List<EquipmentSet> armorSets = new()
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
        };
    //Standard cloak sets
    public static readonly List<EquipmentSet> cloakSets = new()
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
    }; 
    #endregion    
}
