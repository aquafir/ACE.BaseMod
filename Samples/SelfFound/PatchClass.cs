using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Server.Factories.Enum;
using ACE.Server.Factories;
using ACE.Entity.Models;
using ACE.Server.WorldObjects.Managers;
using ACE.Server.Managers;
using ACE.Database.Models.World;
using ACE.Server.Factories.Entity;
using ACE.Server.WorldObjects;
using ACE.Server.Network.Structure;
using ACE.Database;
using ACE.Server.Network;

namespace SelfFound;

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

    [CommandHandler("d", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Toggles SSF mode.")]
    public static void HandleD(Session session, params string[] parameters)
    {
        var player = session.Player;
        player.Die();
    }


    [CommandHandler("ssf", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, "Toggles SSF mode.")]
    public static void HandleDebugGPS(Session session, params string[] parameters)
    {
        var player = session.Player;

        player.ToggleSelfFound();

        player.SendMessage($"You are {(player.IsSelfFound() ? "now" : "no longer")} self found.");
    }

    #region Generators / SSF Flagging
    #region Chests
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Chest), nameof(Chest.Unlock), new Type[] { typeof(uint), typeof(uint), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref })]
    public static void PostUnlock(uint unlockerGuid, uint playerLockpickSkillLvl, int difficulty, ref Chest __instance, ref UnlockResults __result)
