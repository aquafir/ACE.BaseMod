namespace Expansion.Features;

[CommandCategory(nameof(Feature.BonusStats))]
[HarmonyPatchCategory(nameof(Feature.BonusStats))]
public class BonusStats
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureSkill), nameof(CreatureSkill.InitLevel), MethodType.Getter)]
    public static void PostGetInitLevel(ref CreatureSkill __instance, ref uint __result)
    {
        //Add on the alternative levels to the InitLevel?
        //Uses Krafs publicizer to get access to CreatureSkill.creature
        __result += (uint)__instance.creature.GetBonus(__instance.Skill);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureVital), nameof(CreatureVital.StartingValue), MethodType.Getter)]
    public static void PostGetStartingValue(ref CreatureVital __instance, ref uint __result)
    {
        __result += (uint)__instance.creature.GetBonus(__instance.Vital);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureAttribute), nameof(CreatureAttribute.StartingValue), MethodType.Getter)]
    public static void PostGetStartingValue(ref CreatureAttribute __instance, ref uint __result)
    {
        __result += (uint)__instance.creature.GetBonus(__instance.Attribute);
    }

    //Test command
    //[CommandHandler("bonus", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    //public static void Sim(Session session, params string[] parameters)
    //{
    //    var player = session.Player;
    //    player.IncBonus(PropertyAttribute.Strength, 3);
    //    player.SendUpdated(PropertyAttribute.Strength);
    //    player.IncBonus(PropertyAttribute2nd.MaxHealth, 3);
    //    player.SendUpdated(PropertyAttribute2nd.MaxHealth);
    //    player.IncBonus(Skill.Alchemy, 3);
    //    player.SendUpdated(Skill.Alchemy);
    //}
}
