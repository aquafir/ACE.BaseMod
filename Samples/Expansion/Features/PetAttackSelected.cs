﻿namespace Expansion.Features;

[HarmonyPatchCategory(nameof(Feature.PetAttackSelected))]
internal class PetAttackSelected
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.OnTargetSelected), new Type[] { typeof(Player) })]
    public static void PreOnTargetSelected(Player player, ref Creature __instance, ref bool __result)
    {
        if (player.CurrentActivePet is null || __instance.IsNPC)
            return;

        //Follow
        if (player == __instance)
        {
            player.CurrentActivePet.StartFollow();
            player.SendMessage($"Your {player.CurrentActivePet.Name} is following you.");
        }
        else
        {
            player.SendMessage($"Your {player.CurrentActivePet.Name} is targeting {__instance.Name}");
            player.CurrentActivePet.AttackTarget = __instance;
        }
    }

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(Creature), nameof(Creature.OnTargetDeselected), new Type[] { typeof(Player) })]
    //public static void PreOnTargetDeselected(Player player, ref Creature __instance, ref bool __result)
    //{
    //    player?.CurrentActivePet?.StartFollow();
    //}


    //AttackTarget = nearest[0].Target;
}
