using ACE.DatLoader.FileTypes;
using ACE.Server.Entity.Actions;

namespace QualityOfLife;

[HarmonyPatchCategory(nameof(Features.Animations))]
internal static class Animations
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MotionTable), nameof(MotionTable.GetAnimationLength), new Type[] { typeof(MotionCommand) })]
    public static bool PreGetAnimationLength(MotionCommand motion, ref MotionTable __instance, ref float __result)
    {
        //Intercept animations.  Doesn't factor in stance?
        if (S.Settings.Animations.AnimationSpeeds.TryGetValue(motion, out __result))
            return false;

        return true;
    }

    //Rewrite suicide
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), "HandleSuicide", new Type[] { typeof(int), typeof(int) })]
    public static bool PreHandleSuicide(int numDeaths, int step, ref Player __instance)
    {
        // S.Settings.Animations.SuicideSeconds

        if (!__instance.suicideInProgress || numDeaths != __instance.NumDeaths)
            return false;

        if (step < Player.SuicideMessages.Count)
        {
            //Laaaaame approach but needed to use the same anonymous method
            if (PlayerManager.GetOnlinePlayer(__instance.Guid) is not Player p)
                return true;

#if REALM
        p.EnqueueBroadcast(new GameMessageHearSpeech(Player.SuicideMessages[step], p.GetNameWithSuffix(), p.Guid.ClientGUID, ChatMessageType.Speech), WorldObject.LocalBroadcastRange);
#else
        p.EnqueueBroadcast(new GameMessageHearSpeech(Player.SuicideMessages[step], p.GetNameWithSuffix(), p.Guid.Full, ChatMessageType.Speech), WorldObject.LocalBroadcastRange);
#endif

            var suicideChain = new ActionChain();
            suicideChain.AddDelaySeconds(S.Settings.Animations.DieSeconds);
            suicideChain.AddAction(p, () => p.HandleSuicide(numDeaths, step + 1));
            suicideChain.EnqueueChain();
        }
        else
            __instance.Die(new DamageHistoryInfo(__instance), __instance.DamageHistory.TopDamager);

        //Return true to execute original
        return false;
    }
}

public class AnimationSettings
{
    public float DieSeconds { get; set; } = 0.0f;
    public Dictionary<MotionCommand, float> AnimationSpeeds { get; set; } = new()
    {
        [MotionCommand.AllegianceHometownRecall] = 0f,
        [MotionCommand.HouseRecall] = 0f,
        [MotionCommand.LifestoneRecall] = 0f,
        [MotionCommand.MarketplaceRecall] = 0f,
        [MotionCommand.PKArenaRecall] = 0f,
    };
}