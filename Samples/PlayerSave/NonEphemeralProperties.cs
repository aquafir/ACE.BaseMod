using ACE.Entity.Enum.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSave
{
    //Todo: think about adding [ServerOnly] or [SendOnLogin]
    //public static class NonEphemeralProperties
    //{
    //    /// <summary>
    //    /// Method to return a list of enums by attribute type - in this case [Ephemeral] using generics to enhance code reuse.
    //    /// </summary>
    //    /// <typeparam name="T">Enum to list by [Ephemeral]</typeparam>
    //    /// <typeparam name="TResult">Type of the results</typeparam>
    //    private static HashSet<T> GetValues<T>()
    //    {
    //        var list = typeof(T).GetFields().Select(x => new
    //        {
    //            att = x.GetCustomAttributes(false).OfType<EphemeralAttribute>().FirstOrDefault(),
    //            member = x
    //        }).Where(x => x.att is null && x.member.Name != "value__").Select(x => (T)x.member.GetValue(null)).ToList();

    //        return new HashSet<T>(list);
    //    }

    //    /// <summary>
    //    /// returns a list of values for PropertyInt that aren't [Ephemeral]
    //    /// </summary
    //    public static HashSet<PropertyInt> PropertiesInt = GetValues<PropertyInt>();

    //    /// <summary>
    //    /// returns a list of values for PropertyInt64 that aren't [Ephemeral]
    //    /// </summary>
    //    public static HashSet<PropertyInt64> PropertiesInt64 = GetValues<PropertyInt64>();

    //    /// <summary>
    //    /// returns a list of values for PropertyBool that aren't [Ephemeral]
    //    /// </summary>
    //    public static HashSet<PropertyBool> PropertiesBool = GetValues<PropertyBool>();

    //    /// <summary>
    //    /// returns a list of values for PropertyString that aren't [Ephemeral]
    //    /// </summary>
    //    public static HashSet<PropertyString> PropertiesString = GetValues<PropertyString>();

    //    /// <summary>
    //    /// returns a list of values for PropertyFloat that aren't [Ephemeral]
    //    /// </summary>
    //    public static HashSet<PropertyFloat> PropertiesDouble = GetValues<PropertyFloat>();

    //    /// <summary>
    //    /// returns a list of values for PropertyDataId that aren't [Ephemeral]
    //    /// </summary>
    //    public static HashSet<PropertyDataId> PropertiesDataId = GetValues<PropertyDataId>();

    //    /// <summary>
    //    /// returns a list of values for PropertyInstanceId that aren't [Ephemeral]
    //    /// </summary>
    //    public static HashSet<PropertyInstanceId> PropertiesInstanceId = GetValues<PropertyInstanceId>();


    //    /// <summary>
    //    /// returns a list of values for PositionType that aren't [Ephemeral]
    //    /// </summary>
    //    public static HashSet<PositionType> PositionTypes = GetValues<PositionType>();
    //}
}
