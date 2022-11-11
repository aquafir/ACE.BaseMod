using ACE.Common;
using ACE.DatLoader.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity;
using ACE.Server.Factories;
using ACE.Server.Managers;
using System;
using System.Xml.Linq;

namespace Respawn
{
    public static class SpawnHelper
    {
        static Dictionary<Landblock, LinkedList<Creature>> landblockCreatures = new();

        //Exposes LandblockManager's list of landblock groups
        private static readonly List<LandblockGroup> _landblockGroups = Traverse.Create(typeof(LandblockManager)).Field<List<LandblockGroup>>("landblockGroups").Value;

        //Cached exposure of Landblock's players
        private static readonly Dictionary<Landblock, List<Player>> _landblockPlayers = new();
        /// <summary>
        /// Get a list of players in a landblock.
        /// </summary>
        public static List<Player> GetPlayers(this Landblock landblock)
        {
            if (!_landblockPlayers.TryGetValue(landblock, out var players))
            {
                //Landblock.sortedGeneratorsByNextGeneratorUpdate
                players = Traverse.Create(landblock).Field<List<Player>>("players").Value;
                _landblockPlayers.Add(landblock, players);
            }

            return players;
        }

        //Cached exposure of Landblock's sortedCreaturesByNextTick
        private static readonly Dictionary<Landblock, LinkedList<Creature>> _landblockCreatures = new();
        /// <summary>
        /// Get a list of creatures in a landblock.
        /// </summary>
        public static LinkedList<Creature> GetCreatures(this Landblock landblock)
        {
            if (!_landblockCreatures.TryGetValue(landblock, out var creatures))
            {
                //Landblock.sortedGeneratorsByNextGeneratorUpdate
                creatures = Traverse.Create(landblock).Field<LinkedList<Creature>>("sortedCreaturesByNextTick").Value;

                _landblockCreatures.Add(landblock, creatures);
            }

            return creatures;
        }

        public static int GetCreatureCount(this Landblock landblock)
        {
            return landblock.GetCreatureProfiles().Sum(x => x.CurrentCreate);
        }

        //Cached exposure of generators for a landblock
        private static readonly Dictionary<Landblock, LinkedList<WorldObject>> _landblockGenerators = new();
        /// <summary>
        /// Get a list of generator WorldObjects in a landblock
        /// </summary>
        public static LinkedList<WorldObject> GetGenerators(this Landblock landblock)
        {
            if (!_landblockGenerators.TryGetValue(landblock, out var generators))
            {
                //Landblock.sortedGeneratorsByNextGeneratorUpdate
                generators = Traverse.Create(landblock).Field<LinkedList<WorldObject>>("sortedGeneratorsByNextGeneratorUpdate").Value;

                _landblockGenerators.Add(landblock, generators);
            }

            return generators;
        }

        //Cached exposure of creature generators for a landblock
        private static readonly Dictionary<Landblock, List<GeneratorProfile>> _landblockCreatureGenerators = new();
        /// <summary>
        /// Filter a list of generators in a landblock to the ones that spawn creatures to specific locations
        /// </summary>
        public static List<GeneratorProfile> GetCreatureProfiles(this Landblock landblock)
        {
            if (!_landblockCreatureGenerators.TryGetValue(landblock, out var profiles))
            {
                profiles = new();

                var gens = landblock.GetGenerators();

                //Narrow to GeneratorActiveProfiles indices?
                var creatureGenerators = gens.Where(gen => PatchClass.Settings.CREATURE_GENERATOR_IDS.Contains(gen.WeenieClassId));

                foreach (var gen in creatureGenerators)
                {
                    profiles.AddRange(gen.GeneratorProfiles.Where(p => p.RegenLocationType == RegenLocationType.Specific));
                }

                var inactive = gens.Where(gen => PatchClass.Settings.CREATURE_GENERATOR_IDS.Contains(gen.WeenieClassId));

                _landblockCreatureGenerators.Add(landblock, profiles);
            }

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
        public static void GenerateLandblockCreatureCounts(bool verbose = true)
        {
            var sb = new StringBuilder($"\r\nFinding creature counts of {_landblockGroups.Count} landblock groups:");

            //Todo: Could look into making this multi-threaded
            //if (ConfigManager.Config.Server.Threading.MultiThreadedLandblockGroupPhysicsTicking)

            int groupCount = 1;
            foreach (var landblockGroup in _landblockGroups)
            {
                if (verbose)
                    sb.AppendLine($"  Group {groupCount++} ({landblockGroup.Count}, {(landblockGroup.IsDungeon ? "Dungeon" : "Map")}):");
                foreach (var landblock in landblockGroup)
                {
                    //Todo: think about inactive/unloaded landblocks? 
                    //if (landblock.IsDormant)
                    //    continue;

                    //Get generators for the landblock
                    var generators = landblock.GetGenerators();
                    var creatureGenerators = landblock.GetCreatureProfiles();
                    var creatures = landblock.GetCreatures().Count;
                    var players = landblock.GetPlayers().Count;
                    var max = landblock.GetMaxSpawns();

                    if (verbose && players > 0 && creatures > 0)    //Might want to see creature/playerless LBs?
                        sb.AppendLine($"    Landblock {landblock.Id.LandblockX}x {landblock.Id.LandblockY}y:\r\n" +
                            $"      {generators.Count} generators\r\n" +
                            $"      {creatureGenerators.Count()} creature generators\r\n" +
                            $"      {creatures} / {max} creatures\r\n" +
                            $"      {players} players");


                    //Creature count stays null if none found
                    CreatureCount[landblock.Id.LandblockX, landblock.Id.LandblockY] = creatures < 1 ? null : creatures;
                }
            }

            if (verbose)
                ModManager.Log(sb.ToString());
        }

        /// <summary>
        /// Respawn creatures in a landblock
        /// </summary>
        /// <param name="landblock"></param>
        /// <param name="skipMaxed">Respawn all, including</param>
        public static void RespawnCreatures(this Landblock landblock)
        {
            //Wipes all generated things (including non-creatures) and adds them back a little later
            //foreach (var gen in landblock.Generators)
            //    gen.ResetGenerator();

            //Respawn creatures
            foreach (var profile in landblock.GetCreatureProfiles())
            {
                //Skip
                if (!PatchClass.Settings.ExceedMax && profile.IsMaxed)
                    continue;

                //Overspawn?
                if (PatchClass.Settings.ExceedMax)
                {
                    profile.Spawn();
                }
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
}
