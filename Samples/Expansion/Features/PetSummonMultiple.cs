using Expansion.Features;
using System.Collections.Concurrent;

namespace Expansion.Features;

[CommandCategory(nameof(Feature.PetSummonMultiple))]
[HarmonyPatchCategory(nameof(Feature.PetSummonMultiple))]
public static class PetSummonMultiple
{
    const double MAXPETS = 2;
    static Dictionary<Player, ConcurrentQueue<Pet>> playerPets { get; set; } = new();

    static decimal MaxPetWeight(this Player player) =>
        Convert.ToDecimal(player.GetProperty(FakeFloat.PetWeightMax) ?? 1)
        + Convert.ToDecimal(player.GetCachedFake(FakeFloat.PetWeightMax));
    static decimal PetWeight(this Pet pet) => Convert.ToDecimal(pet.GetProperty(FakeFloat.PetWeight) ?? 1);

    /// <summary>
    /// Sums the weight of living pets and removes null or dead pets
    /// </summary>
    static decimal TotalPetWeight(this ConcurrentQueue<Pet> pets)
    {
        decimal total = 0;

        for (var i = 0; i < pets.Count; i++)
        {
            if (!pets.TryDequeue(out var pet) || pet is null)
                continue;

            if (pet.IsAlive)
            {
                total += pet.PetWeight();
                pets.Enqueue(pet);
            }
            else
                pet.Destroy();
        }

        return total;
        //return pets.Where(x => x is not null && x.IsAlive).Sum(x => x.PetWeight());
    }
    //static double TotalPetWeight(this Player player) => playerPets.Count;

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


        //Check if pet could be summoned
        var weight = __instance.PetWeight();
        var max = player.MaxPetWeight();
        if (weight > max)
        {
            __result = false;
            return false;
        }

        //Try to create
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

#if REALM
        __instance.Location = __instance.Location.SetLandblockId(new LandblockId(__instance.Location.GetCell()));
#else
#endif

        __instance.Name = player.Name + "'s " + __instance.Name;

        __instance.PetOwner = player.Guid.Full;
        __instance.P_PetOwner = player;

        // All pets don't leave corpses, this maybe should have been in data, but isn't so lets make sure its true.
        __instance.NoCorpse = true;

        var success = __instance.EnterWorld();

        if (!success)
        {
            player.SendTransientError($"Couldn't spawn {__instance.Name}");
            __result = false;
            return false;
        }


        //If the pet was created, remove excess pets and add the new one to the collection of known player pets
        //Find or create the pet queue
        if (!playerPets.TryGetValue(player, out var pets))
        {
            pets = new();
            playerPets.TryAdd(player, pets);
        }
        player.SendMessage($"Trying to add {__instance.Name} ({weight:F2}) to current weight of {pets.TotalPetWeight():F2}/{max:F2}");

        //Destroy pet if needed
        var excessWeight = pets.TotalPetWeight() + weight - max;
        var removedWeight = 0m;

        if (excessWeight > 0)
        {
            //player.SendMessage($"");
            var msg = new StringBuilder($"\nRemoving {excessWeight:F2} excess weight:");
            while (removedWeight < excessWeight)
            {
                if (pets.TryDequeue(out var removedPet))
                {
                    //Ignore dead pets
                    if (removedPet is null)
                        continue;

                    removedWeight += removedPet.PetWeight();
                    msg.Append($"\n  {removedPet.Name}  ({removedPet.PetWeight():F2})");
                    removedPet.Destroy();
                }
                //Skip problem pets
                //else
                //{
                //    ModManager.Log($"Failed to dequeue pet to spawn {__instance.Name}", ModManager.LogLevel.Error);
                //    __result = false;
                //    return false;
                //}
            }

            //Double check for problems occuring?
            //if (removedWeight < excessWeight)
            //{
            //    msg.Append($"\nCulled {pets.Count} misbehaving pets...");
            //    foreach (var pet in pets)
            //        pet?.Destroy();

            //    pets.Clear();
            //}
            //else
            msg.Append($"\n  ={excessWeight:F2} excess - {removedWeight:F2} removed -> {pets.TotalPetWeight() + weight:F2}/{max:F2} current weight");

            player.SendMessage($"{msg}");
        }

