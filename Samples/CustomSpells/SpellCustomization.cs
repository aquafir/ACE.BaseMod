
using NPOI.SS.Formula;
using System.Globalization;
using JsonAttribute = Ganss.Excel.JsonAttribute;

namespace CustomSpells;

public class SpellCustomization
{
    [FormulaResult]
    public SpellId Template { get; set; }
    [FormulaResult]
    public SpellId? Id { get; set; }
    [FormulaResult]
    public string Name { get; set; }
    [FormulaResult]
    public string SpellWords { get; set; }
    [FormulaResult]
    public MagicSchool? School { get; set; }
    [FormulaResult]
    public SpellCategory? Category { get; set; }
    [FormulaResult]
    public SpellFlags? Bitfield { get; set; }
    [FormulaResult]
    public uint? BaseMana { get; set; }
    [FormulaResult]
    public float? BaseRangeConstant { get; set; }
    [FormulaResult]
    public float? BaseRangeMod { get; set; }
    [FormulaResult]
    public uint? Power { get; set; }
    [FormulaResult]
    public EnchantmentTypeFlags? StatModType { get; set; }
    [FormulaResult]
    public uint? StatModKey { get; set; }
    [FormulaResult]
    public float? StatModVal { get; set; }
    [FormulaResult]
    public DamageType? EType { get; set; }
    [FormulaResult]
    public DamageType? DamageType { get; set; }
    [FormulaResult]
    public int? BaseIntensity { get; set; }
    [FormulaResult]
    public int? Variance { get; set; }
    [FormulaResult]
    public int? NumProjectiles { get; set; }
    [FormulaResult]
    public float? DrainPercentage { get; set; }
    [FormulaResult]
    public float? DamageRatio { get; set; }
    [FormulaResult]
    public double? Duration { get; set; }
    [FormulaResult]
    public double? DotDuration { get; set; }
    [FormulaResult]
    public PlayScript? CasterEffect { get; set; }
    [FormulaResult]
    public PlayScript? TargetEffect { get; set; }
    [FormulaResult]
    public ItemType? NonComponentTargetType { get; set; }
    [FormulaResult]
    public uint? Wcid { get; set; }
    [FormulaResult]
    public int? Boost { get; set; }
    [FormulaResult]
    public int? BoostVariance { get; set; }
    [FormulaResult]
    public PropertyAttribute2nd? Source { get; set; }
    [FormulaResult]
    public PropertyAttribute2nd? Destination { get; set; }
    [FormulaResult]
    public float? Proportion { get; set; }
    [FormulaResult]
    public float? LossPercent { get; set; }
    [FormulaResult]
    public int? TransferCap { get; set; }
    [FormulaResult]
    public TransferFlags? TransferFlags { get; set; }
    [FormulaResult]
    public double? PortalLifetime { get; set; }
    [FormulaResult]
    public int? Link { get; set; }
    [FormulaResult]
    public float? SpreadAngle { get; set; }
    [FormulaResult]
    public bool? NonTracking { get; set; }
    [FormulaResult]
    [Json]
    public Vector3? CreateOffset { get; set; }
    [FormulaResult]
    [Json]
    public Vector3? Padding { get; set; }
    [FormulaResult]
    [Json]
    public Vector3? Peturbation { get; set; }
    [FormulaResult]
    public int? MinPower { get; set; }
    [FormulaResult]
    public int? MaxPower { get; set; }
    [FormulaResult]
    public MagicSchool? DispelSchool { get; set; }
    [FormulaResult]
    public DispelType? Align { get; set; }
    [FormulaResult]
    public int? Number { get; set; }
    [FormulaResult]
    public float? NumberVariance { get; set; }
    [FormulaResult]
    public List<uint>? Formula { get; set; }
    //    [FormulaResult]
    //#if REALM
    //    public LocalPosition? Position { get; set; }
    //#else
    //    public Position? Position { get; set; }
    //#endif

    static Settings Settings => PatchClass.Settings;

