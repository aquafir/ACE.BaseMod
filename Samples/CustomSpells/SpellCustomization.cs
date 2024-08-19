
namespace CustomSpells;

public class SpellCustomization(
        SpellId Template,
        SpellId? Id = default(SpellId),
        string Name = null,
        string SpellWords = null,
        MagicSchool? School = null,
        SpellCategory? Category = null,
        SpellFlags? Bitfield = null,
        uint? BaseMana = null,
        float? BaseRangeConstant = null,
        float? BaseRangeMod = null,
        uint? Power = null,
        EnchantmentTypeFlags? StatModType = null,
        uint? StatModKey = null,
        float? StatModVal = null,
        DamageType? EType = null,
        DamageType? DamageType = null,
        int? BaseIntensity = null,
        int? Variance = null,
        //int? NumProjectiles = null,
        //float? DrainPercentage = null,
        //float? DamageRatio = null,
        double? Duration = null,
        double? DotDuration = null,
        PlayScript? CasterEffect = null,
        PlayScript? TargetEffect = null,
        uint? Wcid = null
    )
{
    [FormulaResult]
    public SpellId Template { get; set; } = Template;
    [FormulaResult]
    public SpellId? Id { get; set; } = Id;
    [FormulaResult]
    public string Name { get; set; } = Name;
    [FormulaResult]
    public string SpellWords { get; set; } = SpellWords;
    [FormulaResult]
    public MagicSchool? School { get; set; } = School;
    [FormulaResult]
    public SpellCategory? Category { get; set; } = Category;
    [FormulaResult]
    public SpellFlags? Bitfield { get; set; } = Bitfield;
    [FormulaResult]
    public uint? BaseMana { get; set; } = BaseMana;
    [FormulaResult]
    public float? BaseRangeConstant { get; set; } = BaseRangeConstant;
    [FormulaResult]
    public float? BaseRangeMod { get; set; } = BaseRangeMod;
    [FormulaResult]
    public uint? Power { get; set; } = Power;
    [FormulaResult]
    public EnchantmentTypeFlags? StatModType { get; set; } = StatModType;
    [FormulaResult]
    public uint? StatModKey { get; set; } = StatModKey;
    [FormulaResult]
    public float? StatModVal { get; set; } = StatModVal;
    [FormulaResult]
    public DamageType? EType { get; set; } = EType;
    [FormulaResult]
    public DamageType? DamageType { get; set; } = DamageType;
    [FormulaResult]
    public int? BaseIntensity { get; set; } = BaseIntensity;
    [FormulaResult]
    public int? Variance { get; set; } = Variance;
    [FormulaResult]
    public double? Duration { get; set; } = Duration;
    [FormulaResult]
    public double? DotDuration { get; set; } = DotDuration;
    [FormulaResult]
    public PlayScript? CasterEffect { get; set; } = CasterEffect;
    [FormulaResult]
    public PlayScript? TargetEffect { get; set; } = TargetEffect;
    [FormulaResult]
    public uint? Wcid { get; set; } = Wcid;

    static Settings Settings => PatchClass.Settings;

    /// <summary>
    /// Applies non-null values of a SpellCustomization to a SpellBase and Database Spell
    /// </summary>
    public void Apply(SpellBase sb, ACE.Database.Models.World.Spell db)
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

        Id = Id ?? Template;
        uint ID = (uint)Id;

        //Apply customization to SpellBase and DB Spell
        sb.MetaSpellId = ID;
        sb.spellWords = SpellWords ?? sb.spellWords;
        sb.Name = Name ?? sb.Name;
        sb.School = School ?? sb.School;
        sb.Category = Category ?? sb.Category;
        sb.Bitfield = Bitfield is not null ? (uint)Bitfield.Value : sb.Bitfield;
        sb.BaseMana = BaseMana ?? sb.BaseMana;
        sb.BaseRangeConstant = BaseRangeConstant ?? sb.BaseRangeConstant;
        sb.BaseRangeMod = BaseRangeMod ?? sb.BaseRangeMod;
        sb.Power = Power ?? sb.Power;
        sb.Duration = Duration ?? sb.Duration;
        sb.CasterEffect = CasterEffect is not null ? (uint)CasterEffect : sb.CasterEffect;
        sb.TargetEffect = TargetEffect is not null ? (uint)TargetEffect : sb.TargetEffect;
        //sb.NonComponentTargetType = NonComponentTargetType is not null ? (uint) sb.NonComponentTargetType;

        db.Id = ID;
        db.Wcid = Wcid ?? db.Wcid;
        db.Name = Name ?? db.Name;
        db.StatModType = StatModType is not null ? (uint)StatModType : db.StatModType;
        db.StatModKey = StatModKey is not null ? (uint)StatModKey : db.StatModKey;
        db.EType = EType is not null ? (uint)EType : db.EType;
        db.DamageType = DamageType is not null ? (int)DamageType : db.DamageType;
        db.StatModVal = StatModVal ?? db.StatModVal;
        db.BaseIntensity = BaseIntensity ?? db.BaseIntensity;
        db.Variance = Variance ?? db.Variance;
        //db.NumProjectiles = NumProjectiles ?? db.NumProjectiles;
        //db.DrainPercentage = DrainPercentage ?? db.DrainPercentage;
        //db.DamageRatio = DamageRatio ?? db.DamageRatio;

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

    /// <summary>
    /// Tries to open and correctly map to an excel file, optionally creating it if missing
    /// </summary>
    public static bool TryGetSpreadsheet(string path, out ExcelMapper excel, bool createMissing = true)
    {
        //Save out a default if missing?
        excel = null;
        try
        {
            if (!File.Exists(path) && createMissing)
            {
                new ExcelMapper().Save(path, new List<SpellCustomization>(), "Spells");
                ModManager.Log($"Created: {path}");
            }

            excel = new(path);
        }
        catch (Exception ex)
        {
            ModManager.Log($"Failed to open spreadsheet @ {path}:\r\n{ex.Message}");
            return false;
        }


        //Set up mappings - blank SpellCustomization used for nameof for refactoring
        SpellCustomization b = new(SpellId.AcidArc1);

        //SpellId Template
        excel.AddMapping<SpellCustomization>(nameof(b.Template), p => p.Template)
            .SetPropertyUsing(cellValue => TryParseCellEnum<SpellId>(cellValue, out var parsed) ? parsed : null);
        //SpellId Id
        excel.AddMapping<SpellCustomization>(nameof(b.Id), p => p.Id)
            .SetPropertyUsing(cellValue => TryParseCellEnum<SpellId>(cellValue, out var parsed) ? parsed : null);
        //MagicSchool School
        excel.AddMapping<SpellCustomization>(nameof(b.School), p => p.School)
            .SetPropertyUsing(cellValue => TryParseCellEnum<MagicSchool>(cellValue, out var parsed, true) ? parsed : null);
        //SpellCategory Category
        excel.AddMapping<SpellCustomization>(nameof(b.Category), p => p.Category)
            .SetPropertyUsing(cellValue => TryParseCellEnum<SpellCategory>(cellValue, out var parsed) ? parsed : null);
        //SpellFlags Bitfield
        excel.AddMapping<SpellCustomization>(nameof(b.Bitfield), p => p.Bitfield)
            .SetPropertyUsing(cellValue => TryParseCellEnum<SpellFlags>(cellValue, out var parsed) ? parsed : null);
        //EnchantmentTypeFlags? StatModType
        excel.AddMapping<SpellCustomization>(nameof(b.StatModType), p => p.StatModType)
            .SetPropertyUsing(cellValue => TryParseCellEnum<EnchantmentTypeFlags>(cellValue, out var parsed) ? parsed : null);
        //DamageType? EType
        excel.AddMapping<SpellCustomization>(nameof(b.EType), p => p.EType)
            .SetPropertyUsing(cellValue => TryParseCellEnum<DamageType>(cellValue, out var parsed, true) ? parsed : null);
        //DamageType? DamageType
        excel.AddMapping<SpellCustomization>(nameof(b.DamageType), p => p.DamageType)
            .SetPropertyUsing(cellValue => TryParseCellEnum<DamageType>(cellValue, out var parsed, true) ? parsed : null);
        //PlayScript? CasterEffect
        excel.AddMapping<SpellCustomization>(nameof(b.CasterEffect), p => p.CasterEffect)
            .SetPropertyUsing(cellValue => TryParseCellEnum<PlayScript>(cellValue, out var parsed) ? parsed : null);
        //PlayScript? TargetEffect
        excel.AddMapping<SpellCustomization>(nameof(b.TargetEffect), p => p.TargetEffect)
            .SetPropertyUsing(cellValue => TryParseCellEnum<PlayScript>(cellValue, out var parsed) ? parsed : null);

        return true;
    }


    /// <summary>
    /// Tries to load SpellCustomizations from the path in Settings
    /// </summary>
    public static List<SpellCustomization> ParseCustomizations()
    {
        if (TryGetSpreadsheet(Settings.CustomSpellsSpreadsheet, out var excel))
            return ParseCustomizations(excel);

        return new();
    }

    /// <summary>
    /// Tries to load SpellCustomizations from an Excel file
    /// </summary>
    public static List<SpellCustomization> ParseCustomizations(ExcelMapper excel)
    {
        List<SpellCustomization> customSpells = new();

        if (excel is null)
            return customSpells;

        try
        {
            customSpells = excel.Fetch<SpellCustomization>().ToList();
        }
        catch (Exception ex)
        {
            ModManager.Log($"Failed to parse custom spells:\r\n{ex.Message}");
            return customSpells;
        }

        return customSpells;
    }

    /// <summary>
    /// Tries to convert a cell to an Enum, attempting to parse as a number first
    /// </summary>
    public static bool TryParseCellEnum<T>(object cellValue, out T parsed, bool requireDefined = false) where T : struct, Enum
    {
        parsed = default;

        //Make sure the cell has a string value and add a special case where 'null' is ignored?
        //if ((cellValue is not string stringValue)) //|| String.Equals(stringValue, "null", StringComparison.InvariantCultureIgnoreCase))
        //    return false;

        return cellValue.TryConvertToEnum(out parsed, requireDefined);

        //if (cellValue.TryConvertToEnum(out parsed))
        //    return true;

        //if (long.TryParse(stringValue, out var numberValue))
        //    return numberValue.TryConvertToEnum<T>(out parsed);

        //return false;

        //Check for number value first?
        //if (long.TryParse(stringValue, out var numberValue))
        //    return numberValue.TryConvertToEnum<T>(out parsed);

        //Fall back to string
        //return cellValue.TryConvertToEnum(out parsed);
    }
}
