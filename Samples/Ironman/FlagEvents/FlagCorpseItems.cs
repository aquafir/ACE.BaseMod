namespace Ironman.FlagEvents;

[HarmonyPatchCategory(nameof(FlagCorpseItems))]
public static class FlagCorpseItems
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GenerateTreasure), new Type[] { typeof(DamageHistoryInfo), typeof(Corpse) })]
    public static void PostGenerateTreasure(DamageHistoryInfo killer, Corpse corpse, Creature __instance, List<WorldObject> __result)
    {
        if (killer is null || killer.TryGetPetOwnerOrAttacker() is not Player player)
            return;

        if (player.GetProperty(FakeBool.Ironman) != true)
            return;

        //foreach (var item in __result)
        foreach (var item in corpse.Inventory.Values)
            item.SetProperty(FakeBool.Ironman, true);

        //player.SendMessage($"Claimed corpse");
    }
}
