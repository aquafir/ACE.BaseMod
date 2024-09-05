namespace Expansion.Features;

[CommandCategory(nameof(Feature.AutoLoot))]
[HarmonyPatchCategory(nameof(Feature.AutoLoot))]
internal class AutoLoot
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GenerateTreasure), new Type[] { typeof(DamageHistoryInfo), typeof(Corpse) })]
    public static void PostGenerateTreasure(DamageHistoryInfo killer, Corpse corpse, ref Creature __instance, ref List<WorldObject> __result)
    {
        if (killer.TryGetPetOwnerOrAttacker() is not Player player)
            return;

        var pyreals = corpse.Inventory.Values.Where(x => x.ItemType == ItemType.Money || x.Value > 20000);
        long amount = (long)pyreals.Select(x => x.Value).Sum();

        var watch = Stopwatch.StartNew();
        var sb = new StringBuilder($"AutoLooting {corpse.Name} of {pyreals.Count()} items worth {amount}.");

        foreach (var item in pyreals)
        {
            sb.Append($"\n{item.Name} - {item.Value}");
            item.Destroy();
        }
        var total = amount + player.GetProperty(FakeInt64.Pyreals).GetValueOrDefault();
        player.SetProperty(FakeInt64.Pyreals, total);

        var casters = corpse.Inventory.Values.Where(x => x.ItemType.HasAny(ItemType.WeaponOrCaster | ItemType.Jewelry | ItemType.Clothing));
        foreach (var item in casters)
        {
            //player.TryAdd
            //if (player.TryAddToInventory(item))            {
            if (player.TryCreateInInventoryWithNetworking(item))
            {
                sb.Append($"\nLoot {item.Name}");
            }
        }


        sb.Append($"\nPlayer pyreals = {total}");

        watch.Stop();
        sb.Append($"\n=====Looted in {watch.ElapsedMilliseconds} ms=====");

        player.SendMessage(sb.ToString());

    }





    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.Generator_Generate))]
    public static bool PreGenerator_Generate(ref WorldObject __instance)
    {
        //Return false to override
        //return false;
        //        Debugger.Break();

        //Return true to execute original
        return true;
    }

}
