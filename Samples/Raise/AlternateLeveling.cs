
using ACE.Shared.Helpers;
using System.Diagnostics;

[HarmonyPatchCategory(nameof(AlternateLeveling))]
[CommandCategory(nameof(AlternateLeveling))]
public class AlternateLeveling
{
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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureSkill), nameof(CreatureSkill.InitLevel), MethodType.Getter)]
    public static void PostGetInitLevel(ref CreatureSkill __instance, ref uint __result)
    {
        //Add on the alternative levels to the InitLevel?
        //Uses Krafs publicizer to get access to CreatureSkill.creature
        __result += (uint)__instance.creature.GetLevel(__instance.Skill);
    }

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(CreatureSkill), nameof(CreatureSkill.Base), MethodType.Getter)]
    //public static void PostGetBase(ref CreatureSkill __instance, ref uint __result)
    //{
    //    //Add to Base / Current / whatevs instead of Init
    //    //Not sure what makes most sense
    //    //__result += (uint)__instance.creature.GetSkillLevel(__instance.Skill);
    //}


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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureVital), nameof(CreatureVital.StartingValue), MethodType.Getter)]
    public static void PostGetStartingValue(ref CreatureVital __instance, ref uint __result)
    {
        __result += (uint)__instance.creature.GetLevel(__instance.Vital);
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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureAttribute), nameof(CreatureAttribute.StartingValue), MethodType.Getter)]
    public static void PostGetStartingValue(ref CreatureAttribute __instance, ref uint __result)
    {
        __result += (uint)__instance.creature.GetLevel(__instance.Attribute);
    }


    const int spacing = -20;
    [CommandHandler("levels", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleLevels(Session session, params string[] parameters)
    {
        var player = session.Player;

        var sb = new StringBuilder();

        sb.Append($"\n====================Attributes====================");
        sb.Append($"\n{"Level",spacing}{"Next",spacing}{"Total",spacing}{"Name",spacing}");
        foreach (var attr in Enum.GetValues<PropertyAttribute>().OrderBy(x => x.ToString()))
        {
            //Skip skills you can't level?
            if (!player.TryGetAttributeCost(attr, out var cost))
                continue;
            var total = player.GetCost(attr);
            var level = player.GetLevel(attr);
            sb.Append($"\n{attr,spacing}{level,spacing}{cost,spacing}{total,spacing}");
        }

        sb.Append($"\n\n====================Vitals====================");
        sb.Append($"\n{"Level",spacing}{"Next",spacing}{"Total",spacing}{"Name",spacing}");
        foreach (var attr in Enum.GetValues<PropertyAttribute2nd>().OrderBy(x => x.ToString()))
        {
            //Skip skills you can't level?
            if (!attr.ToString().StartsWith("Max") || !player.TryGetVitalCost(attr, out var cost))
                continue;
            var total = player.GetCost(attr);
            var level = player.GetLevel(attr);
            sb.Append($"\n{attr,spacing}{level,spacing}{cost,spacing}{total,spacing}");
        }

        sb.Append($"\n\n====================Skills====================");
        sb.Append($"\n{"Level",spacing}{"Next",spacing}{"Total",spacing}{"Name",spacing}");
        foreach (var attr in Enum.GetValues<Skill>().OrderBy(x => x.ToString()))
        {
            //Skip skills you can't level?
            if (!player.TryGetSkillCost(attr, out var cost))
                continue;
            var total = player.GetCost(attr);
            var level = player.GetLevel(attr);
            sb.Append($"\n{attr,spacing}{level,spacing}{cost,spacing}{total,spacing}");
        }

        player.SendMessage(sb.ToString());
    }

    [CommandHandler("refund", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleRefund(Session session, params string[] parameters)
    {
        var player = session.Player;

        long refund = 0;

        var sb = new StringBuilder();
        foreach (var attr in Enum.GetValues<PropertyAttribute>().OrderBy(x => x.ToString()))
        {
            var total = player.GetCost(attr);
            if (total <= 0)
                continue;

            var level = player.GetLevel(attr);
            player.SetLevel(attr, 0);
            player.SetCost(attr, 0);
            player.SendUpdated(attr);
            refund += total;

            sb.Append($"\nRefund {level} levels of {attr} for {total:N0}");
        }

        foreach (var attr in Enum.GetValues<PropertyAttribute2nd>().OrderBy(x => x.ToString()))
        {
            var total = player.GetCost(attr);
            if (total <= 0)
                continue;

            var level = player.GetLevel(attr);
            player.SetLevel(attr, 0);
            player.SetCost(attr, 0);
            player.SendUpdated(attr);
            refund += total;

            sb.Append($"\nRefund {level} levels of {attr} for {total:N0}");
        }

        foreach (var attr in Enum.GetValues<Skill>().OrderBy(x => x.ToString()))
        {
            var total = player.GetCost(attr);
            if (total <= 0)
                continue;

            var level = player.GetLevel(attr);
            player.SetLevel(attr, 0);
            player.SetCost(attr, 0);
            player.SendUpdated(attr);
            refund += total;

            sb.Append($"\nRefund {level} levels of {attr} for {total:N0}");
        }

        if (refund == 0)
        {
            player.SendMessage($"Nothing to refund.");
            return;
        }

        player.AvailableExperience += refund;
        player.SendMessage(sb.ToString());
    }



    [CommandHandler("t1", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0)]
    public static void HandleT1(Session session, params string[] parameters)
    {
        var player = session.Player;

        if (!player.Attributes.TryGetValue(PropertyAttribute.Strength, out var str))
            return;

        str.Ranks += 3;
        player.Session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, str));
    }
}
