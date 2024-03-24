using ACE.Server.Factories;

namespace QualityOfLife;

[HarmonyPatchCategory(nameof(SettingsDefaults))]
internal static class SettingsDefaults
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerFactory), nameof(PlayerFactory.CharacterCreateSetDefaultCharacterOptions), new Type[] { typeof(Player) })]
    public static void PostCharacterCreateSetDefaultCharacterOptions(Player player)
    {
        foreach (var option in PatchClass.Settings.CharacterSettingsDefaults.Defaults)
            player.SetCharacterOption(option.Key, option.Value);
    }
}

public class CharacterSettings
{
    public Dictionary<CharacterOption, bool> Defaults = new()
    {
        [CharacterOption.AutoRepeatAttacks] = true,
        [CharacterOption.IgnoreAllegianceRequests] = true,
        [CharacterOption.IgnoreFellowshipRequests] = true,
        [CharacterOption.IgnoreAllTradeRequests] = true,
        [CharacterOption.DisableMostWeatherEffects] = true,
        [CharacterOption.AlwaysDaylightOutdoors] = true,
        [CharacterOption.LetOtherPlayersGiveYouItems] = true,
        [CharacterOption.KeepCombatTargetsInView] = true,
        [CharacterOption.Display3dTooltips] = true,
        [CharacterOption.AttemptToDeceiveOtherPlayers] = true,
        [CharacterOption.RunAsDefaultMovement] = true,
        [CharacterOption.StayInChatModeAfterSendingMessage] = true,
        [CharacterOption.AdvancedCombatInterface] = true,
        [CharacterOption.AutoTarget] = true,
        [CharacterOption.VividTargetingIndicator] = true,
        [CharacterOption.ShareFellowshipExpAndLuminance] = true,
        [CharacterOption.AcceptCorpseLootingPermissions] = true,
        [CharacterOption.ShareFellowshipLoot] = true,
        [CharacterOption.AutomaticallyAcceptFellowshipRequests] = true,
        [CharacterOption.SideBySideVitals] = true,
        [CharacterOption.ShowCoordinatesByTheRadar] = true,
        [CharacterOption.DisplaySpellDurations] = true,
        [CharacterOption.DisableHouseRestrictionEffects] = true,
        [CharacterOption.DragItemToPlayerOpensTrade] = true,
        [CharacterOption.ShowAllegianceLogons] = true,
        [CharacterOption.UseChargeAttack] = true,
        [CharacterOption.UseCraftingChanceOfSuccessDialog] = true,
        [CharacterOption.ListenToAllegianceChat] = true,
        [CharacterOption.AllowOthersToSeeYourDateOfBirth] = true,
        [CharacterOption.AllowOthersToSeeYourAge] = true,
        [CharacterOption.AllowOthersToSeeYourChessRank] = true,
        [CharacterOption.AllowOthersToSeeYourFishingSkill] = true,
        [CharacterOption.AllowOthersToSeeYourNumberOfDeaths] = true,
        [CharacterOption.DisplayTimestamps] = true,
        [CharacterOption.SalvageMultipleMaterialsAtOnce] = true,
        [CharacterOption.ListenToGeneralChat] = true,
        [CharacterOption.ListenToTradeChat] = true,
        [CharacterOption.ListenToLFGChat] = true,
        [CharacterOption.ListenToRoleplayChat] = true,
        [CharacterOption.AppearOffline] = true,
        [CharacterOption.AllowOthersToSeeYourNumberOfTitles] = true,
        [CharacterOption.UseMainPackAsDefaultForPickingUpItems] = true,
        [CharacterOption.LeadMissileTargets] = true,
        [CharacterOption.UseFastMissiles] = true,
        [CharacterOption.FilterLanguage] = true,
        [CharacterOption.ConfirmUseOfRareGems] = true,
        [CharacterOption.ListenToSocietyChat] = true,
        [CharacterOption.ShowYourHelmOrHeadGear] = true,
        [CharacterOption.DisableDistanceFog] = true,
        [CharacterOption.UseMouseTurning] = true,
        [CharacterOption.ShowYourCloak] = true,
        [CharacterOption.LockUI] = true,
        [CharacterOption.ListenToPKDeathMessages] = true,
        [CharacterOption.CharacterOptions1Default] = true,
        [CharacterOption.CharacterOptions2Default] = true,
    };
}