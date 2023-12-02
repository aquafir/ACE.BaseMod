using ACE.DatLoader.FileTypes;
using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity.Actions;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Physics.Animation;
using ACE.Server.WorldObjects;
using Iced.Intel.EncoderInternal;
using MotionTable = ACE.DatLoader.FileTypes.MotionTable;

namespace QualityOfLife;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
    const int RETRIES = 10;

    public static Settings Settings = new();
    private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
    private static FileInfo settingsInfo = new(settingsPath);

    private static JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

        if (!settingsInfo.RetryWrite(jsonString, RETRIES))
        {
            ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
        }
    }

    private static void LoadSettings()
    {
        if (!settingsInfo.Exists)
        {
            ModManager.Log($"Creating {settingsInfo}...");
            SaveSettings();
        }
        else
            ModManager.Log($"Loading settings from {settingsPath}...");

        if (!settingsInfo.RetryRead(out string jsonString, RETRIES))
        {
            Mod.State = ModState.Error;
            return;
        }

        try
        {
            Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
        }
        catch (Exception)
        {
            ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
            return;
        }
    }
    #endregion

    #region Start/Shutdown
    public static void Start()
    {
        //Need to decide on async use
        Mod.State = ModState.Loading;
        LoadSettings();

        if (Mod.State == ModState.Error)
        {
            ModManager.DisableModByPath(Mod.ModPath);
            return;
        }

        PatchCategories();

        Mod.State = ModState.Running;
    }

    public static void Shutdown()
    {
        //if (Mod.State == ModState.Running)
        // Shut down enabled mod...

        //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
        //SaveSettings();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    private static void PatchCategories()
    {
        if (Settings.OverrideDefaultProperties)
            Mod.Harmony.PatchCategory(Settings.DefaultOverrideCategory);

        if (Settings.OverrideAnimations)
            Mod.Harmony.PatchCategory(Settings.AnimationOverrideCategory);
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

        __instance.EnqueueBroadcast(new GameMessageSystemChat($"{__instance.Name} is recalling to the marketplace.", ChatMessageType.Recall), Player.LocalBroadcastRange, ChatMessageType.Recall);

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

            player.Teleport(Player.MarketplaceDrop);
        });

        // Set the chain to run
        mpChain.EnqueueChain();

        return false;
    }

    //Fakes having more credits invested
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SkillAlterationDevice), "GetTotalSpecializedCredits", new Type[] { typeof(Player) })]
    public static void PostGetTotalSpecializedCredits(Player player, ref SkillAlterationDevice __instance, ref int __result)
        => __result = Math.Max(0, __result + (70 - PatchClass.Settings.MaxSpecCredits));

    //Rewrite suicide
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), "HandleSuicide", new Type[] { typeof(int), typeof(int) })]
    public static bool PreHandleSuicide(int numDeaths, int step, ref Player __instance)
    {
        // PatchClass.Settings.SuicideSeconds

        if (!__instance.suicideInProgress || numDeaths != __instance.NumDeaths)
            return false;

        if (step < Player.SuicideMessages.Count)
        {
            //Laaaaame approach but needed to use the same anonymous method
            if (PlayerManager.GetOnlinePlayer(__instance.Guid) is not Player p)
                return true;

            p.EnqueueBroadcast(new GameMessageHearSpeech(Player.SuicideMessages[step], p.GetNameWithSuffix(), p.Guid.Full, ChatMessageType.Speech), Player.LocalBroadcastRange);

            var suicideChain = new ActionChain();
            suicideChain.AddDelaySeconds(PatchClass.Settings.DieSeconds);
            suicideChain.AddAction(p, () => p.HandleSuicide(numDeaths, step + 1));
            suicideChain.EnqueueChain();
        }
        else
            __instance.Die(new DamageHistoryInfo(__instance), __instance.DamageHistory.TopDamager);

        //Return true to execute original
        return false;
    }

    [CommandHandler("setlum", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 0, "Sets luminance to the max if unlocked")]
    public static void HandleSetProperty(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player.MaximumLuminance is not null && Settings.Int64Defaults.TryGetValue(PropertyInt64.MaximumLuminance, out var value))
        {
            player.MaximumLuminance = value;
            player.SendMessage($"Your luminance is now {value}");

            player.UpdateProperty(player, PropertyInt64.MaximumLuminance, value);
        }
        else
            player.SendMessage($"Your luminance was already the max or a max is not set.");
    }
}
