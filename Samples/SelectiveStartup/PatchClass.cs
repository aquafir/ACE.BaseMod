using ACE.Database;
using ACE.Server.Network.Managers;

namespace SelectiveStartup;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EventManager), nameof(EventManager.Initialize))]
    public static bool PreInitialize1()
    {
        return !Settings.Skip.Contains(ServerTask.EventManager);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.Initialize))]
    public static bool PreInitialize2()
    {
        return !Settings.Skip.Contains(ServerTask.WorldManager);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.Initialize))]
    public static bool PreInitialize3()
    {
        return !Settings.Skip.Contains(ServerTask.NetworkManager);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SocketManager), nameof(SocketManager.Initialize))]
    public static bool PreInitialize4()
    {
        return !Settings.Skip.Contains(ServerTask.SocketManager);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(InboundMessageManager), nameof(InboundMessageManager.Initialize))]
    public static bool PreInitialize5()
    {
        return !Settings.Skip.Contains(ServerTask.InboundMessageManager);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HouseManager), nameof(HouseManager.Initialize))]
    public static bool PreInitialize6()
    {
        return !Settings.Skip.Contains(ServerTask.HouseManager);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.Initialize))]
    public static bool PreInitialize7()
    {
        return !Settings.Skip.Contains(ServerTask.PlayerManager);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RealmManager), nameof(RealmManager.Initialize))]
    public static bool PreInitialize8()
    {
        return !Settings.Skip.Contains(ServerTask.RealmManager);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GuidManager), nameof(GuidManager.Initialize))]
    public static bool PreInitialize9()
    {
        return !Settings.Skip.Contains(ServerTask.GuidManager);
    }

    static bool propManagerPatched = false;
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PropertyManager), nameof(PropertyManager.Initialize))]
    public static bool PreInitialize10()
    {
        if (Settings.Skip.Contains(ServerTask.PropertyManager) && !propManagerPatched)
        {
            propManagerPatched = true;
            Mod.Instance.Harmony.PatchCategory(nameof(ServerTask.PropertyManager));
        }

        return !Settings.Skip.Contains(ServerTask.PropertyManager);
    }
    [HarmonyPrefix]

    [HarmonyPatch(typeof(DatabaseManager), nameof(DatabaseManager.Initialize))]
    public static bool PreInitialize11()
    {
        return !Settings.Skip.Contains(ServerTask.DatabaseManager);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DatabaseManager), nameof(DatabaseManager.Start))]
    public static bool PreStart(ref DatabaseManager __instance)
    {
        return !Settings.Skip.Contains(ServerTask.DatabaseManager);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DDDManager), nameof(DDDManager.Initialize))]
    public static bool PreInitialize12()
    {
        return !Settings.Skip.Contains(ServerTask.DDDManager);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ServerManager), nameof(ServerManager.Initialize))]
    public static bool PreInitialize14()
    {
        return !Settings.Skip.Contains(ServerTask.ServerManager);
    }

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(ShardDatabaseOfflineTools), nameof(ShardDatabaseOfflineTools.CheckForBiotaPropertiesPaletteOrderColumnInShard))]
    //public static bool PreCheckForBiotaPropertiesPaletteOrderColumnInShard()
    //{
    //    return !Settings.Skip.Contains(ServerTask.CheckBiotaPropertiesPaletteOrder);
    //}



    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(DatManager), nameof(DatManager.Initialize))]
    //public static bool PreInitialize13()
    //{
    //    return !Settings.Skip.Contains(ServerTask.DatManager);
    //}
    ////Fake dat data used
    //static PortalDatDatabase fakeDat;
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(DatManager), nameof(DatManager.PortalDat), MethodType.Getter)]
    //public static bool PreGetPortalDat(ref PortalDatDatabase __result)
    //{
    //    if (Settings.Skip.Contains(ServerTask.DatManager))
    //    {
    //        fakeDat = new("");
    //        __result = fakeDat;
    //        //DatManager.PortalDat.RegionDesc.LandDefs.LandHeightTable = new();

    //        return false;
    //    }

    //    //Return true to execute original
    //    return true;
    //}

    public static void FakeDatInitialize(string datFileDirectory, bool keepOpen = false, bool loadCell = true)
    {
        var datDir = Path.GetFullPath(Path.Combine(datFileDirectory));

        //if (loadCell)
        //{
        //    try
        //    {
        //        datFile = Path.Combine(datDir, "client_cell_1.dat");
        //        CellDat = new CellDatDatabase(datFile, keepOpen);
        //        count = CellDat.AllFiles.Count;
        //        log.Info($"Successfully opened {datFile} file, containing {count} records, iteration {CellDat.Iteration}");
        //        if (CellDat.Iteration != ITERATION_CELL)
        //            log.Warn($"{datFile} iteration does not match expected end-of-retail version of {ITERATION_CELL}.");
        //    }
        //    catch (FileNotFoundException ex)
        //    {
        //        log.Error($"An exception occured while attempting to open {datFile} file!  This needs to be corrected in order for Landblocks to load!");
        //        log.Error($"Exception: {ex.Message}");
        //    }
        //}

        //try
        //{
        //    datFile = Path.Combine(datDir, "client_portal.dat");
        //    PortalDat = new PortalDatDatabase(datFile, keepOpen);
        //    PortalDat.SkillTable.AddRetiredSkills();
        //    count = PortalDat.AllFiles.Count;
        //    log.Info($"Successfully opened {datFile} file, containing {count} records, iteration {PortalDat.Iteration}");
        //    if (PortalDat.Iteration != ITERATION_PORTAL)
        //        log.Warn($"{datFile} iteration does not match expected end-of-retail version of {ITERATION_PORTAL}.");
        //}
        //catch (FileNotFoundException ex)
        //{
        //    log.Error($"An exception occured while attempting to open {datFile} file!\n\n *** Please check your 'DatFilesDirectory' setting in the config.js file. ***\n *** ACE will not run properly without this properly configured! ***\n");
        //    log.Error($"Exception: {ex.Message}");
        //}

        //// Load the client_highres.dat file. This is not required for ACE operation, so no exception needs to be generated.
        //datFile = Path.Combine(datDir, "client_highres.dat");
        //if (File.Exists(datFile))
        //{
        //    HighResDat = new DatDatabase(datFile, keepOpen);
        //    count = HighResDat.AllFiles.Count;
        //    log.Info($"Successfully opened {datFile} file, containing {count} records, iteration {HighResDat.Iteration}");
        //    if (HighResDat.Iteration != ITERATION_HIRES)
        //        log.Warn($"{datFile} iteration does not match expected end-of-retail version of {ITERATION_HIRES}.");
        //}

        //try
        //{
        //    datFile = Path.Combine(datDir, "client_local_English.dat");
        //    LanguageDat = new LanguageDatDatabase(datFile, keepOpen);
        //    count = LanguageDat.AllFiles.Count;
        //    log.Info($"Successfully opened {datFile} file, containing {count} records, iteration {LanguageDat.Iteration}");
        //    if (LanguageDat.Iteration != ITERATION_LANGUAGE)
        //        log.Warn($"{datFile} iteration does not match expected end-of-retail version of {ITERATION_LANGUAGE}.");
        //}
        //catch (FileNotFoundException ex)
        //{
        //    log.Error($"An exception occured while attempting to open {datFile} file!\n\n *** Please check your 'DatFilesDirectory' setting in the config.json file. ***\n *** ACE will not run properly without this properly configured! ***\n");
        //    log.Error($"Exception: {ex.Message}");
        //}
    }
}
