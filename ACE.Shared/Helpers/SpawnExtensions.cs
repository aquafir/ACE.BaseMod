namespace ACE.Shared.Helpers;

public static class SpawnExtensions
{
    /// <summary>
    /// Get a list of players in a landblock.
    /// </summary>
    public static List<Player> GetPlayers(this Landblock landblock) => landblock.players;

    //Cached exposure of Landblock's sortedCreaturesByNextTick
    private static readonly Dictionary<Landblock, LinkedList<Creature>> _landblockCreatures = new();
    /// <summary>
    /// Get a list of creatures in a landblock.
    /// </summary>
    public static LinkedList<Creature> GetCreatures(this Landblock landblock) => landblock.sortedCreaturesByNextTick;

    public static int GetCreatureCount(this Landblock landblock) => landblock.GetCreatureProfiles().Sum(x => x.CurrentCreate);

    /// <summary>
    /// Get a list of generator WorldObjects in a landblock
    /// </summary>
    public static LinkedList<WorldObject> GetGenerators(this Landblock landblock) => landblock.sortedGeneratorsByNextGeneratorUpdate;

    //List of creature generator IDs?
    static readonly HashSet<uint> CREATURE_GENERATOR_IDS = new HashSet<uint> { 1154, 3951, 3953, 3955, 4219, 5086, 5485, 7923, 7924, 7925, 7926, 7932, 15274, 21120, 24129, 28282 };
    //WeenieClassId == 3666 is for Placeholder generators, but that isn't in the list of IDs so it doesn't matter

    /// <summary>
    /// Filter a list of generators in a landblock to the ones that spawn creatures to specific locations
    /// </summary>
    public static List<GeneratorProfile> GetCreatureProfiles(this Landblock landblock, RegenLocationType type = RegenLocationType.Specific | RegenLocationType.Undef | RegenLocationType.Scatter | RegenLocationType.OnTop)
    {
        List<GeneratorProfile> profiles = new();

        var gens = landblock.GetGenerators();

        //Find generators based on hard-coded creature gen ID list?
        //Todo: think about this
        var creatureGenerators = gens.Where(x => CREATURE_GENERATOR_IDS.Contains(x.WeenieClassId));

        //Find GeneratorProfiles with specified spawn types
        foreach (var gen in creatureGenerators)
            profiles.AddRange(gen.GeneratorProfiles.Where(p => p.RegenLocationType.HasAny(type)));

        return profiles;
    }

    //Todo: decide on using 255/255 arrays vs Dictionaries for caching
    public static int?[,] CreatureCount { get; set; } = new int?[255, 255];    // Count for each landblock
    /// <summary>
    /// Gets the total count of creatures spawned by static(?) creature generators
    /// </summary>
    public static int GetMaxSpawns(this Landblock landblock)
    {
        var count = CreatureCount[landblock.Id.LandblockX, landblock.Id.LandblockY];

        if (count is null)
        {
            var creatureProfiles = landblock.GetCreatureProfiles();

            /// A generator can have multiple profiles / spawn multiple types of objects
            /// Each generator profile can in turn spawn multiple objects (init_create / max_create)
            count = creatureProfiles.Sum(p => p.MaxCreate > 0 ? p.MaxCreate : 0);

            //count = creatureGenerators.Sum(x => x.MaxCreate);
            CreatureCount[landblock.Id.LandblockX, landblock.Id.LandblockY] = count;
        }

        return count.Value;
    }

    /// <summary>
    /// Get the percentage of creatures still living in a landblock
    /// </summary>
    public static double PercentAlive(this Landblock landblock)
    {
        var current = landblock.GetCreatures().Count;
        var max = landblock.GetMaxSpawns();

        return max == 0 ? 100 : (double)current / max;
    }

    /// <summary>
    /// Cache the max static creatures spawned in a landblock
    /// </summary>
    public static void GenerateLandblockCreatureCounts(this Landblock landblock)
    {
        var sb = new StringBuilder($"\r\nCreatures:");

        //Get generators for the landblock
        var generators = landblock.GetGenerators();
        var creatureGenerators = landblock.GetCreatureProfiles();
        var creatures = landblock.GetCreatures().Count;
        var players = landblock.GetPlayers().Count;
        var max = landblock.GetMaxSpawns();

        if (players > 0 && creatures > 0)    //Might want to see creature/playerless LBs?
            sb.AppendLine($"    Landblock {landblock.Id.LandblockX}x {landblock.Id.LandblockY}y:\r\n" +
                $"      {generators.Count} generators\r\n" +
                $"      {creatureGenerators.Count()} creature generators\r\n" +
                $"      {creatures} / {max} creatures\r\n" +
                $"      {players} players");


        //Creature count stays null if none found
        CreatureCount[landblock.Id.LandblockX, landblock.Id.LandblockY] = creatures < 1 ? null : creatures;

        ModManager.Log(sb.ToString());
    }

    /// <summary>
    /// Respawn creatures in a landblock
    /// </summary>
    /// <param name="landblock"></param>
    /// <param name="skipMaxed">Respawn all, including</param>
    public static void RespawnCreatures(this Landblock landblock, bool ExceedMax = false)
    {
        //Wipes all generated things (including non-creatures) and adds them back a little later
        //foreach (var gen in landblock.Generators)
        //    gen.ResetGenerator();

        //Respawn creatures
        foreach (var profile in landblock.GetCreatureProfiles())
        {
            //Skip
            if (!ExceedMax && profile.IsMaxed)
                continue;

            //Overspawn?
            if (ExceedMax)
                profile.Spawn();
            else
            {
                //Add immediate spawns for each missing and trigger
                for (var i = profile.CurrentCreate; i < profile.MaxCreate; i++)
                {
                    profile.SpawnQueue.Add(DateTime.MinValue);
                }
                profile.ProcessQueue();
            }

            //Todo: rethink this?  Existing queued items will overspawn
            profile.SpawnQueue.Clear();
        }
    }
}
