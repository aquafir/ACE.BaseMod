namespace Expansion.Mutators;

[HarmonyPatchCategory(nameof(Mutation.SampleMutator))]  //Mutator categories not used, just convenience for generating enum or a placeholder
internal class SampleMutator : Mutator
{
    public override bool TryMutateCorpse(HashSet<Mutation> mutations, Creature creature, DamageHistoryInfo killer, Corpse corpse, WorldObject item)
    {
        if (item is null) return false;

        item.Name += $" (C-{creature?.Name}-{killer?.Name})";
        return true;
    }
    public override bool TryMutateEmoteGive(HashSet<Mutation> mutations, WorldObject item, WorldObject giver, Player player)
    {
        if (item is null) return false;

        item.Name += $" (E-{giver?.Name}-{player?.Name})";
        return true;
    }
    public override bool TryMutateEnterInventory(HashSet<Mutation> mutations, WorldObject item, Player player)
    {
        if(item is null) return false;

        item.Name += $" (I-{player?.Name})";
        return true;
    }
    public override bool TryMutateEnterWorld(HashSet<Mutation> mutations, WorldObject item)
    {
        if (item is null) return false;

        item.Name += " (W)";
        return true;
    }
    public override bool TryMutateFactory(HashSet<Mutation> mutations, WorldObject item)
    {
        if (item is null) return false;

        item.Name += $" (F)";
        return true;
    }
    public override bool TryMutateGenerator(HashSet<Mutation> mutations, GeneratorProfile generator, WorldObject item)
    {
        if (item is null) return false;

        item.Name += $" (G)";
        return true;
    }
    public override bool TryMutateLoot(HashSet<Mutation> mutations, TreasureDeath profile, TreasureRoll roll, WorldObject item)
    {
        if (item is null) return false;

        item.Name += $" (L-{profile?.TreasureType})";
        return true;
    }
    public override bool TryMutateVendorBuy(HashSet<Mutation> mutations, WorldObject item, Vendor vendor, Player player)
    {
        if (item is null) return false;

        item.Name += $" (V-{vendor?.Name})";
        return true;
    }
}
