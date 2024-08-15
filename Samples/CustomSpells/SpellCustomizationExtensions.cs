
namespace CustomSpells;

public static class SpellCustomizationExtensions
{
    public static void Apply(this SpellCustomization sc, SpellBase sb, ACE.Database.Models.World.Spell db)
    {
        #region SpellBase
        //string spellWords;
        //string Name
        //string Desc
        //MagicSchool School
        //uint Icon
        //SpellCategory Category
        //uint Bitfield
        //uint BaseMana
        //float BaseRangeConstant
        //float BaseRangeMod
        //uint Power
        //float SpellEconomyMod
        //uint FormulaVersion
        //float ComponentLoss
        //SpellType MetaSpellType
        //uint MetaSpellId
        //double Duration
        //float DegradeModifier
        //float DegradeLimit
        //double PortalLifetime
        //List<uint> Formula
        //uint CasterEffect
        //uint TargetEffect
        //uint FizzleEffect
        //double RecoveryInterval
        //float RecoveryAmount
        //uint DisplayOrder
        //uint NonComponentTargetType
        //uint ManaMod
        #endregion
        #region ACE.Database.Models.World.Spell
        //uint Id
        //string Name
        //uint? StatModType
        //uint? StatModKey
        //float? StatModVal
        //uint? EType
        //int? BaseIntensity
        //int? Variance
        //uint? Wcid
        //int? NumProjectiles
        //int? NumProjectilesVariance
        //float? SpreadAngle
        //float? VerticalAngle
        //float? DefaultLaunchAngle
        //bool? NonTracking
        //float? CreateOffsetOriginX
        //float? CreateOffsetOriginY
        //float? CreateOffsetOriginZ
        //float? PaddingOriginX
        //float? PaddingOriginY
        //float? PaddingOriginZ
        //float? DimsOriginX
        //float? DimsOriginY
        //float? DimsOriginZ
        //float? PeturbationOriginX
        //float? PeturbationOriginY
        //float? PeturbationOriginZ
        //uint? ImbuedEffect
        //int? SlayerCreatureType
        //float? SlayerDamageBonus
        //double? CritFreq
        //double? CritMultiplier
        //int? IgnoreMagicResist
        //double? ElementalModifier
        //float? DrainPercentage
        //float? DamageRatio
        //int? DamageType
        //int? Boost
        //int? BoostVariance
        //int? Source
        //int? Destination
        //float? Proportion
        //float? LossPercent
        //int? SourceLoss
        //int? TransferCap
        //int? MaxBoostAllowed
        //uint? TransferBitfield
        //int? Index
        //int? Link
        //uint? PositionObjCellId
        //float? PositionOriginX
        //float? PositionOriginY
        //float? PositionOriginZ
        //float? PositionAnglesW
        //float? PositionAnglesX
        //float? PositionAnglesY
        //float? PositionAnglesZ
        //int? MinPower
        //int? MaxPower
        //float? PowerVariance
        //int? DispelSchool
        //int? Align
        //int? Number
        //float? NumberVariance
        //double? DotDuration
        #endregion

        sc.Id = sc.Id ?? sc.Template;
        uint ID = (uint)sc.Id;

        //Apply customization to SpellBase and DB Spell
        sb.MetaSpellId = ID;
        sb.spellWords = sc.SpellWords ?? sb.spellWords;
        sb.Name = sc.Name ?? sb.Name;
        sb.School = sc.School ?? sb.School;
        sb.Category = sc.Category ?? sb.Category;
        sb.Bitfield = sc.Bitfield is not null ? (uint)sc.Bitfield.Value : sb.Bitfield;
        sb.BaseMana = sc.BaseMana ?? sb.BaseMana;
        sb.BaseRangeConstant = sc.BaseRangeConstant ?? sb.BaseRangeConstant;
        sb.BaseRangeMod = sc.BaseRangeMod ?? sb.BaseRangeMod;
        sb.Power = sc.Power ?? sb.Power;
        sb.Duration = sc.Duration ?? sb.Duration;
        sb.CasterEffect = sc.CasterEffect is not null ? (uint)sc.CasterEffect : sb.CasterEffect;
        sb.TargetEffect = sc.TargetEffect is not null ? (uint)sc.TargetEffect : sb.TargetEffect;
        //sb.NonComponentTargetType = sc.NonComponentTargetType is not null ? (uint)sc. sb.NonComponentTargetType;
        
        db.Id = ID;
        db.Wcid = sc.Wcid ?? db.Wcid;
        db.Name = sc.Name ?? db.Name;
        db.StatModType = sc.StatModType is not null ? (uint)sc.StatModType : db.StatModType;
        db.StatModKey = sc.StatModKey is not null? (uint)sc.StatModKey : db.StatModKey;
        db.EType = sc.EType is not null ? (uint)sc.EType : db.EType;
        db.DamageType = sc.DamageType is not null ? (int)sc.DamageType : db.DamageType;
        db.StatModVal = sc.StatModVal ?? db.StatModVal;
        db.BaseIntensity = sc.BaseIntensity ?? db.BaseIntensity;
        db.Variance = sc.Variance ?? db.Variance;
        //db.NumProjectiles = sc.NumProjectiles ?? db.NumProjectiles;
        //db.DrainPercentage = sc.DrainPercentage ?? db.DrainPercentage;
        //db.DamageRatio = sc.DamageRatio ?? db.DamageRatio;

        #region Used?
        //USED / Converted
        //uint? MetaSpellId = ID
        //string? spellWords
        //string? Name
        //MagicSchool? School
        //SpellCategory? Category
        //SpellFlags? Bitfield
        //uint? BaseMana
        //float? BaseRangeConstant
        //float? BaseRangeMod
        //uint? Power
        //double? Duration
        //PlayScript? CasterEffect
        //PlayScript? TargetEffect
        //PlayScript? FizzleEffect
        //ItemType? NonComponentTargetType
        //uint? Wcid

        //uint? Id
        //string? Name
        //EnchantmentTypeFlags? StatModType
        //One of: Skill | PropertyAttribute | PropertyAttribute2nd | PropertyInt | PropertyFloat
        //uint? StatModKey
        //DamageType? EType
        //DamageType? DamageType
        //float? StatModVal
        //int? BaseIntensity
        //int? Variance
        //int? NumProjectiles
        //float? DrainPercentage
        //float? DamageRatio
        //double? DotDuration

        //Life stuff
        //int? Boost
        //int? BoostVariance
        //PropertyAttribute2nd? Source
        //PropertyAttribute2nd? Destination
        //float? Proportion
        //float? LossPercent
        //int? SourceLoss
        //int? TransferCap
        #endregion
    }
}

