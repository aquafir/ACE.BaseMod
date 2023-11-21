﻿
namespace CustomLoot.Mutators;

public class Slayer : Mutator
{
    CreatureType[] species;

    public override bool Mutates(TreasureDeath profile, TreasureRoll roll, HashSet<Mutation> mutations, WorldObject item = null)
    {
        //Doesn't mutate Slayers
        if (item.GetProperty(PropertyInt.SlayerCreatureType) is not null)
            return false;

        return base.Mutates(profile, roll, mutations, item);
    }

    public override bool TryMutate(TreasureDeath profile, TreasureRoll roll, HashSet<Mutation> mutations, WorldObject item)
    {
        //Try to get a random type
        if (!species.TryGetRandom(out var type))
            return false;

        var power = PatchClass.Settings.SlayerPower[profile.Tier];

        item.SetProperty(PropertyInt.SlayerCreatureType, (int)type);
        item.SetProperty(PropertyFloat.SlayerDamageBonus, power);

        return true;
    }

    /// <summary>
    /// Example of doing some setup when the Mutator is created
    /// </summary>
    public override void Start()
    {
        //Use all creatures or just a subset
        var cTypes = PatchClass.Settings.UseCustomSlayers ? PatchClass.Settings.SlayerSpecies : Enum.GetValues<CreatureType>();
        //Construct bag without bad types
        species = cTypes.Where(x => x != CreatureType.Invalid && x != CreatureType.Unknown && x != CreatureType.Wall).ToArray();

        if (PatchClass.Settings.Verbose)
            ModManager.Log($"Set up bag of {species.Length} species to add Slayer from.");
    }
}