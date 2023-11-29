using CustomLoot.Helpers;

namespace CustomLoot.Mutators;

public class Resize : Mutator
{
    static readonly Dictionary<uint, float> objectScale = new();
    static float goalHeight = 1.8f;

    public override bool TryMutateFactory(HashSet<Mutation> mutations, WorldObject item)
    {
        if (item is not Creature creature)
            return false;

        if(!objectScale.TryGetValue(creature.WeenieClassId, out var scale))
        {
            //Calculate scale
            if (creature.Height == 0)
                creature.InitPhysicsObj();  //Wasn't calculating height

            scale = goalHeight / creature.Height;
            objectScale.TryAdd(creature.WeenieClassId, scale);
        }
        creature.ObjScale *= scale;

        return true;
    }
}