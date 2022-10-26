namespace ACE.BaseMod;

[HarmonyPatch]
public class PatchClass
{
    private static Settings _settings = new();
    private static string filePath = Path.Combine(Mod.ModPath, "Settings.json");

    public static void Start()
    {
        if (File.Exists(filePath))
        {
            try
            {
                ModManager.Log($"Loading from {filePath}");
                var jsonString = File.ReadAllText(filePath);
                _settings = JsonSerializer.Deserialize<Settings>(jsonString);
            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to deserialize from {filePath}");
                _settings = new Settings();
                return;
            }
        }
        else
        {
            ModManager.Log($"Creating {filePath}");
            string jsonString = JsonSerializer.Serialize(_settings);
            File.WriteAllText(filePath, jsonString);
        }
    }
    public static void Shutdown()
    {
        string jsonString = JsonSerializer.Serialize(_settings);
        File.WriteAllText(filePath, jsonString);
    }

    //Use Harmony attributes to override Player-on-NP crit chance using Settings
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponCriticalChance), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
    public static bool Prefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, ref float __result)
    {
        if (target is not Player)
        {
            __result = _settings.CritOverride;
            return false;
        }

        //Don't skip if not handled
        return true;
    }

    //Explicitly patch this in Mod.cs as a prefix for GetDeathMessage
    public static void PrefixDeathMessage(DamageHistoryInfo lastDamagerInfo, DamageType damageType, bool criticalHit, ref Creature __instance)
    {
        if (lastDamagerInfo.IsPlayer)
        {
        }
    }
}

