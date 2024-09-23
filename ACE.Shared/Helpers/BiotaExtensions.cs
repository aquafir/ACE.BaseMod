namespace ACE.Shared.Helpers;

public static class BiotaExtensions
{
    public static Weenie ToWeenie(this Entity.Models.Biota biota, bool includeEphemeral = false)
    {
        Weenie weenie = new Weenie();
        weenie.WeenieClassId = biota.WeenieClassId;
        weenie.ClassName = biota.GetName();
        weenie.WeenieType = biota.WeenieType;
        if (biota.PropertiesBool != null)
        {
            weenie.PropertiesBool = biota.PropertiesBool.Where((KeyValuePair<PropertyBool, bool> x) => includeEphemeral || !EphemeralProperties.PropertiesBool.Contains(x.Key)).ToDictionary((KeyValuePair<PropertyBool, bool> x) => x.Key, (KeyValuePair<PropertyBool, bool> y) => y.Value);
        }

        if (biota.PropertiesDID != null)
        {
            weenie.PropertiesDID = biota.PropertiesDID.Where((KeyValuePair<PropertyDataId, uint> x) => includeEphemeral || !EphemeralProperties.PropertiesDataId.Contains(x.Key)).ToDictionary((KeyValuePair<PropertyDataId, uint> x) => x.Key, (KeyValuePair<PropertyDataId, uint> y) => y.Value);
        }

        if (biota.PropertiesFloat != null)
        {
            weenie.PropertiesFloat = biota.PropertiesFloat.Where((KeyValuePair<PropertyFloat, double> x) => includeEphemeral || !EphemeralProperties.PropertiesDouble.Contains(x.Key)).ToDictionary((KeyValuePair<PropertyFloat, double> x) => x.Key, (KeyValuePair<PropertyFloat, double> y) => y.Value);
        }

        if (biota.PropertiesIID != null)
        {
#if REALM
            weenie.PropertiesIID = biota.PropertiesIID.Where((KeyValuePair<PropertyInstanceId, ulong> x) => includeEphemeral || !EphemeralProperties.PropertiesInstanceId.Contains(x.Key)).ToDictionary((KeyValuePair<PropertyInstanceId, ulong> x) => x.Key, (KeyValuePair<PropertyInstanceId, ulong> y) => y.Value);
#else
            weenie.PropertiesIID = biota.PropertiesIID.Where((KeyValuePair<PropertyInstanceId, uint> x) => includeEphemeral || !EphemeralProperties.PropertiesInstanceId.Contains(x.Key)).ToDictionary((KeyValuePair<PropertyInstanceId, uint> x) => x.Key, (KeyValuePair<PropertyInstanceId, uint> y) => y.Value);
#endif
        }

        if (biota.PropertiesInt != null)
        {
            weenie.PropertiesInt = biota.PropertiesInt.Where((KeyValuePair<PropertyInt, int> x) => includeEphemeral || !EphemeralProperties.PropertiesInt.Contains(x.Key)).ToDictionary((KeyValuePair<PropertyInt, int> x) => x.Key, (KeyValuePair<PropertyInt, int> y) => y.Value);
        }

        if (biota.PropertiesInt64 != null)
        {
            weenie.PropertiesInt64 = biota.PropertiesInt64.Where((KeyValuePair<PropertyInt64, long> x) => includeEphemeral || !EphemeralProperties.PropertiesInt64.Contains(x.Key)).ToDictionary((KeyValuePair<PropertyInt64, long> x) => x.Key, (KeyValuePair<PropertyInt64, long> y) => y.Value);
        }

        if (biota.PropertiesString != null)
        {
            weenie.PropertiesString = biota.PropertiesString.Where((KeyValuePair<PropertyString, string> x) => includeEphemeral || !EphemeralProperties.PropertiesString.Contains(x.Key)).ToDictionary((KeyValuePair<PropertyString, string> x) => x.Key, (KeyValuePair<PropertyString, string> y) => y.Value);
        }

        if (biota.PropertiesPosition != null)
        {
            weenie.PropertiesPosition = biota.PropertiesPosition.Where((KeyValuePair<PositionType, PropertiesPosition> x) => includeEphemeral || !EphemeralProperties.PositionTypes.Contains(x.Key)).ToDictionary((KeyValuePair<PositionType, PropertiesPosition> x) => x.Key, (KeyValuePair<PositionType, PropertiesPosition> y) => y.Value);
        }

        if (biota.PropertiesAnimPart != null)
        {
            weenie.PropertiesAnimPart = biota.PropertiesAnimPart.Clone();
        }

        if (biota.PropertiesPalette != null)
        {
            weenie.PropertiesPalette = biota.PropertiesPalette.Clone();
        }

        if (biota.PropertiesTextureMap != null)
        {
            weenie.PropertiesTextureMap = biota.PropertiesTextureMap.Clone();
        }

        if (biota.PropertiesSpellBook != null)
        {
            weenie.PropertiesSpellBook = biota.PropertiesSpellBook.Clone();
        }

        if (biota.PropertiesCreateList != null)
        {
            weenie.PropertiesCreateList = biota.PropertiesCreateList.Clone();
        }

        if (biota.PropertiesEmote != null)
        {
            weenie.PropertiesEmote = biota.PropertiesEmote.Clone();
        }

        if (biota.PropertiesEventFilter != null)
        {
            weenie.PropertiesEventFilter = biota.PropertiesEventFilter.Clone();
        }

        if (biota.PropertiesGenerator != null)
        {
            weenie.PropertiesGenerator = biota.PropertiesGenerator.Clone();
        }

        if (biota.PropertiesAttribute != null)
        {
            weenie.PropertiesAttribute = biota.PropertiesAttribute.Clone();
        }

        if (biota.PropertiesAttribute2nd != null)
        {
            weenie.PropertiesAttribute2nd = biota.PropertiesAttribute2nd.Clone();
        }

        if (biota.PropertiesBodyPart != null)
        {
            weenie.PropertiesBodyPart = biota.PropertiesBodyPart.Clone();
        }

        if (biota.PropertiesSkill != null)
        {
            weenie.PropertiesSkill = biota.PropertiesSkill.Clone();
        }

        if (biota.PropertiesBook != null)
        {
            weenie.PropertiesBook = biota.PropertiesBook.Clone();
        }

        if (biota.PropertiesBookPageData != null)
        {
            weenie.PropertiesBookPageData = biota.PropertiesBookPageData.Clone();
        }

        return weenie;
    }

    public static bool IsNpc(this ACE.Entity.Models.Biota biota)
    {
        //Assume npc
        if (biota is null || biota.PropertiesBool is null || biota.PropertiesInt is null) return true;

        //IsNPC => !(this is Player) && !Attackable && TargetingTactic == TargetingTactic.None;
        //Attackable = GetProperty(PropertyBool.Attackable) ?? true;
        if (!biota.PropertiesBool.TryGetValue(PropertyBool.Attackable, out var value) || value != false)
            return false;

        //TargetingTactic = (TargetingTactic)(GetProperty(PropertyInt.TargetingTactic) ?? 0);
        if (!biota.PropertiesInt.TryGetValue(PropertyInt.TargetingTactic, out var target) || target != (int)ACE.Entity.Enum.TargetingTactic.None)
            return false;

        return true;
    }
}
