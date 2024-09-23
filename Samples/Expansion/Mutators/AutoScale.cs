namespace Expansion.Mutators;

#if REALM

#else
[HarmonyPatchCategory(nameof(Mutation.AutoScale))]  //Mutator categories not used, just convenience for generating enum or a placeholder
public class AutoScale : Mutator
{
    static readonly Dictionary<string, Position> nearestTown = new();
    const float TIER_INTERVAL = 500;
    static readonly PropertyAttribute[] attributesToScale = Enum.GetValues<PropertyAttribute>().ToArray();

    public override bool TryMutateEnterWorld(HashSet<Mutation> mutations, WorldObject item)
    {
        if (item is not Creature creature)
            return false;

        if (creature.Location is null) return false;

        //Debugger.Break();
        //Get nearest poi?
        var nearest = nearestTown.OrderBy(x => x.Value.SquaredDistanceTo(creature.Location)).First();
        var distance = nearest.Value.Distance2D(creature.Location);

        int tier = (int)(distance / TIER_INTERVAL);
        creature.XpOverride *= (int)Math.Pow(2, tier);
        creature.ScaleAttributeBase(1 + .5f * tier, attributesToScale);
        creature.ObjScale *= tier / 3;
        creature.SetMaxVitals();
        creature.Name = $"Tier {tier} {creature.Name}";

        PlayerManager.BroadcastToAuditChannel(null, $"{creature.Name} ({tier}) spawn {distance} from {nearest.Key} ");

        return true;
    }

    public override void Start()
    {
        //Get pois
        DatabaseManager.World.CacheAllPointsOfInterest();
        var pois = DatabaseManager.World.GetPointsOfInterestCache();

        foreach (var poi in pois)
        {
            var teleportPOI = DatabaseManager.World.GetCachedPointOfInterest(poi.Key);
            if (teleportPOI == null)
                return;
            var weenie = DatabaseManager.World.GetCachedWeenie(teleportPOI.WeenieClassId);
            var portalDest = new Position(weenie.GetPosition(PositionType.Destination));
            WorldObject.AdjustDungeon(portalDest);
            nearestTown.Add(poi.Key, portalDest);
        }
    }
}
#endif
