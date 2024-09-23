namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeEquipRestriction))]
[HarmonyPatchCategory(nameof(Feature.FakeEquipRestriction))]
internal class FakeEquipRestriction
{
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), nameof(Player.CheckWieldRequirements), new Type[] { typeof(WorldObject) })]
    //public static bool PreCheckWieldRequirements(WorldObject item, ref Player __instance, ref WeenieError __result)
    //{
    //    if (!__instance.CurrentLandblock.IsDungeon)
    //        return true;

    //    //var cs = item.GetProperty(FakeBool.CorpseSpawnedDungeon);
    //    //if (!(item.GetProperty(FakeBool.CorpseSpawnedDungeon) ?? false))
    //    //    return true;

    //    var lb = item.GetProperty(FakeDID.LocationLockId) ?? 0;
    //    if(lb != __instance.CurrentLandblock.Id.Raw) {
    //        __instance.SendMessage($"Unable to wield item that spawned in a different dungeon.\nFound: {lb:X4}\nIn: {__instance.CurrentLandblock.Id.Raw:X4}");

    //        //What message to use?
    //        __result = WeenieError.YouDoNotOwnThatItem; 
    //        return false;
    //    }
    //    __instance.SendMessage($"Able to wield item that spawned in a matching dungeon.\nFound: {lb:X4}\nIn: {__instance.CurrentLandblock.Id.Raw:X4}");

    //    //Do regular checks
    //    return true;
    //}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.CheckWieldRequirements), new Type[] { typeof(WorldObject) })]
    public static bool PreCheckWieldRequirementsCustom(WorldObject item, ref Player __instance, ref WeenieError __result)
    {
        //Get type
        var reqType = item.GetProperty(FakeInt.ItemWieldRequirementType1);
        if (reqType is null)
            return true;
        PropertyType propType = (PropertyType)reqType;

        //Get key
        var reqKey = item.GetProperty(FakeInt.ItemWieldRequirementKey1);
        if (reqKey is null)
            return true;

        //Get comparison
        var reqComparison = item.GetProperty(FakeInt.ItemWieldRequirementCompareType1);
        if (reqKey is null)
            return true;
        CompareType comparison = (CompareType)reqComparison;

        //Get normalized value of the key
        double? value = propType switch
        {
            //Base attribute
            PropertyType.PropertyAttribute => __instance.Attributes?.GetValueOrDefault((PropertyAttribute)reqKey)?.Base.Normalize(),
            PropertyType.PropertyAttribute2nd => __instance.Vitals?.GetValueOrDefault((PropertyAttribute2nd)reqKey)?.Base.Normalize(),
            //PropertyType.PropertyBook => __instance.GetProperty((PropertyBool)reqKey).Normalize(),
            PropertyType.PropertyBool => __instance.GetProperty((PropertyBool)reqKey).Normalize(),
            PropertyType.PropertyDataId => __instance.GetProperty((PropertyDataId)reqKey).Normalize(),
            PropertyType.PropertyDouble => __instance.GetProperty((PropertyFloat)reqKey).Normalize(),
            PropertyType.PropertyInstanceId => __instance.GetProperty((PropertyInstanceId)reqKey).Normalize(),
            PropertyType.PropertyInt => __instance.GetProperty((PropertyInt)reqKey).Normalize(),
            PropertyType.PropertyInt64 => __instance.GetProperty((PropertyInt64)reqKey).Normalize(),
            //PropertyType.PropertyString => __instance.GetProperty((PropertyBool)reqKey).Normalize(),
            //PropertyType.PropertyPosition => __instance.GetProperty((PropertyBool)reqKey).Normalize(),
            _ => null,
        };

        double? targetValue = item.GetProperty(FakeFloat.ItemWieldRequirementValue1);

        if (!((CompareType)reqComparison).VerifyRequirement(value, targetValue))
        {
            __instance.SendMessage($"Unable to wield {item.Name} without {propType}.{reqKey} {comparison.Friendly()} {targetValue}.  Currently {value}.");

            //What message to use?
            __result = WeenieError.BeWieldedFailure;
            return false;
        }
        __instance.SendMessage($"Passed requirement for {item.Name}: {propType}.{reqKey} {comparison.Friendly()} {targetValue}.  Currently {value}.");

        //Do regular checks
        return true;
    }
}


