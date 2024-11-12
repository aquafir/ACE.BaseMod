namespace Expansion.Mutators;

[HarmonyPatchCategory(nameof(Mutation.RandomColor))]  //Mutator categories not used, just convenience for generating enum or a placeholder
internal class RandomColor : Mutator
{
    public override bool TryMutateEnterInventory(HashSet<Mutation> mutations, WorldObject item, Player player) =>
        LootGenerationFactory.RandomizeColorTotallyRandom(item) is not null;
}
