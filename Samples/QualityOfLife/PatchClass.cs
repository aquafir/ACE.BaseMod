using MotionTable = ACE.DatLoader.FileTypes.MotionTable;

namespace QualityOfLife;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;

        ModC.RegisterPatchCategories(Settings.Patches);

        if (Settings.Patches.Contains(Features.Augmentations))
            Augmentations.OverrideCaps();

        if (Settings.Patches.Contains(Features.Fellowships))
            Fellowships.SetFellowshipSettings();
    }

    public override void Stop()
    {
        base.Stop();

        if (ModC.State == ModState.Running)
        {
            if (Settings.Patches.Contains(Features.Fellowships))
                Fellowships.RestoreFellowSettings();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HandleActionTeleToMarketPlace))]
    public static bool PreHandleActionTeleToMarketPlace(ref Player __instance)
    {
        if (__instance.IsOlthoiPlayer)
        {
            __instance.Session.Network.EnqueueSend(new GameEventWeenieError(__instance.Session, WeenieError.OlthoiCanOnlyRecallToLifestone));
            return false;
        }

        if (__instance.PKTimerActive)
        {
            __instance.Session.Network.EnqueueSend(new GameEventWeenieError(__instance.Session, WeenieError.YouHaveBeenInPKBattleTooRecently));
            return false;
        }

        if (__instance.RecallsDisabled)
        {
            __instance.Session.Network.EnqueueSend(new GameEventWeenieError(__instance.Session, WeenieError.ExitTrainingAcademyToUseCommand));
            return false;
        }

        if (__instance.TooBusyToRecall)
        {
            __instance.Session.Network.EnqueueSend(new GameEventWeenieError(__instance.Session, WeenieError.YoureTooBusy));
            return false;
        }

        if (__instance.CombatMode != CombatMode.NonCombat)
        {
            // this should be handled by a different thing, probably a function that forces player into peacemode
            var updateCombatMode = new GameMessagePrivateUpdatePropertyInt(__instance, PropertyInt.CombatMode, (int)CombatMode.NonCombat);
            __instance.SetCombatMode(CombatMode.NonCombat);
            __instance.Session.Network.EnqueueSend(updateCombatMode);
        }

        __instance.EnqueueBroadcast(new GameMessageSystemChat($"{__instance.Name} is recalling to the marketplace.", ChatMessageType.Recall), WorldObject.LocalBroadcastRange, ChatMessageType.Recall);

        __instance.SendMotionAsCommands(MotionCommand.MarketplaceRecall, MotionStance.NonCombat);

        var startPos = new Position(__instance.Location);

        // TODO: (OptimShi): Actual animation length is longer than in retail. 18.4s
        ActionChain mpChain = new ActionChain();
        var animLength = DatManager.PortalDat.ReadFromDat<MotionTable>(__instance.MotionTableId).GetAnimationLength(MotionCommand.MarketplaceRecall);
        mpChain.AddDelaySeconds(animLength);
        //mpChain.AddDelaySeconds(14);

        //resolve player
        var player = PlayerManager.FindByGuid(__instance.Guid) as Player;
        if (player is null)
            return false;

        // Then do teleport
        __instance.IsBusy = true;
        mpChain.AddAction(__instance, () =>
        {
            player.IsBusy = false;
            var endPos = new Position(player.Location);
            if (startPos.SquaredDistanceTo(endPos) > Player.RecallMoveThresholdSq)
            {
                player.Session.Network.EnqueueSend(new GameEventWeenieError(player.Session, WeenieError.YouHaveMovedTooFar));
                return;
            }

#if REALM
            player.Teleport(Player.MarketplaceDrop.AsInstancedPosition(player.Location.Instance));
#else
            player.Teleport(Player.MarketplaceDrop);
#endif
        });

        // Set the chain to run
        mpChain.EnqueueChain();

        return false;
    }

    //Fakes having more credits invested
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SkillAlterationDevice), "GetTotalSpecializedCredits", new Type[] { typeof(Player) })]
    public static void PostGetTotalSpecializedCredits(Player player, ref SkillAlterationDevice __instance, ref int __result)
        => __result = Math.Max(0, __result + (70 - Settings.MaxSpecCredits));

    [CommandHandler("setlum", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0, "Sets luminance to the max if unlocked")]
    public static void HandleSetProperty(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player.MaximumLuminance is not null && S.Settings.Defaults.Int64Defaults.TryGetValue(PropertyInt64.MaximumLuminance, out var value))
        {
            player.MaximumLuminance = value;
            player.SendMessage($"Your luminance is now {value}");

            player.UpdateProperty(player, PropertyInt64.MaximumLuminance, value);
        }
        else
            player.SendMessage($"Your luminance was already the max or a max is not set.");
    }
}

