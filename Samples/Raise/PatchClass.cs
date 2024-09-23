﻿using Raise;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    #region Settings
    //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
    const int RETRIES = 10;

    public static Settings Settings = new();
    private static string settingsPath = Path.Combine(ModC.ModPath, "Settings.json");
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
            ModC.State = ModState.Error;
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
            ModC.State = ModState.Error;
            return;
        }

        try
        {
            Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
        }
        catch (Exception)
        {
            ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
            ModC.State = ModState.Error;
            return;
        }
    }
    #endregion

    #region Start/Shutdown
    public static async void Start()
    {
        //Need to decide on async use
        ModC.State = ModState.Loading;
        LoadSettings();

        if (ModC.State == ModState.Error)
        {
            ModManager.DisableModByPath(ModC.ModPath);
            return;
        }

        Init();

        ModC.State = ModState.Running;
    }

    static async void Init()
    {
        while (true)
        {
            if (WorldManager.WorldStatus == WorldManager.WorldStatusState.Open)
                break;

            await Task.Delay(1000);
        }
        
        if(Settings.AlternateLeveling.Enabled)
        {

            ModC.Harmony.PatchCategory(nameof(AlternateLeveling));
            ModC.Container.RegisterCommandCategory(nameof(AlternateLeveling));
        }

        storedCosts = DatManager.PortalDat.XpTable.CharacterLevelXPList.ToList();
        storedCredits = DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.ToList();
        SetMaxLevel();
    }

    public static void Shutdown()
    {
        if (ModC.State == ModState.Running)
        {
            RestoreMaxLevel();
        }

        if (ModC.State == ModState.Error)
            ModManager.Log($"Improper shutdown: {ModC.ModPath}", ModManager.LogLevel.Error);
    }
    #endregion

    //Probably shouldn't be doing this except at the start/end...
    static List<ulong> storedCosts = new();
    static List<uint> storedCredits = new();
    private static void SetMaxLevel()
    {
        RestoreMaxLevel();

        //Add levels up to max
        for (int i = DatManager.PortalDat.XpTable.CharacterLevelXPList.Count; i <= Settings.MaxLevel; i++)
        {
            //var cost = DatManager.PortalDat.XpTable.CharacterLevelXPList.Last() + PatchClass.Settings.CostPerLevel;
            var cost = DatManager.PortalDat.XpTable.CharacterLevelXPList.Last() + (ulong)Settings.LevelCost.GetCost(i);
            var credits = (uint)(i % Settings.CreditInterval == 0 ? 1 : 0);
            DatManager.PortalDat.XpTable.CharacterLevelXPList.Add(cost);
            DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Add(credits);
            //session?.Player?.SendMessage($"Adding level {i} for {cost}.  {credits} skill credits.");
        }

        ModManager.Log($"Set max level to {Settings.MaxLevel}");
    }

    private static async void RestoreMaxLevel()
    {
        //Restored the original values...
        DatManager.PortalDat.XpTable.CharacterLevelXPList.Clear();
        DatManager.PortalDat.XpTable.CharacterLevelXPList.AddRange(storedCosts);
        DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.Clear();
        DatManager.PortalDat.XpTable.CharacterLevelSkillCreditList.AddRange(storedCredits);

        ModManager.Log($"Restored max level to {DatManager.PortalDat.XpTable.CharacterLevelXPList.Count}");
    }
}

