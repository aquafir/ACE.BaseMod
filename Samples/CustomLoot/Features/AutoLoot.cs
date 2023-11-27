using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoot.Features;

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

        var casters = corpse.Inventory.Values.Where(x => (x.ItemType & (ItemType.WeaponOrCaster | ItemType.Jewelry | ItemType.Clothing)) > 0);
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
}