public record struct SetTier(uint NumEquipped, List<SpellId> Spells);

public record struct SpellCustomization(SpellId Template, 
        SpellId? Id = default(SpellId),
        string Name = null,
        string SpellWords = null,
        // The magic school this spell belongs to
        MagicSchool? School = null,
        // Used for spell stacking, ie. Strength Self I and Strength Self VI will be the same category
        SpellCategory? Category = null,

        // bit flags for the spell
        SpellFlags? Bitfield = null,
        // The base mana cost required for casting the spell
        uint? BaseMana = null,
        //The base maximum distance for casting the spell
        float? BaseRangeConstant = null,
        // An additive multiplier to BaseRangeConstant based on caster's skill level
        float? BaseRangeMod = null,

        // The difficulty of casting the spell
        uint? Power = null,

        //ItemType? NonComponentTargetType,        

        // The stat modifier type
        EnchantmentTypeFlags? StatModType = null,
        // The stat modifier key, used for lookup in the enchantment registry
        uint? StatModKey = null,
        // The amount to modify a stat
        float? StatModVal = null,

        // The damage type for this spell
        DamageType? EType = null,
        DamageType? DamageType = null,

        // The base amount of damage for this spell
        int? BaseIntensity = null,
        // The maximum additional daamage for this spell
        int? Variance = null,

        //int? NumProjectiles = null,
        //float? DrainPercentage = null,
        //float? DamageRatio = null,

        // The amount of time the spell lasts for EnchantmentSpell / FellowshipEnchantmentSpells
        double? Duration = null,
        // The DoT (damage over time) duration for the spell
        double? DotDuration = null,

        //Cosmetic
        // Effect that plays on the caster for this spell (ie. for buffs, protects, etc.)
        PlayScript? CasterEffect = null,
        // Effect that plays on the target for this spell (ie. for debuffs, vulns, etc.)
        PlayScript? TargetEffect = null,
        //PlayScript? FizzleEffect = null,
        // The weenie class ID associated for this spell, ie. the projectile weenie class id
        uint? Wcid = null
    );
