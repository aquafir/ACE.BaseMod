namespace Ironman.FlagEvents;

[HarmonyPatchCategory(nameof(FlagChest))]
public static class FlagChest
{
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
    public static void PostReset(double? resetTimestamp, ref Chest __instance) => __instance.RemoveProperty(FakeBool.Ironman);

    //Claim a container by unlocking it
    public static void HandleClaimChest(uint unlockerGuid, Chest container, UnlockResults result)
    {
        //Only care about successful unlocks
        if (result != UnlockResults.UnlockSuccess) return;

        //Check for Ironman players
        if (PlayerManager.GetOnlinePlayer(unlockerGuid) is not Player player || player.GetProperty(FakeBool.Ironman) != true)
            return;

        container.SetClaimedBy(player);
    }
}