=> HandleClaimChest(unlockerGuid, __instance, __result);
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Chest), nameof(Chest.Unlock), new Type[] { typeof(uint), typeof(Key), typeof(string) })]
    public static void PostUnlock(uint unlockerGuid, Key key, string keyCode, ref Chest __instance, ref UnlockResults __result)
        => HandleClaimChest(unlockerGuid, __instance, __result);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Chest), nameof(Chest.Reset), new Type[] { typeof(double?) })]
    public static void PostReset(double? resetTimestamp, ref Chest __instance) => __instance.RemoveSelfFound();

    //Claim a container by unlocking it
    public static void HandleClaimChest(uint unlockerGuid, Chest container, UnlockResults result)
    {
        //Only care about successful unlocks
        if (result != UnlockResults.UnlockSuccess) return;

        //Already claimed
        if (container.IsSelfFound()) return;

        //Get a player?
        if (PlayerManager.GetOnlinePlayer(unlockerGuid) is not Player player)
            return;

        if (!player.IsSelfFound())
            return;


        player.ClaimContainer(container);
    }

    #endregion

    //Add SSF for killer
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), "GenerateTreasure", new Type[] { typeof(DamageHistoryInfo), typeof(Corpse) })]
    public static void PostGenerateTreasure(DamageHistoryInfo killer, Corpse corpse, ref Creature __instance, ref List<WorldObject> __result)
    {
        if (!killer.IsPlayer)
            return;

        if (PlayerManager.FindByGuid(killer.Guid) is not Player player)
            return;

        if (!player.IsSelfFound())
            return;

        player.ClaimContainer(corpse);

        //var sb = new StringBuilder($"\n{player.Name} is claiming {corpse.Name}");

        ////Debugger.Break();
        //foreach (var item in corpse.Inventory.Values)  //Missing in __result?
        //{
        //    sb.Append($"\n  {item.Name}");
        //    item.MakeSelfFound(player);
        //}

        //player.SendMessage(sb.ToString());
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EmoteManager), nameof(EmoteManager.ExecuteEmote), new Type[] { typeof(PropertiesEmote), typeof(PropertiesEmoteAction), typeof(WorldObject) })]
    public static bool PreExecuteEmote(PropertiesEmote emoteSet, PropertiesEmoteAction emote, WorldObject targetObject, ref EmoteManager __instance, ref float __result)
    {
        if ((EmoteType)emote.Type != EmoteType.CreateTreasure)
            return true;


        var player = targetObject as Player;
        var creature = __instance.WorldObject as Creature;
        var targetCreature = targetObject as Creature;

        var delay = 0.0f;
        var emoteType = (EmoteType)emote.Type;

        //if (Debug)
        //Console.WriteLine($"{WorldObject.Name}.ExecuteEmote({emoteType})");

        var text = emote.Message;

        if (player != null)
        {
            var treasureTier = emote.WealthRating ?? 1;

            var treasureType = (TreasureItemCategory?)emote.TreasureType ?? TreasureItemCategory.Undef;

            var treasureClass = (TreasureItemType_Orig?)emote.TreasureClass ?? TreasureItemType_Orig.Undef;

            // Create a dummy treasure profile for passing emote values
            var profile = new ACE.Database.Models.World.TreasureDeath
            {
                Tier = treasureTier,
                //TreasureType = (uint)treasureType,
                LootQualityMod = 0,
                ItemChance = 100,
                ItemMinAmount = 1,
                ItemMaxAmount = 1,
                //ItemTreasureTypeSelectionChances = (int)treasureClass,
                MagicItemChance = 100,
                MagicItemMinAmount = 1,
                MagicItemMaxAmount = 1,
                //MagicItemTreasureTypeSelectionChances = (int)treasureClass,
                MundaneItemChance = 100,
                MundaneItemMinAmount = 1,
                MundaneItemMaxAmount = 1,
                //MundaneItemTypeSelectionChances = (int)treasureClass,
                UnknownChances = 21
            };

            var treasure = LootGenerationFactory.CreateRandomLootObjects_New(profile, treasureType, treasureClass);
            if (treasure != null)
            {
                player.TryCreateForGive(__instance.WorldObject, treasure);
            }
        }


        //Skip
        return false;
    }

    #endregion

    #region Restrictions
    //Only allow OriginalFinder to wield
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), "CheckWieldRequirements", new Type[] { typeof(WorldObject) })]
    public static bool PreCheckWieldRequirements(WorldObject item, ref Player __instance, ref WeenieError __result)
    {
        if (!__instance.IsSelfFound()) return true;

        //If an item isn't SSF it can't be equipped
        if (!item.IsSelfFound())
        {
            //Related check
            //var allowedWielder = item.GetProperty(PropertyInstanceId.AllowedWielder);
            //if (allowedWielder != null && (allowedWielder != Guid.Full))
            //    return WeenieError.YouDoNotOwnThatItem; // Unsure of the exact message

            //What message to use?
            __result = WeenieError.YouDoNotOwnThatItem;
            return false;
        }

        if (!item.WasFoundBy(__instance))
        {

            var finderName = PlayerManager.FindByGuid(item.FinderId())?.Name ?? "<unknown>";
            __instance.SendMessage($"Equipping {item.Name} found by {finderName} {item.FinderId()}");

            __result = WeenieError.YouDoNotOwnThatItem;
            return false;
        }

        //Do regular checks
        return true;
    }

    #endregion

    #region Indicators
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.Name), MethodType.Getter)]
    public static void PreGetName(ref WorldObject __instance, ref string __result)
    {
        if (!__instance.IsSelfFound()) return;

        //Return false to override
        __result = $"{__instance.FinderName()}'s {__result}";
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AppraiseInfo), nameof(AppraiseInfo.BuildProfile), new Type[] { typeof(WorldObject), typeof(Player), typeof(bool) })]
    public static void PostBuildProfile(WorldObject wo, Player examiner, bool success, ref AppraiseInfo __instance)
    {
        if (!wo.IsSelfFound())
            return;

        if (!__instance.PropertiesString.TryGetValue(PropertyString.LongDesc, out var desc))
            return;

        __instance.PropertiesString[PropertyString.Name] = $"{wo.FinderName()}'s {__instance.PropertiesString[PropertyString.Name]}";
        __instance.PropertiesString[PropertyString.LongDesc] = $"{wo.FinderName()}'s {desc}";
    }
    #endregion

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.OnDeath), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
    public static void PostOnDeath(DamageHistoryInfo lastDamager, DamageType damageType, bool criticalHit, ref Player __instance, ref DeathMessage __result)
    {
        if(!PatchClass.Settings.Hardcore) return;

        //Taken from /deletecharacter
        if (__instance is Player player) //isOnline && 
        {
            //Corpse explosion?

            player.Character.DeleteTime = (ulong)Time.GetUnixTime();
            player.Character.IsDeleted = true;
            player.CharacterChangesDetected = true;
            player.Session.LogOffPlayer(true);
            PlayerManager.HandlePlayerDelete(player.Character.Id);

            var success = PlayerManager.ProcessDeletedPlayer(player.Character.Id);
            if (success)
                ModManager.Log($"Successfully deleted character {__instance.Name} (0x{__instance.Guid}).");
            else
                ModManager.Log($"Unable to delete character {__instance.Name} (0x{__instance.Guid}) due to PlayerManager failure.");
        }
    }



    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(LootGenerationFactory), nameof(LootGenerationFactory.CreateAndMutateWcid), new Type[] { typeof(TreasureDeath), typeof(TreasureRoll), typeof(bool) })]
    //public static void PostCreateAndMutateWcid(TreasureDeath treasureDeath, TreasureRoll treasureRoll, bool isMagical, ref WorldObject __result)
    //{
    //}


    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), "CheckWieldRequirements", new Type[] { typeof(WorldObject) })]
    //public static bool PreCheckWieldRequirements(WorldObject item, ref Player __instance, ref WeenieError __result)
    //{
    //    //Check SSF
    //    var allowedWielder = item.GetProperty(Settings.OriginalFinder);
    //    if (allowedWielder != null && (allowedWielder != __instance.Guid.Full))
    //    {
    //        __result = WeenieError.YouDoNotOwnThatItem; // Unsure of the exact message
    //        return false;
    //    }

    //    return true;
    //}
}
public static class Helpers
{
    public static void SetSelfFoundName(this WorldObject wo, Player player) => wo.SetProperty(PatchClass.Settings.OriginalFinderName, player.Name);
    public static string FinderName(this WorldObject wo) => wo.GetProperty(PatchClass.Settings.OriginalFinderName) ?? "";

