using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakeItemLoot))]
internal class FakeItemLoot
{
   // readonly Dictionary<(uint, )> 

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GenerateTreasure), new Type[] { typeof(DamageHistoryInfo), typeof(Corpse) })]
    public static void PreGenerateTreasure(DamageHistoryInfo killer, Corpse corpse, ref Creature __instance, ref List<WorldObject> __result)
    {
        if (!killer.IsPlayer)
            return;

        if (killer.TryGetPetOwnerOrAttacker() is not Player player)
            return;

        //Todo: think about efficiency
        //DeathTreasure is shared so I need to create a clone for individual use
     //   __instance.DeathTreasure = __instance.DeathTreasure.MemberwiseClone() as TreasureDeath;
        var treasure = __instance.DeathTreasure;
        if (treasure is null)
            return;
        
        //Short circuit roll?
        var chance = player.GetCachedFake(FakeFloat.ItemLootTierUpgrade);
        if(chance > 0 && ThreadSafeRandom.Next(0f, 1.0f) < chance)
        {
            treasure.Tier = Math.Min(8, treasure.Tier + 1);
            player.SendMessage($"Upgraded treasure tier of {corpse.Name} to {treasure.Tier}");
        }
    }


}
