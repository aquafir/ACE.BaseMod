
namespace Expansion.Mutators;

internal class RandomColor : Mutator
{
    public override bool TryMutateEnterInventory(HashSet<Mutation> mutations, WorldObject item) => 
        LootGenerationFactory.RandomizeColorTotallyRandom(item) is not null;
}
