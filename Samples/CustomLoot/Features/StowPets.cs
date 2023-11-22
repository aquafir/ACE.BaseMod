using ACE.Adapter.Enum;

namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.StowPets))]
internal class StowPets
{

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Pet), nameof(Pet.HandleCurrentActivePet_Replace), new Type[] { typeof(Player) })]
    public static bool PreHandleCurrentActivePet_Replace(Player player, ref Pet __instance, ref bool __result)
    {
        // original ace logic
        if (player.CurrentActivePet == null)
            return true;

        if (player.CurrentActivePet is CombatPet)
        {
            // possibly add the ability to stow combat pets with passive pet devices here?
            //player.SendTransientError($"{player.CurrentActivePet.Name} is already active");
            //return false;
        }

        var stowPet = __instance.WeenieClassId == player.CurrentActivePet.WeenieClassId;

        // despawn passive pet
        player.CurrentActivePet.Destroy();

        return !stowPet;
    }
}