        //Also add pet to queue
        pets.Enqueue(__instance);
        player.SendMessage($"Summoned pet #{pets.Count}");

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
        __result = __instance.CheckUseRequirements();
        if (!__result.Success)
            return false;

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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.Teleport), new Type[] { typeof(Position), typeof(bool), typeof(bool) })]
    public static void PostTeleport(Position newPosition, bool teleportingFromInstance, bool fromPortal, ref Player __instance)
    {
        //Destroy pets on teleport
        if (!playerPets.TryGetValue(__instance, out var pets))
            return;

        foreach (var pet in pets)
            pet?.Destroy();
    }


    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(WorldObject), nameof(WorldObject.Destroy), new Type[] { typeof(bool), typeof(bool) })]
    //public static bool PreDestroy(bool raiseNotifyOfDestructionEvent, bool fromLandblockUnload, ref WorldObject __instance)
    //{
    //    if (__instance is not Pet pet)
    //        return true;

    //    //Override destroy behavior without redundant check
    //    if (__instance.IsDestroyed)
    //        return false;

    //    __instance.IsDestroyed = true;
    //    __instance.ReleasedTimestamp = Time.GetUnixTime();


    //    //if (__instance is Creature creature)
    //    //{
    //    //    foreach (var item in creature.EquippedObjects.Values)
    //    //        item.Destroy();
    //    //}

    //    if (pet.P_PetOwner?.CurrentActivePet == __instance)
    //        pet.P_PetOwner.CurrentActivePet = null;

    //    if (pet.P_PetDevice?.Pet == __instance.Guid.Full)
    //        pet.P_PetDevice.Pet = null;

    //    if (raiseNotifyOfDestructionEvent)
    //        __instance.NotifyOfEvent(RegenerationType.Destruction);

    //    __instance.CurrentLandblock?.RemoveWorldObject(__instance.Guid);
    //    __instance.RemoveBiotaFromDatabase();

    //    if (__instance.Guid.IsDynamic())
    //        GuidManager.RecycleDynamicGuid(__instance.Guid);

    //    if (playerPets.TryGetValue(pet.P_PetOwner, out var pets))
    //    {
    //        //Todo: find a non-terrible way to do this?
    //        var livingPets = pets.Where(x => x.Guid != pet.Guid);
    //        playerPets[pet.P_PetOwner] = new(livingPets);

    //        pet.P_PetOwner?.SendMessage($"{pet.Name} shuffled off this mortal coil.");
    //        pet = null;
    //    }


    //    return false;
    //}

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.Destroy), new Type[] { typeof(bool), typeof(bool) })]
    public static void PostDestroy(bool raiseNotifyOfDestructionEvent, bool fromLandblockUnload, ref WorldObject __instance)
    {
        if (__instance is not Pet pet)
            return;

        pet.Health.Current = 0;

        //Recreate?  Terrible way of doing this
        //if (playerPets.TryGetValue(pet.P_PetOwner, out var pets))
        //{
        //    pets.get
        //    //Todo: find a non-terrible way to do this?
        //    var livingPets = pets.Where(x => x.Guid != pet.Guid);
        //    playerPets[pet.P_PetOwner] = new(livingPets);

        //    pet.P_PetOwner?.SendMessage($"{pet.Name} shuffled off this mortal coil.");
        //    pet = null;
        //}
    }



    #region Commands
    static string Commands => string.Join(", ", Enum.GetNames<PetCommand>());
    static readonly string[] USAGES = new string[] {
            $@"(?<verb>{PetCommand.List})$",
            $@"(?<verb>{PetCommand.Kill})$",
            $@"(?<verb>{PetCommand.Warp})$",
        };
    //Join usages in a regex pattern
    static string Pattern => string.Join("|", USAGES.Select(x => $"({x})"));
    static string Desc => $"pet {string.Join("|", Enum.GetNames<PetCommand>())}";
    static Regex CommandRegex = new(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    [CommandHandler("pet", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void Sim(Session session, params string[] parameters)
    {
        var player = session.Player;

        if (!TryParseCommand(parameters, out var verb))
        {
            player.SendMessage(Desc);
            return;
        }
        if (!playerPets.TryGetValue(player, out var pets) || pets.Count == 0)
        {
            player.SendMessage($"No pets found.");
            return;
        }

        var sb = new StringBuilder("\n");
        switch (verb)
        {
            case PetCommand.List:
                sb.Append($"\nPets ({pets.TotalPetWeight():F2}/{player.MaxPetWeight()}):");
                foreach (var pet in pets)
                {
                    if (pet.IsAlive)
                        sb.Append($"\n{pet.Name} ({pet.PetWeight():F2})");
                }

                break;
            case PetCommand.Kill:
                sb.Append($"\nDestroyed:");
                foreach (var pet in pets)
                {
                    sb.Append($"\n{pet.Name} ({pet.PetWeight():F2})");
                    pet?.Destroy();
                }
                pets.Clear();
                break;
            case PetCommand.Warp:
                foreach (var pet in pets)
                {
                    if (player.GetCylinderDistance(pet) > 5)
                    {
                        //Todo: fix teleport
#if REALM
                        if(pet.TryTeleport(player.Location.InFrontOf(1), out var result))
                            player.SendMessage($"{pet.Name} has caught up to you.");
#else
#endif
                    }
                }
                break;
        }
        player.SendMessage($"{sb}");
    }


    static bool TryParseCommand(string[] parameters, out PetCommand verb)
    {
        //Set defaults
        verb = 0;

        //Check for valid command
        var match = CommandRegex.Match(string.Join(" ", parameters));
        if (!match.Success)
            return false;

        //Parse verb
        return Enum.TryParse(match.Groups["verb"].Value, true, out verb);
    }

    enum PetCommand
    {
        List,
        Kill,
        //Come,
        Warp,
    }
    #endregion
}
