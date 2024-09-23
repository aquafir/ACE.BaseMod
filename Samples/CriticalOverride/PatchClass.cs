namespace CriticalOverride;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    #region Patches
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponCriticalChance), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
    public static bool Prefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, ref float __result)
    {
        //Proceed normally with player targets
        if (target is Player)
            return true;

        __result = Settings.CritChance;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeaponMagicCritFrequency), new Type[] { typeof(WorldObject), typeof(Creature), typeof(CreatureSkill), typeof(Creature) })]
    public static bool MagicCritPrefix(WorldObject weapon, Creature wielder, CreatureSkill skill, Creature target, double __state, ref float __result)
    {
        //Proceed normally with player targets
        if (target is Player)
            return true;

        __result = Settings.MagicCritChance;
        return false;
    }
    #endregion
}