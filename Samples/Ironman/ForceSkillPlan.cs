using static ACE.Server.Mods.ModManager;

namespace Ironman;

[HarmonyPatchCategory(nameof(ForceSkillPlan))]
public class ForceSkillPlan
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionTrainSkill), new Type[] { typeof(Skill), typeof(int) })]
    public static bool PreHandleActionTrainSkill(Skill skill, int creditsSpent, ref Player __instance, ref bool __result)
    {
        if (__instance is null || __instance.GetProperty(FakeBool.Ironman) != true)
            return true;

        __instance.SendMessage($"Ironmode players can't train skills.");
        __instance.Session.Network.EnqueueSend(new GameMessageSystemChat($"Failed to train {skill.ToSentence()}!", ChatMessageType.Advancement));
        __result = false;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.SetMaxVitals))]
    public static void PreSetMaxVitals(ref Player __instance)
    {
        //Least intrusive place to check for levelup?
        if (__instance is null || __instance.GetProperty(FakeBool.Ironman) != true)
            return;

        //Proceed with plan
        var plan = (__instance.GetProperty(FakeString.IronmanPlan) ?? "").Split(';');
        var i = 0;
        for (; i < plan.Length; i++)
        {
            if (!Enum.TryParse<Skill>(plan[i], out var skill))
            {
                Log($"Unable to parse Skill from plan @ {plan[i]}", LogLevel.Error);
                return;
            }

            //Failed to train/spec
            if (!__instance.TryAdvanceSkill(skill))
                break;
        }

        //Nothing changed
        if (i == 0)
            return;

        var msg = $"{__instance.Name} advanced their skill plan {i} steps with {string.Join("->", plan.Take(i))}";
        Log(msg);
        __instance.SendMessage(msg);

        //Store the update plan
        __instance.SetProperty(FakeString.IronmanPlan, string.Join(';', plan.Skip(i)));
        __instance.SendUpdatedSkills();
    }
}