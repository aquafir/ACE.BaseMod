namespace ACE.Shared.Helpers;
public static class EnumExtensions
{
    //Nullable
    public static bool TryConvertToEnum<T>(this object value, out T result, bool requireDefined = false) where T : struct, Enum
    {
        result = default;

        //Try to parse it as a string
        bool parsedNum = false;
        if (value is string stringValue)
        {
            //Directly
            if (Enum.TryParse(stringValue, true, out result))
                return !requireDefined || Enum.IsDefined(result);
        }

        //Check if already the enum
        if(value is T enumValue)
        {
            result = enumValue;
            return !requireDefined || Enum.IsDefined(result); 
        }

        // Otherwise try to convert a parsed number of original cell value to the Enum type?
        if (value is IConvertible convertible)
        {
            try
            {
                // Convert the input to the underlying type of the enum
                Type enumType = typeof(T);
                Type underlyingType = Enum.GetUnderlyingType(enumType);
                object convertedValue = convertible.ToType(underlyingType, null);

                // Cast the converted value to the enum type
                result = (T)convertedValue; //: (T)Enum.ToObject(enumType, convertedValue);
                return !requireDefined || Enum.IsDefined(result); ;
            }
            catch { }
        }

        return false;
    }

    //public static bool TryConvertToDefinedEnum<T>(this object value, out T? parsed) where T : struct, Enum
    //{
    //    parsed = null;
    //    //default(T);

    //    //Require it being defined?
    //    if (!Enum.IsDefined(typeof(T), value))
    //    {
    //        parsed = null;
    //        return false;
    //    }

    //    return value.TryConvertToEnum<T>(out parsed);
    //}

    //public static bool TryConvertToEnum<T>(this object value, out T parsed) where T : struct, Enum
    //{
    //    parsed = default(T);

    //    try
    //    {
    //        parsed = (T)Enum.ToObject(typeof(T), value);
    //        return true;
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }
    //}
    //public static bool TryConvertToDefinedEnum<T>(this object value, out T parsed) where T : struct, Enum
    //{
    //    parsed = default(T);

    //    //Require it being defined?
    //    if (!Enum.IsDefined(typeof(T), value))
    //        return false;

    //    return value.TryConvertToEnum<T>(out parsed);
    //}
}
