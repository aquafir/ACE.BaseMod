using ACE.DatLoader.FileTypes;

namespace QualityOfLife;

[HarmonyPatchCategory(Settings.AnimationOverrideCategory)]
internal static class AnimationOverrides
{

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), nameof(Player.SendMotionAsCommands), new Type[] { typeof(MotionCommand), typeof(MotionStance) })]
    //public static bool PreSendMotionAsCommands(MotionCommand motionCommand, MotionStance motionStance, ref Player __instance)
    //{

    //    //Intercept animations.  Doesn't factor in stance?
    //    if (PatchClass.Settings.AnimationSpeeds.TryGetValue(motionCommand, out __result))
    //        return false;

    //    return true;

    //    //Return false to override
    //    //return false;

    //    //Return true to execute original
    //    return true;
    //}


    [HarmonyPrefix]
    [HarmonyPatch(typeof(MotionTable), nameof(MotionTable.GetAnimationLength), new Type[] { typeof(MotionCommand) })]
    public static bool PreGetAnimationLength(MotionCommand motion, ref MotionTable __instance, ref float __result)
    {
        //Intercept animations.  Doesn't factor in stance?
        if (PatchClass.Settings.AnimationSpeeds.TryGetValue(motion, out __result))
            return false;

        return true;
    }
}