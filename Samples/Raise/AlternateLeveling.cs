[HarmonyPatchCategory(nameof(AlternateLeveling))]
public class AlternateLeveling
{
    #region Intercept Raise / Leveling
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionRaiseSkill), new Type[] { typeof(Skill), typeof(uint) })]
    public static bool PreHandleActionRaiseSkill(Skill skill, uint amount, ref Player __instance, ref bool __result)
    {
        //Pretend amount is the number of levels
        amount = amount > 300 ? 10 : 1u;

        for (var i = 0; i < amount; i++)
            if (!__instance.TryRaiseSkill(skill))
                break;

        //Try to always update?
        //__instance.Session.Network.EnqueueSend(new GameMessagePrivateUpdateSkill(__instance, __instance.GetCreatureSkill(skill, false)));
        __instance.SendUpdated(skill);

        //Skip original handling
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionRaiseAttribute), new Type[] { typeof(PropertyAttribute), typeof(uint) })]
    public static bool PreHandleActionRaiseAttribute(PropertyAttribute attribute, uint amount, ref Player __instance, ref bool __result)
    {
        //Pretend amount is the number of levels
        amount = amount > 300 ? 10 : 1u;

        for (var i = 0; i < amount; i++)
            if (!__instance.TryRaiseAttribute(attribute))
                break;

        if (__instance.Attributes.TryGetValue(attribute, out var creatureAttribute))
            //__instance.Session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(__instance, creatureAttribute));
            __instance.SendUpdated(creatureAttribute);

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionRaiseVital), new Type[] { typeof(PropertyAttribute2nd), typeof(uint) })]
    public static bool PreHandleActionRaiseVital(PropertyAttribute2nd vital, uint amount, ref Player __instance, ref bool __result)
    {
        //Pretend amount is the number of levels
        amount = amount > 300 ? 10 : 1u;

        for (var i = 0; i < amount; i++)
            if (!__instance.TryRaiseVital(vital))
                break;

        if (__instance.Vitals.TryGetValue(vital, out var creatureVital))
            __instance.SendUpdated(creatureVital);
            //__instance.Session.Network.EnqueueSend(new GameMessagePrivateUpdateVital(__instance, creatureVital));

        return false;
    }
    #endregion


    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureSkill), nameof(CreatureSkill.InitLevel), MethodType.Getter)]
    public static void PostGetInitLevel(ref CreatureSkill __instance, ref uint __result)
    {
        //Add on the alternative levels to the InitLevel?
        //Uses Krafs publicizer to get access to CreatureSkill.creature
        __result += (uint)__instance.creature.GetLevel(__instance.Skill);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureVital), nameof(CreatureVital.StartingValue), MethodType.Getter)]
    public static void PostGetStartingValue(ref CreatureVital __instance, ref uint __result)
    {
        __result += (uint)__instance.creature.GetLevel(__instance.Vital);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureAttribute), nameof(CreatureAttribute.StartingValue), MethodType.Getter)]
    public static void PostGetStartingValue(ref CreatureAttribute __instance, ref uint __result)
    {
        __result += (uint)__instance.creature.GetLevel(__instance.Attribute);
    }

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(CreatureSkill), nameof(CreatureSkill.Base), MethodType.Getter)]
    //public static void PostGetBase(ref CreatureSkill __instance, ref uint __result)
    //{
    //    //Add to Base / Current / whatevs instead of Init
    //    //Not sure what makes most sense
    //    //__result += (uint)__instance.creature.GetSkillLevel(__instance.Skill);
    //}
}
