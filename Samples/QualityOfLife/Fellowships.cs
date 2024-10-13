namespace QualityOfLife;

[HarmonyPatchCategory(nameof(Features.Fellowships))]
public class Fellowships
{
    //private static long evenShare;
    public static void SetFellowshipSettings()
    {
        Fellowship.MaxFellows = S.Settings.Fellowship.MaxMembers;
        ModManager.Log($"Set max fellow members to {Fellowship.MaxFellows}");

        //Use the property for share level
        //Could overwrite CalculateXPSharing()
        //evenShare = PropertyManager.GetLong("fellowship_even_share_level").Item;
        //PropertyManager.ModifyLong("fellowship_even_share_level", S.Settings.Fellowship.EvenShareLevel);
    }

    public static void RestoreFellowSettings()
    {
        Fellowship.MaxFellows = 9;
        ModManager.Log($"Restored max fellow members to {Fellowship.MaxFellows}");

        //PropertyManager.ModifyLong("fellowship_even_share_level", evenShare);
        //Traverse.Create<Fellowship>().Field(nameof(Fellowship.MaxDistance)).SetValue(600);        
    }

    //Use a patch to rework the method
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Fellowship), nameof(Fellowship.AddFellowshipMember), new Type[] { typeof(Player), typeof(Player) })]
    public static bool PreAddFellowshipMember(Player inviter, Player newMember, ref Fellowship __instance)
    {
        if (inviter == null || newMember == null)
            return false;

        if (S.Settings.Fellowship.SendDetails)
        {
            var details = $"\n{inviter.Name} wants to add {newMember.Name} to {__instance.FellowshipName}.\nShareXP: {GetFellowshipShare(__instance.GetFellowshipMembers().Count + 1)}";
            inviter.SendMessage(details);
            newMember.SendMessage(details);
        }
        //return true;

        //Method rewrite
        if (__instance.IsLocked)
        {

            if (!__instance.DepartedMembers.TryGetValue(newMember.Guid.Full, out var timeDeparted))
            {
                inviter.Session.Network.EnqueueSend(new GameEventWeenieErrorWithString(inviter.Session, WeenieErrorWithString.LockedFellowshipCannotRecruit_, newMember.Name));
                //newMember.SendWeenieError(WeenieError.LockedFellowshipCannotRecruitYou);
                return false;
            }
            else
            {
                var timeLimit = Time.GetDateTimeFromTimestamp(timeDeparted).AddSeconds(600);
                if (DateTime.UtcNow > timeLimit)
                {
                    inviter.Session.Network.EnqueueSend(new GameEventWeenieErrorWithString(inviter.Session, WeenieErrorWithString.LockedFellowshipCannotRecruit_, newMember.Name));
                    //newMember.SendWeenieError(WeenieError.LockedFellowshipCannotRecruitYou);
                    return false;
                }
            }
        }

        if (__instance.FellowshipMembers.Count == S.Settings.Fellowship.MaxMembers)
        //if (__instance.FellowshipMembers.Count == Fellowship.MaxFellows)
        {
            inviter.Session.Network.EnqueueSend(new GameEventWeenieError(inviter.Session, WeenieError.YourFellowshipIsFull));
            return false;
        }

        if (newMember.Fellowship != null || __instance.FellowshipMembers.ContainsKey(newMember.Guid.Full))
        {
            inviter.Session.Network.EnqueueSend(new GameMessageSystemChat($"{newMember.Name} is already a member of a Fellowship.", ChatMessageType.Broadcast));
        }
        else
        {
            if (PropertyManager.GetBool("fellow_busy_no_recruit").Item && newMember.IsBusy)
            {
                inviter.Session.Network.EnqueueSend(new GameMessageSystemChat($"{newMember.Name} is busy.", ChatMessageType.Broadcast));
                return false;
            }

            if (newMember.GetCharacterOption(CharacterOption.AutomaticallyAcceptFellowshipRequests))
            {
                __instance.AddConfirmedMember(inviter, newMember, true);
            }
            else
            {
                if (!newMember.ConfirmationManager.EnqueueSend(new Confirmation_Fellowship(inviter.Guid, newMember.Guid), inviter.Name))
                {
                    inviter.Session.Network.EnqueueSend(new GameMessageSystemChat($"{newMember.Name} is busy.", ChatMessageType.Broadcast));
                }
            }
        }
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Fellowship), nameof(Fellowship.AddConfirmedMember), new Type[] { typeof(Player), typeof(Player), typeof(bool) })]
    public static bool PreAddConfirmedMember(Player inviter, Player player, bool response, ref Fellowship __instance)
    {
        if (inviter == null || inviter.Session == null || inviter.Session.Player == null || player == null) return false;

        if (!response)
        {
            // player clicked 'no' on the fellowship popup
            inviter.Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} declines your invite", ChatMessageType.Fellowship));
            inviter.Session.Network.EnqueueSend(new GameEventWeenieError(inviter.Session, WeenieError.FellowshipDeclined));
            return false;
        }

        if (__instance.FellowshipMembers.Count == Fellowship.MaxFellows)
        {
            inviter.Session.Network.EnqueueSend(new GameEventWeenieError(inviter.Session, WeenieError.YourFellowshipIsFull));
            return false;
        }

        __instance.FellowshipMembers.TryAdd(player.Guid.Full, new WeakReference<Player>(player));
        player.Fellowship = inviter.Fellowship;

        __instance.CalculateXPSharing();

        var fellowshipMembers = __instance.GetFellowshipMembers();

        foreach (var member in fellowshipMembers.Values.Where(i => i.Guid != player.Guid))
            member.Session.Network.EnqueueSend(new GameEventFellowshipUpdateFellow(member.Session, player, __instance.ShareXP));

        if (__instance.ShareLoot)
        {
            foreach (var member in fellowshipMembers.Values.Where(i => i.Guid != player.Guid))
            {
                member.Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has given you permission to loot his or her kills.", ChatMessageType.Broadcast));
                member.Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} may now loot your kills.", ChatMessageType.Broadcast));

                player.Session.Network.EnqueueSend(new GameMessageSystemChat($"{member.Name} has given you permission to loot his or her kills.", ChatMessageType.Broadcast));
                player.Session.Network.EnqueueSend(new GameMessageSystemChat($"{member.Name} may now loot your kills.", ChatMessageType.Broadcast));
            }
        }

        __instance.UpdateAllMembers();

        if (inviter.CurrentMotionState.Stance == MotionStance.NonCombat) // only do this motion if inviter is at peace, other times motion is skipped. 
            inviter.SendMotionAsCommands(MotionCommand.BowDeep, MotionStance.NonCombat);

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
        //__result = S.Settings.Fellowship.SharePercent.TryGetValue(__instance.GetFellowshipMembers().Count, out var share) ? share : S.Settings.Fellowship.DefaultShare;
        __result = GetFellowshipShare(__instance.GetFellowshipMembers().Count);

        //Return false to override
        return false;
    }

    static double GetFellowshipShare(int members) =>
        S.Settings.Fellowship.SharePercent.TryGetValue(members, out var share) ? share : S.Settings.Fellowship.DefaultShare;


    [CommandHandler("fship", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleIronman(Session session, params string[] parameters)
    {
        try
        {
            var player = session.Player;
            var lb = player.CurrentLandblock;

            if (player.Fellowship is null)
            {
                player.SendMessage($"Create a fellowship first.");
                return;
            }

            var list = parameters.Length > 0 ?
                PlayerManager.GetAllOnline().Where(x => x.Fellowship is null && x.Guid != player.Guid && x.Name.Contains(parameters[0], StringComparison.OrdinalIgnoreCase)) :
                lb.GetPlayers().Where(x => x.Fellowship is null && x.Guid != player.Guid);

            if (list.Count() == 0)
            {
                player.SendMessage($"No players found.");
                return;
            }

            foreach (var p in list)
            {
                player.Fellowship.AddFellowshipMember(player, p);
                player.SendMessage($"Inviting {p.Name}");

                //Todo: stop at max?
            }
        }
        catch (Exception ex) { ModManager.Log(ex.Message, ModManager.LogLevel.Error); }
    }
}

public class FellowshipSettings
{
    public bool SendDetails { get; set; } = true;
    public int MaxMembers { get; set; } = 2;

    public Dictionary<int, double> SharePercent { get; set; } = new()
    {
        [1] = 1.0,
        [2] = .75,
        [3] = .6,
        [4] = .55,
        [5] = .5,
        [6] = .45,
        [7] = .4,
        [8] = .35,
        [9] = .3,
    };
    public double DefaultShare { get; set; } = 0;

    //public long EvenShareLevel { get; set; } = 50;  //fellowship_even_share_level
}