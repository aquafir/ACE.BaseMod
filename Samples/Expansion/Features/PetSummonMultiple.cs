using ACE.Database;
using System.Collections.Concurrent;

namespace Expansion.Features;

[CommandCategory(nameof(Feature.PetSummonMultiple))]
[HarmonyPatchCategory(nameof(Feature.PetSummonMultiple))]
public static class PetSummonMultiple
{
    const int MAXPETS = 3;
    static Dictionary<Player, ConcurrentQueue<Pet>> playerPets { get; set; } = new();

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Pet), nameof(Pet.HandleCurrentActivePet), new Type[] { typeof(Player) })]
    //public static bool PreHandleCurrentActivePet(Player player, ref Pet __instance, ref bool? __result)
    //{        
    //    //True if you have no active pet
    //    __result = true;

    //    //Return true to execute original
    //    return false;
    //}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Pet), nameof(Pet.Init), new Type[] { typeof(Player), typeof(PetDevice) })]
    public static bool PreInit(Player player, PetDevice petDevice, ref Pet __instance, ref bool? __result)
    {
        //Skip original handling of active pet
        //var result = __instance.HandleCurrentActivePet(player);
        //if (result == null || !result.Value)
        //{
        //    __result = result;
        //    return false;
        //}

        // get physics radius of player and pet
        var playerRadius = player.PhysicsObj.GetPhysicsRadius();
        var petRadius = __instance.GetPetRadius();

        var spawnDist = playerRadius + petRadius + Pet.MinDistance;

        if (__instance.IsPassivePet)
        {
            __instance.Location = player.Location.InFrontOf(spawnDist, true);
            __instance.TimeToRot = -1;
        }
        else
        {
            __instance.Location = player.Location.InFrontOf(spawnDist, false);
        }

        __instance.Location = __instance.Location.SetLandblockId(new LandblockId(__instance.Location.GetCell()));

        __instance.Name = player.Name + "'s " + __instance.Name;

        __instance.PetOwner = player.Guid.Full;
        __instance.P_PetOwner = player;

        // All pets don't leave corpses, this maybe should have been in data, but isn't so lets make sure its true.
        __instance.NoCorpse = true;

        Debugger.Break();
        var success = __instance.EnterWorld();

        if (!success)
        {
            player.SendTransientError($"Couldn't spawn {__instance.Name}");
            __result = false;
            return false;
        }


        //Assume permission to create
        //Find or create the pet queue
        if (!playerPets.TryGetValue(player, out var pets))
        {
            pets = new();
            playerPets.TryAdd(player, pets);
        }

        //Destroy pet if needed
        if (pets.Count >= MAXPETS)
        {
            if (pets.TryDequeue(out var pet))
            {
                player.SendMessage($"Your {pet.Name} has been sent to a peaceful farm.");
                pet?.Destroy();
            }
        }
        //Also add pet to queue
        pets.Enqueue(__instance);
        player.SendMessage($"Summoned {pets.Count}th pet");

        player.CurrentActivePet = __instance;

        petDevice.Pet = __instance.Guid.Full;
        __instance.PetDevice = petDevice.Guid.Full;
        __instance.P_PetDevice = petDevice;

        if (__instance.IsPassivePet)
            __instance.nextSlowTickTime = Time.GetUnixTime();

        __result = true;

        return false;
    }

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Pet), nameof(Pet.HandleCurrentActivePet), new Type[] { typeof(Player) })]
    //public static bool PreHandleCurrentActivePet(Player player, ref Pet __instance, ref bool? __result)
    //{
    //    __result = false;

    //    if (player is null)
    //        return false;

    //    //Find or create the pet queue
    //    if(!playerPets.TryGetValue(player, out var pets))
    //    {
    //        pets = new ();
    //        playerPets.TryAdd(player, pets);
    //    }
    //    if (pets.Count >= MAXPETS)
    //    {
    //        if (pets.TryDequeue(out var pet))
    //        {
    //            player.SendMessage($"Your {pet.Name} has been sent to a peaceful farm.");
    //            pet?.Destroy();
    //        }
    //    }
    //    player.CurrentActivePet = __instance;


    //    __result = true;

    //    //Return true to execute original
    //    return false;
    //}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PetDevice), nameof(PetDevice.CheckUseRequirements), new Type[] { typeof(WorldObject) })]
    public static bool PreCheckUseRequirements(WorldObject activator, ref PetDevice __instance, ref ActivationResult __result)
    {
        //Override use requirements
        if (!(activator is Player player))
        {
            __result = new ActivationResult(false);
            return false;
        }

        //Check base requirements
        var asWorldObject = __instance as WorldObject;
        
        if (asWorldObject is null || !asWorldObject.CheckUseRequirements(activator).Success)
        {
            __result = new ActivationResult(false);
            return false;
        }

        // verify summoning mastery
        if (__instance.SummoningMastery != null && player.SummoningMastery != __instance.SummoningMastery)
        {
            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"You must be a {__instance.SummoningMastery} to use the {__instance.Name}", ChatMessageType.Broadcast));
            {
                __result = new ActivationResult(false);
                return false;
            }
        }

        // duplicating some of this verification logic here from Pet.Init()
        // since the PetDevice owner and the summoned Pet are separate objects w/ potentially different heartbeat offsets,
        // the cooldown can still expire before the CombatPet's lifespan
        // in this case, if the player tries to re-activate the PetDevice while the CombatPet is still in the world,
        // we want to return an error without re-activating the cooldown

        //Skip check
        //if (player.CurrentActivePet != null && player.CurrentActivePet is CombatPet)
        //{
        //    if (PropertyManager.GetBool("pet_stow_replace").Item)
        //    {
        //        // original ace
        //        player.SendTransientError($"{player.CurrentActivePet.Name} is already active");
        //        {
        //            __result = new ActivationResult(false);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        // retail stow
        //        var weenie = DatabaseManager.World.GetCachedWeenie((uint)__instance.PetClass);

        //        if (weenie == null || weenie.WeenieType != WeenieType.Pet)
        //        {
        //            player.SendTransientError($"{player.CurrentActivePet.Name} is already active");
        //            {
        //                __result = new ActivationResult(false);
        //                return false;
        //            }
        //        }
        //    }
        //}
        __result = new ActivationResult(true);

        return false;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.LogOut_Final), new Type[] { typeof(bool) })]
    public static void PreLogOut_Final(bool skipAnimations, ref Player __instance)
    {
        //Destroy pets on logout
        if (!playerPets.TryGetValue(__instance, out var pets))
            return;

        foreach (var pet in pets)
            pet?.Destroy();

        pets = null;
        playerPets.Remove(__instance);
    }
}