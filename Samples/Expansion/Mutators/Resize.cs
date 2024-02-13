namespace Expansion.Mutators;

public class Resize : Mutator
{
    //static readonly Dictionary<uint, float> objectScale = new();
    static float goalHeight = 1.8f;
    static readonly Dictionary<uint, float> objectScale = new();

    public override bool TryMutateEnterWorld(HashSet<Mutation> mutations, WorldObject item)
    {
        if (item is not Creature creature)
            return false;

        if (creature.Location is null || !creature.Location.Indoors)
            return false;

        if (!objectScale.TryGetValue(creature.WeenieClassId, out var scale))
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

    //public override bool TryMutateFactory(HashSet<Mutation> mutations, WorldObject item)
    //{
    //    if (item is not Creature creature)
    //        return false;

    //    if(!objectScale.TryGetValue(creature.WeenieClassId, out var scale))
    //    {
    //        //Calculate scale
    //        if (creature.Height == 0)
    //            creature.InitPhysicsObj();  //Wasn't calculating height

    //        scale = goalHeight / creature.Height;
    //        objectScale.TryAdd(creature.WeenieClassId, scale);
    //    }
    //    creature.ObjScale *= scale;

    //    return true;
    //}
}