    public SpellCustomization() { }
    public SpellCustomization(
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
            int? NumProjectiles = null,
            float? DrainPercentage = null,
            float? DamageRatio = null,
            double? Duration = null,
            double? DotDuration = null,
            PlayScript? CasterEffect = null,
            PlayScript? TargetEffect = null,
            ItemType? NonComponentTargetType = null,
            uint? Wcid = null,
            int? Boost = null,
            int? BoostVariance = null,
            PropertyAttribute2nd? Source = null,
            PropertyAttribute2nd? Destination = null,
            float? Proportion = null,
            float? LossPercent = null,
            int? TransferCap = null,
            TransferFlags? TransferFlags = null,
            double? PortalLifetime = null,
            int? Link = null,
            float? SpreadAngle = null,
            bool? NonTracking = null,
            Vector3? CreateOffset = null,
            Vector3? Padding = null,
            Vector3? Peturbation = null,
            int? MinPower = null,
            int? MaxPower = null,
            MagicSchool? DispelSchool = null,
            DispelType? Align = null,
            int? Number = null,
            float? NumberVariance = null
            //List<uint>? Formula = null
    //        ,
    //#if REALM
    //            LocalPosition? Position = null
    //#else
    //        Position? Position = null
    //#endif
    )
    {
        this.Template = Template;
        this.Id = Id;
        this.Name = Name;
        this.SpellWords = SpellWords;
        this.School = School;
        this.Category = Category;
        this.Bitfield = Bitfield;
        this.BaseMana = BaseMana;
        this.BaseRangeConstant = BaseRangeConstant;
        this.BaseRangeMod = BaseRangeMod;
        this.Power = Power;
        this.StatModType = StatModType;
        this.StatModKey = StatModKey;
        this.StatModVal = StatModVal;
        this.EType = EType;
        this.DamageType = DamageType;
        this.BaseIntensity = BaseIntensity;
        this.Variance = Variance;
        this.NumProjectiles = NumProjectiles;
        this.DrainPercentage = DrainPercentage;
        this.DamageRatio = DamageRatio;
        this.Duration = Duration;
        this.DotDuration = DotDuration;
        this.CasterEffect = CasterEffect;
        this.TargetEffect = TargetEffect;
        this.NonComponentTargetType = NonComponentTargetType;
        this.Wcid = Wcid;
        this.Boost = Boost;
        this.BoostVariance = BoostVariance;
        this.Source = Source;
        this.Destination = Destination;
        this.Proportion = Proportion;
        this.LossPercent = LossPercent;
        this.TransferCap = TransferCap;
        this.TransferFlags = TransferFlags;
        this.PortalLifetime = PortalLifetime;
        this.Link = Link;
        this.SpreadAngle = SpreadAngle;
        this.NonTracking = NonTracking;
        this.CreateOffset = CreateOffset;
        this.Padding = Padding;
        this.Peturbation = Peturbation;
        this.MinPower = MinPower;
        this.MaxPower = MaxPower;
        this.DispelSchool = DispelSchool;
        this.Align = Align;
        this.Number = Number;
        this.NumberVariance = NumberVariance;

        this.Formula = Formula;
        //this.Position = Position;
    }

    /// <summary>
    /// Creates a spell customization from a SpellBase and Database Spell
    /// </summary>
    public SpellCustomization(SpellBase sb, ACE.Database.Models.World.Spell db, ACE.Server.Entity.Spell spell)
    {
        Template = (SpellId)db.Id;
        Id = (SpellId?)db.Id;
        Name = db.Name;
        SpellWords = sb.spellWords;
        School = sb.School;
        Category = sb.Category;
        Bitfield = (SpellFlags?)sb.Bitfield;
        BaseMana = sb.BaseMana;
        BaseRangeConstant = sb.BaseRangeConstant;
        BaseRangeMod = sb.BaseRangeMod;
        Power = sb.Power;
        StatModType = (EnchantmentTypeFlags?)db.StatModType;
        StatModKey = db.StatModKey;
        StatModVal = db.StatModVal;
        EType = (DamageType?)db.EType;
        DamageType = (DamageType?)db.DamageType;
        BaseIntensity = db.BaseIntensity;
        Variance = db.Variance;
        NumProjectiles = db.NumProjectiles;
        DrainPercentage = db.DrainPercentage;
        DamageRatio = db.DamageRatio;
        Duration = sb.Duration;
        DotDuration = db.DotDuration;
        CasterEffect = (PlayScript?)sb.CasterEffect;
        TargetEffect = (PlayScript?)sb.TargetEffect;
        NonComponentTargetType = (ItemType?)sb.NonComponentTargetType;
        Wcid = db.Wcid;
        Boost = db.Boost;
        BoostVariance = db.BoostVariance;
        Source = (PropertyAttribute2nd?)db.Source;
        Destination = (PropertyAttribute2nd?)db.Destination;
        Proportion = db.Proportion;
        LossPercent = db.LossPercent;
        TransferCap = db.TransferCap;

        TransferFlags = (TransferFlags?)db.TransferBitfield;
        PortalLifetime = sb.PortalLifetime;
        Link = db.Link;
        SpreadAngle = db.SpreadAngle;
        NonTracking = db.NonTracking;

        MinPower = db.MinPower;
        MaxPower = db.MaxPower;
        DispelSchool = (MagicSchool?)db.DispelSchool;
        Align = (DispelType?)db.Align;
        Number = db.Number;
        NumberVariance = db.NumberVariance;

        //TODO: Property write Vector3 / Lists
        //CreateOffset = db.CreateOffsetOriginX is null ? null : default;
        //new Vector3(db.CreateOffsetOriginX ?? 0.0f, db.CreateOffsetOriginY ?? 0.0f, db.CreateOffsetOriginZ ?? 0.0f);
        //Padding = db.PaddingOriginX is null ? null : //default;
        //new Vector3(db.PaddingOriginX ?? 0.0f, db.PaddingOriginY ?? 0.0f, db.PaddingOriginZ ?? 0.0f);
        //Peturbation = db.PeturbationOriginX is null ? null : //default;
        //    new Vector3(db.PeturbationOriginX ?? 0.0f, db.PeturbationOriginY ?? 0.0f, db.PeturbationOriginZ ?? 0.0f);
        CreateOffset = spell.CreateOffset;
        Padding = spell.Padding;
        Peturbation = spell.Peturbation;
        //Formula = sb.Formula;

        //#if REALM
        //        LocalPosition? Position = null
        //#else
        //        Position? Position = null
        //#endif
    }

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
        sb.NonComponentTargetType = NonComponentTargetType is not null ? (uint)NonComponentTargetType : sb.NonComponentTargetType;

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

