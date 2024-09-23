namespace AutoLoot.Loot;

public class ValueRequirement
{
    /// <summary>
    /// Type of property being evaluated
    /// </summary>
    public ValueProp PropType { get; set; }

    /// <summary>
    /// Enum key of the PropertyType being evaluated
    /// </summary>
    public int PropKey { get; set; }

    /// <summary>
    /// Value normalized like RecipeManager uses?
    /// </summary>
    public double TargetValue { get; set; }

    /// <summary>
    /// Method of comparison
    /// </summary>
    public CompareType Type { get; set; }


    /// <summary>
    /// Finds and normalizes the value corresponding to a property type and key on a WorldObject and compares it to the required value
    /// </summary>
    public bool VerifyRequirement(WorldObject item)
    {
        //Null or double value
        var normalizedValue = PropType switch
        {
            ValueProp.PropertyBool => item.GetProperty((PropertyBool)PropKey).Normalize(),
            ValueProp.PropertyDataId => item.GetProperty((PropertyDataId)PropKey).Normalize(),
            ValueProp.PropertyDouble => item.GetProperty((PropertyFloat)PropKey).Normalize(),
            ValueProp.PropertyInstanceId => item.GetProperty((PropertyInstanceId)PropKey).Normalize(),
            ValueProp.PropertyInt => item.GetProperty((PropertyInt)PropKey).Normalize(),
            ValueProp.PropertyInt64 => item.GetProperty((PropertyInt64)PropKey).Normalize(),
            _ => throw new NotImplementedException(),
        };
        //Debugger.Break();
        return VerifyRequirement(normalizedValue);
    }

    /// <summary>
    /// True if a WorldObject's value succeeds in a comparison with a target value
    /// </summary>
    public bool VerifyRequirement(double? prop)
    {

        return Type switch
        {
            CompareType.GreaterThan => (prop ?? 0) > TargetValue,
            CompareType.GreaterThanEqual => (prop ?? 0) >= TargetValue,
            CompareType.LessThan => (prop ?? 0) < TargetValue,
            CompareType.LessThanEqual => (prop ?? 0) <= TargetValue,
            CompareType.NotEqual => (prop ?? 0) != TargetValue,
            CompareType.Equal => (prop ?? 0) == TargetValue,
            CompareType.NotEqualNotExist => prop == null || prop.Value != TargetValue,    //Todo, not certain about the inversion.  I'm tired.
            CompareType.NotExist => prop is null,
            CompareType.Exist => prop is not null,
            CompareType.NotHasBits => ((int)(prop ?? 0) & (int)TargetValue) == 0,
            CompareType.HasBits => ((int)(prop ?? 0) & (int)TargetValue) == (int)TargetValue,
            _ => true,
        };

        //var success = compareType switch
        //{
        //    CompareType.GreaterThan => !((prop ?? 0) > val),
        //    CompareType.LessThanEqual => !((prop ?? 0) <= val),
        //    CompareType.LessThan => !((prop ?? 0) < val),
        //    CompareType.GreaterThanEqual => !((prop ?? 0) >= val),
        //    CompareType.NotEqual => !((prop ?? 0) != val),
        //    CompareType.NotEqualNotExist => !(prop == null || prop.Value != val),
        //    CompareType.Equal => !((prop ?? 0) == val),
        //    CompareType.NotExist => !(prop == null),
        //    CompareType.Exist => !(prop != null),
        //    CompareType.NotHasBits => !(((int)(prop ?? 0) & (int)val) == 0),
        //    CompareType.HasBits => !(((int)(prop ?? 0) & (int)val) == (int)val),
        //    _ => true,
        //};
        //return success;
    }
}
