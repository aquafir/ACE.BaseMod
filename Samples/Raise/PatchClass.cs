using Raise;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnWorldOpen()
    {
        Settings = SettingsContainer.Settings;

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

    public override void Stop()
    {
        base.Stop();

        if (ModC.State == ModState.Running)
        {
            RestoreMaxLevel();
        }
    }

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

