using ACE.Database.Models.World;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;
using System.Numerics;

namespace Tinkering;

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

        //Add difficulties
        ModifyTinkering();

        Mod.State = ModState.Running;
    }

    public static void Shutdown()
    {
        //Restore difficulties
        if (Mod.State == ModState.Running)
            RestoreTinkering();

        if (Mod.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    //Preserve/restore tinkering difficulties on start/shutdown
    static readonly List<float> difficulty = RecipeManager.TinkeringDifficulty.ToList();
    private static void RestoreTinkering()
    {
        RecipeManager.TinkeringDifficulty = difficulty.ToList();
    }

    private static void ModifyTinkering()
    {
        var diffs = RecipeManager.TinkeringDifficulty.Count;
        var last = RecipeManager.TinkeringDifficulty.Last();
        var toAdd = Math.Max(0, Settings.MaxTries - diffs);
        var steps = Enumerable.Range(diffs, toAdd)
            .Select((x, i) => last + (i + 1) * PatchClass.Settings.Scale);

        RecipeManager.TinkeringDifficulty.AddRange(steps);
        ModManager.Log($"Tink diffs ({RecipeManager.TinkeringDifficulty.Count}): {String.Join(", ", RecipeManager.TinkeringDifficulty)}");
    }

    //Full override of VerifyRequirements
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RecipeManager), nameof(RecipeManager.VerifyRequirements), new Type[] { typeof(Recipe), typeof(Player), typeof(WorldObject), typeof(RequirementType) })]
    public static bool PreVerifyRequirements(Recipe recipe, Player player, WorldObject obj, RequirementType reqType, ref RecipeManager __instance, ref bool __result)
    {
        //Use normal method unless using Target reqs just to be safe?
        //if (reqType != RequirementType.Target) __result = true;

        #region Setup
        //Assume it succeeds
        __result = true;

        var boolReqs = recipe.RecipeRequirementsBool.Where(i => i.Index == (int)reqType).ToList();
        var intReqs = recipe.RecipeRequirementsInt.Where(i => i.Index == (int)reqType).ToList();
        var floatReqs = recipe.RecipeRequirementsFloat.Where(i => i.Index == (int)reqType).ToList();
        var strReqs = recipe.RecipeRequirementsString.Where(i => i.Index == (int)reqType).ToList();
        var iidReqs = recipe.RecipeRequirementsIID.Where(i => i.Index == (int)reqType).ToList();
        var didReqs = recipe.RecipeRequirementsDID.Where(i => i.Index == (int)reqType).ToList();

        var totalReqs = boolReqs.Count + intReqs.Count + floatReqs.Count + strReqs.Count + iidReqs.Count + didReqs.Count;

        if (RecipeManager.Debug && totalReqs > 0)
            Console.WriteLine($"{reqType} Requirements: {totalReqs}");

        #endregion

        foreach (var requirement in intReqs)
        {
            int? value = obj.GetProperty((PropertyInt)requirement.Stat);
            double? normalized = value != null ? (double?)Convert.ToDouble(value.Value) : null;

            //Customize NumTink reqs
            var propInt = (PropertyInt)requirement.Stat;
            var comp = (CompareType)requirement.Enum;


            if (propInt == PropertyInt.ImbuedEffect)
            {
                value = obj.GetProperty((PropertyInt)requirement.Stat) ?? 0;
            }

            //Todo: think about nulls
            __result = propInt switch
            {
                PropertyInt.NumTimesTinkered => comp.Compare((PatchClass.Settings.MaxTries - 1), value ?? 0, player, requirement.Message) ? __result : false, //Only set false
                PropertyInt.ImbuedEffect => comp.Compare((PatchClass.Settings.MaxImbueEffects - 1), BitOperations.PopCount((uint)(value ?? 0)), player, requirement.Message) ? __result : false,
                _ => RecipeManager.VerifyRequirement(player, (CompareType)requirement.Enum, normalized, Convert.ToDouble(requirement.Value), requirement.Message),
            };
            //BitOperations.PopCount()
            //value is not null)
            //{
                //Handle check here?
                //var comp = (CompareType)requirement.Enum;
                //var tries = PatchClass.Settings.MaxTries - 1;   //Todo: figure out if I'm dumb

                //__result = comp.Compare(tries, value.Value);

                //Skip regular check.  Can't return prematurely
                continue;
            //}

            //if (RecipeManager.Debug)
            //    Console.WriteLine($"PropertyInt.{(PropertyInt)requirement.Stat} {(CompareType)requirement.Enum} {requirement.Value}, current: {value}");

            //Default case
            //if (!RecipeManager.VerifyRequirement(player, (CompareType)requirement.Enum, normalized, Convert.ToDouble(requirement.Value), requirement.Message))
            //    __result = false;
        }

        #region Unmodified Requirement Checks
        foreach (var requirement in boolReqs)
        {
            bool? value = obj.GetProperty((PropertyBool)requirement.Stat);
            double? normalized = value != null ? (double?)Convert.ToDouble(value.Value) : null;

            if (RecipeManager.Debug)
                Console.WriteLine($"PropertyBool.{(PropertyBool)requirement.Stat} {(CompareType)requirement.Enum} {requirement.Value}, current: {value}");

            if (!RecipeManager.VerifyRequirement(player, (CompareType)requirement.Enum, normalized, Convert.ToDouble(requirement.Value), requirement.Message))
                __result = false;
        }


        foreach (var requirement in floatReqs)
        {
            double? value = obj.GetProperty((PropertyFloat)requirement.Stat);

            if (RecipeManager.Debug)
                Console.WriteLine($"PropertyFloat.{(PropertyFloat)requirement.Stat} {(CompareType)requirement.Enum} {requirement.Value}, current: {value}");

            if (!RecipeManager.VerifyRequirement(player, (CompareType)requirement.Enum, value, requirement.Value, requirement.Message))
                __result = false;
        }

        foreach (var requirement in strReqs)
        {
            string value = obj.GetProperty((PropertyString)requirement.Stat);

            if (RecipeManager.Debug)
                Console.WriteLine($"PropertyString.{(PropertyString)requirement.Stat} {(CompareType)requirement.Enum} {requirement.Value}, current: {value}");

            if (!RecipeManager.VerifyRequirement(player, (CompareType)requirement.Enum, value, requirement.Value, requirement.Message))
                __result = false;
        }

        foreach (var requirement in iidReqs)
        {
            uint? value = obj.GetProperty((PropertyInstanceId)requirement.Stat);
            double? normalized = value != null ? (double?)Convert.ToDouble(value.Value) : null;

            if (RecipeManager.Debug)
                Console.WriteLine($"PropertyInstanceId.{(PropertyInstanceId)requirement.Stat} {(CompareType)requirement.Enum} {requirement.Value}, current: {value}");

            if (!RecipeManager.VerifyRequirement(player, (CompareType)requirement.Enum, normalized, Convert.ToDouble(requirement.Value), requirement.Message))
                __result = false;
        }

        foreach (var requirement in didReqs)
        {
            uint? value = obj.GetProperty((PropertyDataId)requirement.Stat);
            double? normalized = value != null ? (double?)Convert.ToDouble(value.Value) : null;

            if (RecipeManager.Debug)
                Console.WriteLine($"PropertyDataId.{(PropertyDataId)requirement.Stat} {(CompareType)requirement.Enum} {requirement.Value}, current: {value}");

            if (!RecipeManager.VerifyRequirement(player, (CompareType)requirement.Enum, normalized, Convert.ToDouble(requirement.Value), requirement.Message))
                __result = false;
        }

        if (RecipeManager.Debug && totalReqs > 0)
            Console.WriteLine($"-----");
        #endregion

        //Skip method, return success
        return false;
    }

    //Checks for an imbue, even if not using useMutateNative
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RecipeManager), nameof(RecipeManager.TryMutate), new Type[] { typeof(Player), typeof(WorldObject), typeof(WorldObject), typeof(Recipe), typeof(uint), typeof(HashSet<uint>) })]
    public static bool PreTryMutate(Player player, WorldObject source, WorldObject target, Recipe recipe, uint dataId, HashSet<uint> modified, ref RecipeManager __instance, ref bool __result)
    {
        //Check for imbue to override
        if (!imbueDataIDs.TryGetValue(dataId, out var imbueEffect))
            return true;

        //Todo: check redundant?
        target.ImbuedEffect |= imbueEffect;

        if (RecipeManager.incItemTinkered.Contains(dataId))
            RecipeManager.HandleTinkerLog(source, target);

        //Success
        __result = true;

        return false;
    }

    //DID->Imbue used by RecipeManager.TryMutateNative
    private static readonly Dictionary<uint, ImbuedEffectType> imbueDataIDs = new()
    {
        [0x38000038] = ImbuedEffectType.MeleeDefense,
        [0x38000039] = ImbuedEffectType.MissileDefense,
        [0x38000037] = ImbuedEffectType.MagicDefense,
        [0x38000025] = ImbuedEffectType.ArmorRending,
        [0x38000024] = ImbuedEffectType.CripplingBlow,
        [0x38000023] = ImbuedEffectType.CriticalStrike,
        [0x3800003A] = ImbuedEffectType.AcidRending,
        [0x3800003B] = ImbuedEffectType.BludgeonRending,
        [0x3800003C] = ImbuedEffectType.ColdRending,
        [0x3800003D] = ImbuedEffectType.ElectricRending,
        [0x3800003E] = ImbuedEffectType.FireRending,
        [0x3800003F] = ImbuedEffectType.PierceRending,
        [0x38000040] = ImbuedEffectType.SlashRending,
        [0x38000041] = ImbuedEffectType.Spellbook,
    };

    //RecipeManager.UseObjectOnTarget
    //WorldObject.HandleActionUseOnTarget
}