    public static uint FinderId(this WorldObject wo) => wo.GetProperty(PatchClass.Settings.OriginalFinderId) ?? 0;

    public static bool IsSelfFound(this WorldObject wo) => wo.GetProperty(PatchClass.Settings.OriginalFinderId) != null;
    public static bool WasFoundBy(this WorldObject wo, Player player)
    {
        //var finderId = wo.GetProperty(PatchClass.Settings.OriginalFinder) ?? 0;
        return player.Guid.Full == wo.FinderId();
    }

    public static void MakeSelfFound(this WorldObject wo, Player player)
    {
        //Throw error? Unsure if check needed
        if (player is null) return;

        //Check for item type?
        wo.SetProperty(PatchClass.Settings.OriginalFinderId, player.Guid.Full);

        if (PatchClass.Settings.UseFinderName)
            wo.SetSelfFoundName(player);
    }
    public static void MakeSelfFound(this WorldObject wo, DamageHistoryInfo attackHistory)
    {
        //From GUID?
        //player.TryGetAttacker PlayerManager.FindByGuid(attackHistory.Guid);

        //Resolves pet issue?
        if (attackHistory.TryGetPetOwnerOrAttacker() is Player player)
            wo.MakeSelfFound(player);
    }

    public static void RemoveSelfFound(this WorldObject wo)
    {
        //Check for item type?
        wo.RemoveProperty(PatchClass.Settings.OriginalFinderId);
        wo.RemoveProperty(PatchClass.Settings.OriginalFinderName);
    }

    public static bool TryGetOwnerName(this WorldObject wo, out string name)
    {
        name = "";
        if (!wo.IsSelfFound()) return false;
        if (PlayerManager.FindByGuid(wo.FinderId()) is not Player player) return false;

        name = player.Name;
        return true;
    }

    public static void ToggleSelfFound(this Player player) => player.SetSelfFound(!player.IsSelfFound());
    public static bool IsSelfFound(this Player player) => player.GetProperty(PatchClass.Settings.SelfFound) ?? false;
    public static void SetSelfFound(this Player player, bool enabled) => player.SetProperty(PatchClass.Settings.SelfFound, enabled);

    public static void ClaimContainer(this Player player, Container container)
    {
        container.MakeSelfFound(player);

        foreach (var item in container.Inventory.Values)
            item.MakeSelfFound(player);

        if (!PatchClass.Settings.NotifyClaims)
            return;

        var sb = new StringBuilder($"\n{player.Name} is claiming {container.Name}");
        foreach (var item in container.Inventory.Values)
            sb.Append($"\n  {item.Name}");
        
        player.SendMessage(sb.ToString());
    }
}
