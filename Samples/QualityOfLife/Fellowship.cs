using ACE.Server.Managers;

namespace QualityOfLife;

[HarmonyPatchCategory(nameof(Fellowships))]
public class Fellowships
{
    private static long evenShare;
    private static void SetFellowshipSettings()
    {
        Fellowship.MaxFellows = PatchClass.Settings.MaxMembers;

        //Use the property for share level
        //Could overwrite CalculateXPSharing()
        evenShare = PropertyManager.GetLong("fellowship_even_share_level").Item;
        PropertyManager.ModifyLong("fellowship_even_share_level", PatchClass.Settings.EvenShareLevel);

        //Use a utility class of Harmony to set the value of a readonly?
        //Traverse.Create<Fellowship>().Field(nameof(Fellowship.MaxDistance)).SetValue(PatchClass.Settings.MaxDistance);
    }

    private static void RestoreFellowSettings()
    {
        Fellowship.MaxFellows = 9;
        PropertyManager.ModifyLong("fellowship_even_share_level", evenShare);
        //Traverse.Create<Fellowship>().Field(nameof(Fellowship.MaxDistance)).SetValue(600);        
    }

    //Use a patch to rework the method
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Fellowship), nameof(Fellowship.AddFellowshipMember), new Type[] { typeof(Player), typeof(Player) })]
    public static bool PreAddFellowshipMember(Player inviter, Player newMember, ref Fellowship __instance)
    {
        if (inviter == null || newMember == null)
            return false;

        if (PatchClass.Settings.SendDetails)
        {
            var details = $"\n{inviter.Name} wants to add {newMember.Name} to {__instance.FellowshipName}.\nShareXP: {GetFellowshipShare(__instance.GetFellowshipMembers().Count + 1)}";
            inviter.SendMessage(details);
            newMember.SendMessage(details);
        }
        return true;

        //Method rewrite
        //if (__instance.IsLocked)
        //{

        //    if (!__instance.DepartedMembers.TryGetValue(newMember.Guid.Full, out var timeDeparted))
        //    {
        //        inviter.Session.Network.EnqueueSend(new GameEventWeenieErrorWithString(inviter.Session, WeenieErrorWithString.LockedFellowshipCannotRecruit_, newMember.Name));
        //        //newMember.SendWeenieError(WeenieError.LockedFellowshipCannotRecruitYou);
        //        return false;
        //    }
        //    else
        //    {
        //        var timeLimit = Time.GetDateTimeFromTimestamp(timeDeparted).AddSeconds(600);
        //        if (DateTime.UtcNow > timeLimit)
        //        {
        //            inviter.Session.Network.EnqueueSend(new GameEventWeenieErrorWithString(inviter.Session, WeenieErrorWithString.LockedFellowshipCannotRecruit_, newMember.Name));
        //            //newMember.SendWeenieError(WeenieError.LockedFellowshipCannotRecruitYou);
        //            return false;
        //        }
        //    }
        //}

        //if (__instance.FellowshipMembers.Count == PatchClass.Settings.MaxMembers)
        ////if (__instance.FellowshipMembers.Count == Fellowship.MaxFellows)
        //{
        //    inviter.Session.Network.EnqueueSend(new GameEventWeenieError(inviter.Session, WeenieError.YourFellowshipIsFull));
        //    return false;
        //}

        //if (newMember.Fellowship != null || __instance.FellowshipMembers.ContainsKey(newMember.Guid.Full))
        //{
        //    inviter.Session.Network.EnqueueSend(new GameMessageSystemChat($"{newMember.Name} is already a member of a Fellowship.", ChatMessageType.Broadcast));
        //}
        //else
        //{
        //    if (PropertyManager.GetBool("fellow_busy_no_recruit").Item && newMember.IsBusy)
        //    {
        //        inviter.Session.Network.EnqueueSend(new GameMessageSystemChat($"{newMember.Name} is busy.", ChatMessageType.Broadcast));
        //        return false;
        //    }

        //    if (newMember.GetCharacterOption(CharacterOption.AutomaticallyAcceptFellowshipRequests))
        //    {
        //        __instance.AddConfirmedMember(inviter, newMember, true);
        //    }
        //    else
        //    {
        //        if (!newMember.ConfirmationManager.EnqueueSend(new Confirmation_Fellowship(inviter.Guid, newMember.Guid), inviter.Name))
        //        {
        //            inviter.Session.Network.EnqueueSend(new GameMessageSystemChat($"{newMember.Name} is busy.", ChatMessageType.Broadcast));
        //        }
        //    }
        //}
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Fellowship), nameof(Fellowship.GetDistanceScalar), new Type[] { typeof(Player), typeof(Player), typeof(XpType) })]
    public static bool PreGetDistanceScalar(Player earner, Player fellow, XpType xpType, ref Fellowship __instance, ref double __result)
    {
        //Return false to override
        //return false;

        //Return true to execute original
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Fellowship), nameof(Fellowship.GetMemberSharePercent))]
    public static bool PreGetMemberSharePercent(ref Fellowship __instance, ref double __result)
    {
        //If you don't have enough shares specified use the default
        //__result = PatchClass.Settings.SharePercent.TryGetValue(__instance.GetFellowshipMembers().Count, out var share) ? share : PatchClass.Settings.DefaultShare;
        __result = GetFellowshipShare(__instance.GetFellowshipMembers().Count);

        //Return false to override
        return false;
    }

    static double GetFellowshipShare(int members) =>
        PatchClass.Settings.SharePercent.TryGetValue(members, out var share) ? share : PatchClass.Settings.DefaultShare;
}