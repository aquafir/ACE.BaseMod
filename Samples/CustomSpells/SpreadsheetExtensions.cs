using CustomSpells;
using Ganss.Excel;
using System.Globalization;

namespace CustomSpells;

public static class SpreadsheetExtensions
{
    public static void SetupCustomSpellMappings(this ExcelMapper excel)
    {
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
            .SetPropertyUsing(cellValue => TryParseCellEnum<MagicSchool>(cellValue, out var parsed, false) ? parsed : null);
            //Allows other schools for patch
            //.SetPropertyUsing(cellValue => TryParseCellEnum<MagicSchool>(cellValue, out var parsed, true) ? parsed : null);
        //SpellCategory Category
        excel.AddMapping<SpellCustomization>(nameof(b.Category), p => p.Category)
            .SetPropertyUsing(cellValue => TryParseCellEnum<SpellCategory>(cellValue, out var parsed) ? parsed : null);
        //SpellFlags Bitfield
        excel.AddMapping<SpellCustomization>(nameof(b.Bitfield), p => p.Bitfield)
            .SetPropertyUsing(cellValue => TryParseCellEnum<SpellFlags>(cellValue, out var parsed) ? parsed : null);
        //SpellType MetaSpellType
        excel.AddMapping<SpellCustomization>(nameof(b.MetaSpellType), p => p.MetaSpellType)
            .SetPropertyUsing(cellValue => TryParseCellEnum<SpellType>(cellValue, out var parsed) ? parsed : null);
        //EnchantmentTypeFlags? StatModType
        excel.AddMapping<SpellCustomization>(nameof(b.StatModType), p => p.StatModType)
            .SetCellUsing<EnchantmentTypeFlags?>((c, o) => { if (o is not null && o != default) c.SetCellValue(o.ToString()); })
            .SetPropertyUsing(cellValue => TryParseCellEnum<EnchantmentTypeFlags>(cellValue, out var parsed) ? parsed : null);
        //DamageType? EType
        excel.AddMapping<SpellCustomization>(nameof(b.EType), p => p.EType)
            .SetCellUsing<DamageType?>((c, o) => { if (o is not null && o != default) c.SetCellValue(o.ToString()); })
            .SetPropertyUsing(cellValue => TryParseCellEnum<DamageType>(cellValue, out var parsed, true) ? parsed : null);
        //DamageType? DamageType
        excel.AddMapping<SpellCustomization>(nameof(b.DamageType), p => p.DamageType)
            .SetCellUsing<DamageType?>((c, o) => { if (o is not null && o != default) c.SetCellValue(o.ToString()); })
            .SetPropertyUsing(cellValue => TryParseCellEnum<DamageType>(cellValue, out var parsed, true) ? parsed : null);
        //PlayScript? CasterEffect
        excel.AddMapping<SpellCustomization>(nameof(b.CasterEffect), p => p.CasterEffect)
            .SetCellUsing<PlayScript?>((c, o) => { if (o is not null && o != default) c.SetCellValue(o.ToString()); })
            .SetPropertyUsing(cellValue => TryParseCellEnum<PlayScript>(cellValue, out var parsed) ? parsed : null);
        //PlayScript? TargetEffect
        excel.AddMapping<SpellCustomization>(nameof(b.TargetEffect), p => p.TargetEffect)
            .SetCellUsing<PlayScript?>((c, o) => { if (o is not null && o != default) c.SetCellValue(o.ToString()); })
            .SetPropertyUsing(cellValue => TryParseCellEnum<PlayScript>(cellValue, out var parsed) ? parsed : null);
        //ItemType? NonComponentTargetType
        excel.AddMapping<SpellCustomization>(nameof(b.NonComponentTargetType), p => p.NonComponentTargetType)
            .SetCellUsing<ItemType?>((c, o) => { if (o is not null && o != default) c.SetCellValue(o.ToString()); })
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
        excel.AddMapping<SpellCustomization>(nameof(b.CreateOffset), p => p.CreateOffset)
            .SetCellUsing<Vector3>((c, o) =>
            {
                if (o == default) c.SetCellValue("");
                else c.SetCellValue(o.Serialize());
            })
            .SetPropertyUsing(cellValue => TryParseVector3(cellValue, out var parsed) ? parsed : null);
        //Vector3? Padding
        excel.AddMapping<SpellCustomization>(nameof(b.Padding), p => p.Padding)
            .SetCellUsing<Vector3>((c, o) =>
            {
                if (o == default) c.SetCellValue("");
                else c.SetCellValue(o.Serialize());
            })
            .SetPropertyUsing(cellValue => TryParseVector3(cellValue, out var parsed) ? parsed : null);
        //Vector3? Peturbation
        excel.AddMapping<SpellCustomization>(nameof(b.Peturbation), p => p.Peturbation)
            .SetCellUsing<Vector3>((c, o) =>
            {
                if (o == default) c.SetCellValue("");
                else c.SetCellValue(o.Serialize());
            })
            .SetPropertyUsing(cellValue => TryParseVector3(cellValue, out var parsed) ? parsed : null);

        //List<uint> Formula
        excel.AddMapping<SpellCustomization>(nameof(b.Formula), p => p.Formula)
            .SetCellUsing<List<uint>>((c, o) =>
            {
                if (o == null || o.Count == 0) c.SetCellValue("");
                else c.SetCellValue(string.Join(',', o));
            })
            .SetPropertyUsing(cellValue => TryParseList<uint>(cellValue, out var parsed) ? parsed : null);

        //Position Position
        excel.AddMapping<SpellCustomization>(nameof(b.Position), p => p.Position)
            .SetCellUsing<Position>((c, o) =>
            {
                //Todo: check if LOCString is a Realms thing
                if (o == null || o.LandblockId == default) c.SetCellValue("");
                else c.SetCellValue(o.ToLOCString());
            })
            .SetPropertyUsing(cellValue => TryParsePosition(cellValue, out var parsed) ? parsed : null);



        //Skip defaults?

        //Ignore zeros
        excel.AddMapping<SpellCustomization>(nameof(b.PortalLifetime), p => p.PortalLifetime)
            .SetCellUsing<double?>((c, o) =>
            {
                if (o is null || o == 0) return;
                else c.SetCellValue(o.ToString());
            });
        excel.AddMapping<SpellCustomization>(nameof(b.Duration), p => p.Duration)
            .SetCellUsing<double?>((c, o) =>
            {
                if (o is null || o == 0) return;
                else c.SetCellValue(o.ToString());
            });
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

    public static bool TryParseList<T>(object value, out List<T> result, string separator = ",")
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

    public static bool TryParsePosition(object value, out Position result)
    {
        result = null;

        if (value is not string input || string.IsNullOrEmpty(input))
            return false;

        return input.TryParsePosition(out result);
    }

    public static string Serialize(this Vector3 vector) => $"{vector.X},{vector.Y},{vector.Z}";
}
