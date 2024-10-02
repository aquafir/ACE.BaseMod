namespace ACE.Shared.Augments;
public static class Augment
{
    public static bool TryAugmentBit(this WorldObject wo, AugmentType type, int key, Operation op, double value)
    {

        return true;
    }


    //Todo: random values?  Handle higher up
    public static bool TryAugment(this WorldObject wo, AugmentType type, int key, Operation op, double value)
    {
        if (wo is null) return false;

        //Get the current normalized property
        if (!wo.TryGetAugmentTargetValue(type, key, out var current))
            return false;


        double d;
        //If value was missing set a default based on the operation
        if (current is null)
        {
            current = op switch
            {
                Operation.Multiply => 1,
                _ => 0,
            };
        }

        

        //Hold the 
        double result = 0;
        //switch (op)
        //{
        //    case Operation.Assign:
        //        result = value;
        //        break;
        //    case Operation.Add:
        //        result = current.Value + value; ;
        //        break;
        //    case Operation.Multiply:
        //        result = current.Value * value;
        //        break;
        //    case Operation.BitSet:
        //        result |= Convert.ToInt64(current.Value);
        //        break;
        //    case Operation.BitClear:
        //        break;
        //    default:
        //        break;
        //}

        //Todo: think about this
        //Need to convert value back to long if setting bits?
        //Convert.ToInt64()

        var success = type switch
        {
            AugmentType.Int => wo.TryAugmentInt(op, key, value, ref result),
            AugmentType.Int64 => wo.TryAugmentInt64(op, key, value, ref result),
            AugmentType.Float => wo.TryAugmentFloat(op, key, value, ref result),
            AugmentType.Bool => wo.TryAugmentBool(op, key, value, ref result),
            AugmentType.DataID => wo.TryAugmentDataId(op, key, value, ref result),
            AugmentType.AttributeRanks => wo.TryAugmentAttributeRanks(op, key, value, ref result),
            AugmentType.AttributeStart => wo.TryAugmentAttributeStart(op, key, value, ref result),
            AugmentType.VitalRanks => wo.TryAugmentVitalRanks(op, key, value, ref result),
            AugmentType.VitalStart => wo.TryAugmentVitalStart(op, key, value, ref result),
            AugmentType.SkillRanks => wo.TryAugmentSkillRanks(op, key, value, ref result),
            //AugmentType.SkillStart => wo.TryAugmentSkillStart(op, key, value, ref change),
            _ => false,
        };

        return true;
    }


    /// <summary>
    /// Returns a value or null of an Augment target
    /// </summary>
    private static bool TryGetAugmentTargetValue(this WorldObject wo, AugmentType type, int key, out object? value)
    {
        value = null;

        if (wo is null)
            return false;

        //WorldObject properties
        if (type < AugmentType.AttributeRanks)
        {
            value = type switch
            {
                AugmentType.Int => wo.GetProperty((PropertyInt)key),
                AugmentType.Int64 => wo.GetProperty((PropertyInt64)key),
                AugmentType.Float => wo.GetProperty((PropertyFloat)key),
                AugmentType.Bool => wo.GetProperty((PropertyBool)key),
                AugmentType.DataID => wo.GetProperty((PropertyDataId)key),
                _ => null,
            };
        }
        //Creature properties
        else if (wo is Creature creature)
        {
            value = type switch
            {
                AugmentType.AttributeRanks => creature.Attributes.TryGetValue((PropertyAttribute)key, out var current) ? current.Ranks : null,
                AugmentType.AttributeStart => creature.Attributes.TryGetValue((PropertyAttribute)key, out var current) ? current.StartingValue : null,
                AugmentType.VitalRanks => creature.Vitals.TryGetValue((PropertyAttribute2nd)key, out var current) ? current.Ranks : null,
                AugmentType.VitalStart => creature.Vitals.TryGetValue((PropertyAttribute2nd)key, out var current) ? current.StartingValue : null,
                AugmentType.SkillRanks => creature.Skills.TryGetValue((Skill)key, out var current) ? current.Ranks : null,
                //AugmentType.SkillStart => creature.Skills.TryGetValue((Skill)key, out var current) ? current.StartingValue : null,

                _ => null,
            };
        }
        //Invalid target for type
        else
            return false;

        //return value is null;
        return true;
    }


    private static bool TryAugmentInt(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }
    private static bool TryAugmentInt64(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }
    private static bool TryAugmentFloat(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }
    private static bool TryAugmentBool(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }
    private static bool TryAugmentDataId(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }
    //Creature/Player-only
    private static bool TryAugmentVitalRanks(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }
    private static bool TryAugmentAttributeRanks(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }
    private static bool TryAugmentSkillRanks(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }

    private static bool TryAugmentVitalStart(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }
    private static bool TryAugmentAttributeStart(this WorldObject wo, Operation op, int key, double value, ref double change)
    {
        return true;
    }
    //private static bool TryAugmentSkillStart(this WorldObject wo, Operation op, int key, double value, ref double change) => true

}

public enum Operation
{
    Assign,
    Add,
    Subtract,
    Multiply,
    Divide,
    BitSet,
    BitClear,

    /*
     * value |= (1 << n); // Set the nth bit
     * value &= ~(1 << n); // Clear the nth bit
     * value ^= (1 << n); // Toggle the nth bit
     * (value & (1 << n)) != 0; // Check if the nth bit is set
     * 
     * checked { int result = int.MaxValue + 1; // Throws an OverflowException }
     */

    //Modulo,
    //BitShift,
    //BitAND,
    //BitOR,
    //BitXOR,

    //BitNOT,




    //Subtract,
    //Divide,
    //AtLeastAdd,
    //AtMostSubtract,
    //AddMultiply,
    //AddDivide,
    //SubtractMultiply,
    //SubtractDivide,
    //AssignAdd,
    //AssignSubtract,
    //AssignMultiply,
    //AssignDivide
}

public enum AugmentType
{
    Undef = 0,
    Int = 1,
    Int64 = 2,
    Float = 3,
    Bool = 4,
    DataID = 5,

    //Require Creature 
    AttributeRanks = 100,    //Ranks from experience
    AttributeStart = 101,    //Starting value
    VitalRanks = 102,
    VitalStart = 103,
    SkillRanks = 104,
    //SkillStart = 105,
    //SkillAdvancementClass = 106,

    //Unused
    //Position = 3,
    //String = 5,
    //IID = 7,
    //BodyDamageValue = 10,
    //BodyDamageVariance = 11,
    //BodyArmorValue = 12,
}

/**
 * Example usage:
 * wo.TryAugment(AugmentType.Int
 * 
 */