        db.NumProjectiles = NumProjectiles ?? db.NumProjectiles;
        db.DrainPercentage = DrainPercentage ?? db.DrainPercentage;
        db.DamageRatio = DamageRatio ?? db.DamageRatio;

        db.Boost = Boost ?? db.Boost;
        db.BoostVariance = BoostVariance ?? db.BoostVariance;
        db.Source = Source is not null ? (int)Source : db.Source;
        db.Destination = Destination is not null ? (int)Destination : db.Destination;
        db.Proportion = Proportion ?? db.Proportion;
        db.LossPercent = LossPercent ?? db.LossPercent;
        db.TransferCap = TransferCap ?? db.TransferCap;

        db.TransferBitfield = TransferFlags is not null ? (uint)TransferFlags : db.TransferBitfield;
        sb.PortalLifetime = PortalLifetime ?? sb.PortalLifetime;
        db.Link = Link ?? db.Link;
        db.SpreadAngle = SpreadAngle ?? db.SpreadAngle;
        db.NonTracking = NonTracking ?? db.NonTracking;

        if (CreateOffset is not null)
        {
            db.CreateOffsetOriginX = CreateOffset.Value.X;
            db.CreateOffsetOriginY = CreateOffset.Value.Y;
            db.CreateOffsetOriginZ = CreateOffset.Value.Z;
        }
        if (Padding is not null)
        {
            db.PaddingOriginX = Padding.Value.X;
            db.PaddingOriginY = Padding.Value.Y;
            db.PaddingOriginZ = Padding.Value.Z;
        }
        if (Peturbation is not null)
        {
            db.PeturbationOriginX = Peturbation.Value.X;
            db.PeturbationOriginY = Peturbation.Value.Y;
            db.PeturbationOriginZ = Peturbation.Value.Z;
        }

        db.MinPower = MinPower ?? db.MinPower;
        db.MaxPower = MaxPower ?? db.MaxPower;
        db.DispelSchool = DispelSchool is not null ? (int)DispelSchool : db.DispelSchool;
        db.Align = Align is not null ? (int)Align : db.Align;
        db.Number = Number ?? db.Number;
        db.NumberVariance = NumberVariance ?? db.NumberVariance;

        //sb.Formula = Formula ?? sb.Formula;

