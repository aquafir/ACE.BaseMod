namespace Expansion.Features;

[CommandCategory(nameof(Feature.PetStow))]
[HarmonyPatchCategory(nameof(Feature.PetStow))]
internal class PetStow
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PetDevice), nameof(PetDevice.CheckUseRequirements), new Type[] { typeof(WorldObject) })]
    public static bool PreCheckUseRequirements(WorldObject activator, ref PetDevice __instance, ref ActivationResult __result)
    {
        if (!(activator is Player player))
        {
            __result = new ActivationResult(false);
            return false;
        }

        var pet = player.CurrentActivePet;
        if (player.CurrentActivePet != null && player.CurrentActivePet is CombatPet)
        {
            if (pet.P_PetDevice is not null)
            {
                pet.Destroy();
                pet.P_PetDevice.Structure++;
                player.SendMessage($"Unsummoned {__instance.Name} ({pet.P_PetDevice.Structure} / {pet.P_PetDevice.MaxStructure ?? 0})");
            }
        }

        ////!!!!!!!!!!!!!Base requirements might be missed?
        ////var baseRequirements = __instance.CheckUseRequirements(activator);
        ////if (!baseRequirements.Success)
        ////    return baseRequirements;

        //// verify summoning mastery
        //if (__instance.SummoningMastery != null && player.SummoningMastery != __instance.SummoningMastery)
        //{
        //    player.Session.Network.EnqueueSend(new GameMessageSystemChat($"You must be a {__instance.SummoningMastery} to use the {__instance.Name}", ChatMessageType.Broadcast));
        //    __result = new ActivationResult(false);
        //}

        //// duplicating some of this verification logic here from Pet.Init()
        //// since the PetDevice owner and the summoned Pet are separate objects w/ potentially different heartbeat offsets,
        //// the cooldown can still expire before the CombatPet's lifespan
        //// in this case, if the player tries to re-activate the PetDevice while the CombatPet is still in the world,
        //// we want to return an error without re-activating the cooldown

        //if (player.CurrentActivePet != null && player.CurrentActivePet is CombatPet)
        //{
        //    if (PropertyManager.GetBool("pet_stow_replace").Item)
        //    {
        //        // original ace
        //        player.SendTransientError($"{player.CurrentActivePet.Name} is already active");
        //        __result = new ActivationResult(false);
        //    }
        //    else
        //    {
        //        // retail stow
        //        var weenie = DatabaseManager.World.GetCachedWeenie((uint)__instance.PetClass);

        //        if (weenie == null || weenie.WeenieType != WeenieType.Pet)
        //        {
        //            player.SendTransientError($"{player.CurrentActivePet.Name} is already active");
        //            __result = new ActivationResult(false);
        //        }
        //    }
        //}
        //__result = new ActivationResult(true);
        return true;
    }



    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Pet), nameof(Pet.HandleCurrentActivePet_Replace), new Type[] { typeof(Player) })]
    //public static bool PreHandleCurrentActivePet_Replace(Player player, ref Pet __instance, ref bool __result)
    //{
    //    // original ace logic
    //    if (player.CurrentActivePet == null)
    //        return true;

    //    if (player.CurrentActivePet is CombatPet pet)
    //    {
    //        // possibly add the ability to stow combat pets with passive pet devices here?
    //        //player.SendTransientError($"{player.CurrentActivePet.Name} is already active");
    //        //return false;
    //        if(pet.P_PetDevice is not null)
    //        {
    //            pet.P_PetDevice.Structure++;
    //            player.SendMessage($"Unsummoned {__instance.Name} ({pet.P_PetDevice.Structure} / {pet.P_PetDevice.MaxStructure ?? 0})");
    //        } 
    //    }

    //    var stowPet = __instance.WeenieClassId == player.CurrentActivePet.WeenieClassId;

    //    // despawn passive pet
    //    player.CurrentActivePet.Destroy();

    //    return !stowPet;
    //}

    /////modifybool pet_stow_replace false decides which method is used
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Pet), nameof(Pet.HandleCurrentActivePet_Retail), new Type[] { typeof(Player) })]
    //public static bool PreHandleCurrentActivePet_Retail(Player player, ref Pet __instance, ref bool? __result)
    //{
    //    //Return false to override
    //    //return false;

    //    //Return true to execute original
    //    return true;
    //}
}


