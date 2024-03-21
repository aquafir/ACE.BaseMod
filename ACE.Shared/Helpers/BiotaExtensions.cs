namespace ACE.Shared.Helpers;

public static class BiotaExtensions
{
    /// <summary>
    /// Converts an Entity Biota to an Entity Weenie
    /// </summary>
    public static ACE.Entity.Models.Weenie ToWeenie(this Entity.Models.Biota biota, bool includeEphemeral = false)
    {
        //ACE.Database.Models.World.Weenie result = new();
        ACE.Entity.Models.Weenie result = new();
        result.WeenieClassId = biota.WeenieClassId;
        result.ClassName = biota.GetName();
        result.WeenieType = biota.WeenieType;

        //Props
        if (biota.PropertiesBool is not null)
            result.PropertiesBool = biota.PropertiesBool
                .Where(x => includeEphemeral || !EphemeralProperties.PropertiesBool.Contains(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);

        if (biota.PropertiesDID is not null)
            result.PropertiesDID = biota.PropertiesDID
                .Where(x => includeEphemeral || !EphemeralProperties.PropertiesDataId.Contains(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);

        if (biota.PropertiesFloat is not null)
            result.PropertiesFloat = biota.PropertiesFloat
                .Where(x => includeEphemeral || !EphemeralProperties.PropertiesDouble.Contains(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);

        if (biota.PropertiesIID is not null)
            result.PropertiesIID = biota.PropertiesIID
                .Where(x => includeEphemeral || !EphemeralProperties.PropertiesInstanceId.Contains(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);

        if (biota.PropertiesInt is not null)
            result.PropertiesInt = biota.PropertiesInt
                .Where(x => includeEphemeral || !EphemeralProperties.PropertiesInt.Contains(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);

        if (biota.PropertiesInt64 is not null)
            result.PropertiesInt64 = biota.PropertiesInt64
                .Where(x => includeEphemeral || !EphemeralProperties.PropertiesInt64.Contains(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);

        if (biota.PropertiesString is not null)
            result.PropertiesString = biota.PropertiesString
                .Where(x => includeEphemeral || !EphemeralProperties.PropertiesString.Contains(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);

        //Don't want positions?
        if (biota.PropertiesPosition is not null)
            result.PropertiesPosition = biota.PropertiesPosition
                .Where(x => includeEphemeral || !EphemeralProperties.PositionTypes.Contains(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);

        //Or ObjData?
        if (biota.PropertiesAnimPart is not null)
            result.PropertiesAnimPart = biota.PropertiesAnimPart.Clone();
        if (biota.PropertiesPalette is not null)
            result.PropertiesPalette = biota.PropertiesPalette.Clone();
        if (biota.PropertiesTextureMap is not null)
            result.PropertiesTextureMap = biota.PropertiesTextureMap.Clone();

        if (biota.PropertiesSpellBook is not null)
            result.PropertiesSpellBook = biota.PropertiesSpellBook.Clone();
        if (biota.PropertiesCreateList is not null)
            result.PropertiesCreateList = biota.PropertiesCreateList.Clone();
        if (biota.PropertiesEmote is not null)
            result.PropertiesEmote = biota.PropertiesEmote.Clone();
        if (biota.PropertiesEventFilter is not null)
            result.PropertiesEventFilter = biota.PropertiesEventFilter.Clone();
        if (biota.PropertiesGenerator is not null)
            result.PropertiesGenerator = biota.PropertiesGenerator.Clone();

        //Creature things
        if (biota.PropertiesAttribute is not null)
            result.PropertiesAttribute = biota.PropertiesAttribute.Clone();
        if (biota.PropertiesAttribute2nd is not null)
            result.PropertiesAttribute2nd = biota.PropertiesAttribute2nd.Clone();
        if (biota.PropertiesBodyPart is not null)
            result.PropertiesBodyPart = biota.PropertiesBodyPart.Clone();
        if (biota.PropertiesSkill is not null)
            result.PropertiesSkill = biota.PropertiesSkill.Clone();
        if (biota.PropertiesBook is not null)
            result.PropertiesBook = biota.PropertiesBook.Clone();
        if (biota.PropertiesBookPageData is not null)
            result.PropertiesBookPageData = biota.PropertiesBookPageData.Clone();

        return result;
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