        //#if REALM
        //        LocalPosition? Position = null
        //#else
        //        Position? Position = null
        //#endif
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
                new ExcelMapper().Save(path, new List<SpellCustomization>() { new(SpellId.StrengthSelf1, SpellId.StrengthSelf1, "Buff Buff Self I", StatModVal: 7) }, "Spells");
                ModManager.Log($"Created: {path}");
            }

            //Copy to a temp file to avoid locks?  Not sure why Read access fails while copying works
            var loadedPath = $"{path}.load";

            //Could use temp files?
            //Path.GetTempFileName()

            File.Copy(path, loadedPath, true);
            //var stream = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            excel = new(loadedPath);
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
        //ItemType? NonComponentTargetType
        excel.AddMapping<SpellCustomization>(nameof(b.NonComponentTargetType), p => p.NonComponentTargetType)
            .SetPropertyUsing(cellValue => TryParseCellEnum<ItemType>(cellValue, out var parsed) ? parsed : null);
        //PropertyAttribute2nd? Source
        excel.AddMapping<SpellCustomization>(nameof(b.Source), p => p.Source)
            .SetPropertyUsing(cellValue => TryParseCellEnum<PropertyAttribute2nd>(cellValue, out var parsed) ? parsed : null);
        //PropertyAttribute2nd? Destination
        excel.AddMapping<SpellCustomization>(nameof(b.Destination), p => p.Destination)
            .SetPropertyUsing(cellValue => TryParseCellEnum<PropertyAttribute2nd>(cellValue, out var parsed) ? parsed : null);
        //TransferFlags? TransferFlags
        excel.AddMapping<SpellCustomization>(nameof(b.TransferFlags), p => p.TransferFlags)
            .SetPropertyUsing(cellValue => TryParseCellEnum<TransferFlags>(cellValue, out var parsed) ? parsed : null);
        //MagicSchool? DispelSchool
        excel.AddMapping<SpellCustomization>(nameof(b.DispelSchool), p => p.DispelSchool)
            .SetPropertyUsing(cellValue => TryParseCellEnum<MagicSchool>(cellValue, out var parsed) ? parsed : null);
        //DispelType? Align
        excel.AddMapping<SpellCustomization>(nameof(b.Align), p => p.Align)
            .SetPropertyUsing(cellValue => TryParseCellEnum<DispelType>(cellValue, out var parsed) ? parsed : null);

        //Vector3? CreateOffset
        //excel.AddMapping<SpellCustomization>(nameof(b.CreateOffset), p => p.CreateOffset)
        //    .SetCellUsing<Vector3>((c, o) =>
        //    {
        //        if (o == default) c.SetCellValue(""); else c.SetCellValue(o.Serialize());
        //    })
        //    .SetPropertyUsing(cellValue => TryParseVector3(cellValue, out var parsed) ? parsed : null);
        ////Vector3? Padding
        //excel.AddMapping<SpellCustomization>(nameof(b.Padding), p => p.Padding)
        //    .SetCellUsing<Vector3>((c, o) =>
        //    {
        //        if (o == default) c.SetCellValue(""); else c.SetCellValue(o.Serialize());
        //    })
        //    .SetPropertyUsing(cellValue => TryParseVector3(cellValue, out var parsed) ? parsed : null);
        ////Vector3? Peturbation
        //excel.AddMapping<SpellCustomization>(nameof(b.Peturbation), p => p.Peturbation)
        //    .SetCellUsing<Vector3>((c, o) =>
        //    {
        //        if (o == default) c.SetCellValue(""); else c.SetCellValue(o.Serialize());
        //    })
        //    .SetPropertyUsing(cellValue => TryParseVector3(cellValue, out var parsed) ? parsed : null);

        //List<uint> Formula
        //excel.AddMapping<SpellCustomization>(nameof(b.Formula), p => p.Formula)
        //    .SetCellUsing<List<uint>>((c, o) =>
        //    {
        //        Debugger.Break();
        //        if (o == null || o.Count == 0) c.SetCellValue(""); 
        //        else c.SetCellValue(String.Join(',',o));
        //    })
        //    .SetPropertyUsing(cellValue => TryParseList<uint>(cellValue, out var parsed) ? parsed : null);


        //Ignore zeros
        excel.AddMapping<SpellCustomization>(nameof(b.PortalLifetime), p => p.PortalLifetime)
            .SetCellUsing<double?>((c, o) =>
            {
                if (o is null || o == 0) return;
                //c.SetCellValue("");
                else c.SetCellValue(o.ToString());
            });

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

    public static bool TryParseVector3(object vector, out Vector3 result)
    {
        result = default;
        if (vector is not string vectorString || string.IsNullOrEmpty(vectorString))
            return false;
            //throw new ArgumentException("Input string cannot be null or empty.", nameof(vectorString));

        string[] components = vectorString.Split(',');

        if (components.Length != 3)
            return false;
            //throw new FormatException("Input string is not in the correct format for a Vector3.");

        float x = float.Parse(components[0]);
        float y = float.Parse(components[1]);
        float z = float.Parse(components[2]);

        result = new Vector3(x, y, z);

        return true;
    }

    public static bool TryParseList<T>(object value,  out List<T> result, string separator = ",")
    {
        result = new List<T>();

        if (value is not string input || string.IsNullOrEmpty(input))
            return false; // Return an empty list if the input is null or empty

        // Split the input string by the separator
        string[] items = input.Split(separator);

        foreach (string item in items)
        {
            try
            {
                // Convert each item to the desired type T and add it to the list
                T converted = (T)Convert.ChangeType(item.Trim(), typeof(T), CultureInfo.InvariantCulture);
                result.Add(converted);
            }
            catch (Exception ex)
            {
                return false;
                //throw new FormatException($"Could not convert '{item}' to {typeof(T)}", ex);
            }
        }

        return true;
    }
}

public static class Vector3Serialization
{
    public static string Serialize(this Vector3 vector)
    {
        return $"{vector.X},{vector.Y},{vector.Z}";
    }


}