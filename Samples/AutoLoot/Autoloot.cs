using VTClassic;

namespace AutoLoot;

[HarmonyPatch]
public class AutoLoot
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GenerateTreasure), new Type[] { typeof(DamageHistoryInfo), typeof(Corpse) })]
    public static void PostGenerateTreasure(DamageHistoryInfo killer, Corpse corpse, ref Creature __instance, ref List<WorldObject> __result) {

        try {
            if (killer.TryGetPetOwnerOrAttacker() is not Player player)
            return;

        // TODO: user profile support...
        var profilePath = Path.Combine(ModManager.ModPath, "AutoLoot", "Profiles", "LootSnobV4.utl");

        var watch = Stopwatch.StartNew();
        var sb = new StringBuilder($"Using loot profile: {profilePath}");

        var pyreals = corpse.Inventory.Values.Where(x => x.ItemType == ItemType.Money);
        long amount = (long)pyreals.Select(x => x.Value).Sum();
        sb.Append($"\nAutoLooting {corpse.Name} of {pyreals.Count()} items worth {amount}.");

        foreach (var item in pyreals)
        {
            sb.Append($"\n{item.Name} - {item.Value}");
            item.Destroy();
        }

        var profile = new LootCore();
        profile.Startup();
        profile.LoadProfile(profilePath, false);

        var items = corpse.Inventory.Values.ToList();
        foreach (var item in items) {
            var lootAction = profile.GetLootDecision(item, player);

            if (lootAction.IsNoLoot)
                continue;

            sb.Append($"\nLooting {item.Name} because it matches {lootAction._lootAction} rule {lootAction.RuleName}");

            player.TryCreateInInventoryWithNetworking(item);
        }

        watch.Stop();
        sb.Append($"\n=====Looted in {watch.ElapsedMilliseconds} ms=====");

        player.SendMessage(sb.ToString());

        }
        catch (Exception ex) {
            Console.WriteLine(ex.ToString());
        }

    }
}
