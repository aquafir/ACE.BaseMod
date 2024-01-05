using ACE.Database;
using ACE.Database.Entity;
using ACE.Database.Models.Shard;
using ACE.DatLoader.Entity;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Entity.Actions;
using ACE.Server.Factories;
using ACE.Server.Managers;
using ACE.Server.Network.Enum;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Network.Handlers;
using ACE.Server.WorldObjects;
using CustomLoot.Enums;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using static ACE.Server.Factories.PlayerFactory;
using static ACE.Server.Mods.ModManager;
using static System.Net.Mime.MediaTypeNames;
using Biota = ACE.Entity.Models.Biota;

namespace Ironman;

[HarmonyPatch]
public class PatchClass
{
    #region Settings
    const int RETRIES = 10;
    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);

    private JsonSerializerOptions _serializeOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
    private static int Alive;

    private void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

        if (!settingsInfo.RetryWrite(jsonString, RETRIES))
        {
            ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
            Mod.State = ModState.Error;
        }
    }

    private void LoadSettings()
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
    public void Start()
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

    public void Shutdown()
    {
        //if (Mod.State == ModState.Running)
        // Shut down enabled mod...

        //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
        //SaveSettings();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    private const string NAME_SUFFIX = "-Im";

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerFactory), nameof(PlayerFactory.Create), new Type[] { typeof(CharacterCreateInfo), typeof(Weenie), typeof(ObjectGuid), typeof(uint), typeof(WeenieType), typeof(Player) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
    public static void PreCreate(CharacterCreateInfo characterCreateInfo, Weenie weenie, ObjectGuid guid, uint accountId, WeenieType weenieType, Player player, ref CreateResult __result)
    {
        //Only apply to templated players
        if (characterCreateInfo.TemplateOption == 0)
            return;

        characterCreateInfo.Name = $"{characterCreateInfo.Name}-Im";
        //characterCreateInfo.Name = $"ø{characterCreateInfo.Name}";

        //Randomize heritage / appearance
        PlayerFactoryEx.RandomizeHeritage(characterCreateInfo);

        //Set primary attr
        var primaryAttr = ThreadSafeRandom.Next(0, 5);
        characterCreateInfo.CoordinationAbility = 46;
        characterCreateInfo.EnduranceAbility = 46;
        characterCreateInfo.FocusAbility = 46;
        characterCreateInfo.QuicknessAbility = 46;
        characterCreateInfo.SelfAbility = 46;
        characterCreateInfo.StrengthAbility = 46;

        switch (primaryAttr)
        {
            case 0:
                characterCreateInfo.CoordinationAbility = 100;
                break;
            case 1:
                characterCreateInfo.EnduranceAbility = 100;
                break;
            case 2:
                characterCreateInfo.FocusAbility = 100;
                break;
            case 3:
                characterCreateInfo.QuicknessAbility = 100;
                break;
            case 4:
                characterCreateInfo.SelfAbility = 100;
                break;
            case 5:
                characterCreateInfo.StrengthAbility = 100;
                break;
        }

        //Reset skills seemed like a pain to do here
        pendingFinalization.Add(guid.Full);
    }

    //Tracks created but unfinished players
    static readonly HashSet<uint> pendingFinalization = new();
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.DoPlayerEnterWorld), new Type[] { typeof(Session), typeof(Character), typeof(Biota), typeof(PossessedBiotas) })]
    public static void PostDoPlayerEnterWorld(Session session, Character character, Biota playerBiota, PossessedBiotas possessedBiotas)
    {
        //Check for finalizing / grab the player
        if (!pendingFinalization.Contains(character.Id))
            return;

        var player = PlayerManager.GetOnlinePlayer(character.Id);
        if (player is null)
            return;
        pendingFinalization.Remove(character.Id);

        var actionChain = new ActionChain();
        actionChain.AddDelaySeconds(5);
        actionChain.AddAction(session.Player, () =>
        {
            //Set lives/Ironman props
            player.ApplyIronman();

            //Roll skills
            player.RollIronmanSkills();

            //Welcome them
            player.SendMessage(Settings.WelcomeMessage);
            for (var i = 0; i < 20; i++)
                player.PlayParticleEffect(Enum.GetValues<PlayScript>().Random(), player.Guid, i * .02f);
        });
        actionChain.EnqueueChain();
    }

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
                ModManager.Log($"Unable to parse Skill from plan @ {plan[i]}", LogLevel.Error);
                return;
            }

            //Failed to train/spec
            if (!__instance.TryAdvanceSkill(skill))
                break;
        }

        //Nothing changed
        if (i == 0)
            return;

        var msg = $"{__instance.Name} advanced their skill plan {i} steps with {String.Join("->", plan.Take(i))}";
        ModManager.Log(msg);
        __instance.SendMessage(msg);

        //Store the update plan
        __instance.SetProperty(FakeString.IronmanPlan, String.Join(';', plan.Skip(i)));
        __instance.SendUpdatedSkills();
    }

    #region Commands
    static DateTime timestampLeaderboard = DateTime.MinValue;
    static DateTime timestampGrave = DateTime.MinValue;
    static string lastLeaderboard = "";
    static string lastGrave = "";
    static TimeSpan cacheInterval = TimeSpan.FromSeconds(60);

    [CommandHandler("leaderboard", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleLeaderboard(Session session, params string[] parameters)
    {
        var lapse = DateTime.Now - timestampLeaderboard;
        if (lapse < cacheInterval)
        {
            session.Player.SendMessage($"{lastLeaderboard}");
            return;
        }

        var sb = new StringBuilder();
        var players = PlayerManager.GetAllPlayers().Where(x => x.Name.EndsWith(NAME_SUFFIX));
        foreach (var player in players.OrderByDescending(x => x.Level))
        {
            if (player is not null)
                sb.Append($"\n  {player.Level,-8}{player.Name}");
        }

        timestampLeaderboard = DateTime.Now;
        lastLeaderboard = sb.ToString();

        session.Player.SendMessage($"{sb}");
    }

    [CommandHandler("grave", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleGrave(Session session, params string[] parameters)
    {
        var lapse = DateTime.Now - timestampGrave;
        if (lapse < cacheInterval)
        {
            session.Player.SendMessage($"{lastGrave}");
            return;
        }

        var characters = GetCharacterList();
        var sb = new StringBuilder();

        foreach (var account in characters.OrderBy(x => x.IsDeleted).GroupBy(x => x.AccountId))
        {
            var acct = DatabaseManager.Authentication.GetAccountById(account.Key);
            sb.Append($"\n======={acct.AccountName} ({account.Count()})=======");

            foreach (var character in account.Where(x => x.Name.EndsWith(NAME_SUFFIX)))
                sb.Append($"\n  {character.Name} - Dead");
        }

        timestampGrave = DateTime.Now;
        lastGrave = sb.ToString();

        session.Player.SendMessage($"{sb}");
    }

    private static readonly ConditionalWeakTable<Character, ShardDbContext> CharacterContexts = new ConditionalWeakTable<Character, ShardDbContext>();
    private static List<Character> GetCharacterList()
    {
        var context = new ShardDbContext();

        IQueryable<Character> query;
        query = context.Character.Where(r => r.IsDeleted && r.Name.EndsWith(NAME_SUFFIX));

        var results = query.ToList();

        for (int i = 0; i < results.Count; i++)
        {
            // Do we have a reference to this Character already?
            var existingChar = CharacterContexts.FirstOrDefault(r => r.Key.Id == results[i].Id);

            if (existingChar.Key != null)
                results[i] = existingChar.Key;
            else
            {
                // No reference, pull all the properties and add it to the cache
                //query.Include(r => r.CharacterPropertiesContractRegistry).Load();
                CharacterContexts.Add(results[i], context);
            }
        }

        return results;
    }


    [CommandHandler("plan", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandlePlan(Session session, params string[] parameters)
    {
        var player = session.Player;
        if (player is null || player.GetProperty(FakeBool.Ironman) != true)
            return;

        player.SendMessage($"\n{player.GetProperty(FakeString.IronmanPlan)}\n\n{player.GetProperty(FakeString.IronmanFullPlan)}");
    }

    #endregion
}
