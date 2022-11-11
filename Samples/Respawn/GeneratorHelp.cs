using ACE.Common;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Respawn
{
    public static class GeneratorHelper
    {

        /////// <summary>
        /////// Requires adding a property to expose one of the lists of creatures in Landblock.cs.
        /////// </summary>
        /////// <param name="lb"></param>
        /////// <returns>Percent of the max spawned creatures in the landblock that are alive.</returns>
        ////public static double GetPercentAlive(this Landblock lb)
        ////{
        ////    return (double)lb.Creatures.Count / lb.GetMaxSpawns();
        ////}

        ////public static void RespawnCreatures(this Landblock lb)
        ////{
        ////    //Wipes all generated things (including non-creatures) and adds them back a little later
        ////    //foreach (var gen in lb.Generators)
        ////    //    gen.ResetGenerator();

        ////    //Respawn creatures
        ////    foreach (var gen in lb.Generators.Where(x => MONSTER_GENERATOR_IDS.Contains(x.WeenieClassId)))
        ////    {
        ////        foreach (var profile in gen.GeneratorProfiles)
        ////        {
        ////            //var unspawned = profile.MaxCreate - profile.CurrentCreate;
        ////            //If creatures still exist another copy of them will be made but that wouldn't matter much with a low clear percent required to respawn?
        ////            profile.Spawn();
        ////            profile.SpawnQueue.Clear();
        ////        }
        ////    }
        ////}

        //public static string GeneratorDump(WorldObject generator)
        //{
        //    var msg = new StringBuilder();
        //    msg.Append($"Generator Dump for {generator.Name} (0x{generator.Guid.ToString()})\n");
        //    msg.Append($"Generator WCID: {generator.WeenieClassId}\n");
        //    msg.Append($"Generator WeenieClassName: {generator.WeenieClassName}\n");
        //    msg.Append($"Generator WeenieType: {generator.WeenieType.ToString()}\n");
        //    msg.Append($"Generator Status: {(generator.GeneratorDisabled ? "Disabled" : "Enabled")}\n");
        //    msg.Append($"GeneratorType: {generator.GeneratorType.ToString()}\n");
        //    msg.Append($"GeneratorTimeType: {generator.GeneratorTimeType.ToString()}\n");

        //    if (generator.GeneratorTimeType == GeneratorTimeType.Event)
        //        msg.Append($"GeneratorEvent: {(!string.IsNullOrWhiteSpace(generator.GeneratorEvent) ? generator.GeneratorEvent : "Undef")}\n");

        //    if (generator.GeneratorTimeType == GeneratorTimeType.RealTime)
        //    {
        //        msg.Append($"GeneratorStartTime: {generator.GeneratorStartTime} ({Time.GetDateTimeFromTimestamp(generator.GeneratorStartTime).ToLocalTime()})\n");
        //        msg.Append($"GeneratorEndTime: {generator.GeneratorEndTime} ({Time.GetDateTimeFromTimestamp(generator.GeneratorEndTime).ToLocalTime()})\n");
        //    }
        //    msg.Append($"GeneratorEndDestructionType: {generator.GeneratorEndDestructionType.ToString()}\n");
        //    msg.Append($"GeneratorDestructionType: {generator.GeneratorDestructionType.ToString()}\n");
        //    msg.Append($"GeneratorRadius: {generator.GetProperty(PropertyFloat.GeneratorRadius) ?? 0f}\n");
        //    msg.Append($"InitGeneratedObjects: {generator.InitGeneratedObjects}\n");
        //    msg.Append($"MaxGeneratedObjects: {generator.MaxGeneratedObjects}\n");
        //    msg.Append($"GeneratorInitialDelay: {generator.GeneratorInitialDelay}\n");
        //    msg.Append($"RegenerationInterval: {generator.RegenerationInterval}\n");
        //    msg.Append($"GeneratorUpdateTimestamp: {generator.GeneratorUpdateTimestamp} ({Time.GetDateTimeFromTimestamp(generator.GeneratorUpdateTimestamp).ToLocalTime()})\n");
        //    msg.Append($"NextGeneratorUpdateTime: {generator.NextGeneratorUpdateTime} ({((generator.NextGeneratorUpdateTime == double.MaxValue) ? "Disabled" : Time.GetDateTimeFromTimestamp(generator.NextGeneratorUpdateTime).ToLocalTime().ToString())})\n");
        //    msg.Append($"RegenerationTimestamp: {generator.RegenerationTimestamp} ({Time.GetDateTimeFromTimestamp(generator.RegenerationTimestamp).ToLocalTime()})\n");
        //    msg.Append($"NextGeneratorRegenerationTime: {generator.NextGeneratorRegenerationTime} ({((generator.NextGeneratorRegenerationTime == double.MaxValue) ? "On Demand" : Time.GetDateTimeFromTimestamp(generator.NextGeneratorRegenerationTime).ToLocalTime().ToString())})\n");

        //    msg.Append($"GeneratorProfiles.Count: {generator.GeneratorProfiles.Count}\n");
        //    msg.Append($"GeneratorActiveProfiles.Count: {generator.GeneratorActiveProfiles.Count}\n");
        //    msg.Append($"CurrentCreate: {generator.CurrentCreate}\n");
        //    msg.Append($"===============================================\n");

        //    foreach (var activeProfile in generator.GeneratorActiveProfiles)
        //    {
        //        var profile = generator.GeneratorProfiles[activeProfile];

        //        msg.Append($"Active GeneratorProfile id: {activeProfile} | LinkId: {profile.LinkId}\n");
        //        msg.Append($"Probability: {profile.Biota.Probability} | WCID: {profile.Biota.WeenieClassId} | Delay: {profile.Biota.Delay} | Init: {profile.Biota.InitCreate} | Max: {profile.Biota.MaxCreate}\n");
        //        msg.Append($"WhenCreate: {((RegenerationType)profile.Biota.WhenCreate).ToString()} | WhereCreate: {((RegenLocationType)profile.Biota.WhereCreate).ToString()}\n");
        //        msg.Append($"StackSize: {profile.Biota.StackSize} | PaletteId: {profile.Biota.PaletteId} | Shade: {profile.Biota.Shade}\n");
        //        msg.Append($"CurrentCreate: {profile.CurrentCreate} | Spawned.Count: {profile.Spawned.Count} | SpawnQueue.Count: {profile.SpawnQueue.Count} | RemoveQueue.Count: {profile.RemoveQueue.Count}\n");
        //        msg.Append($"GeneratedTreasureItem: {profile.GeneratedTreasureItem}\n");
        //        msg.Append($"--====--\n");

        //        if (profile.Spawned.Count > 0)
        //        {
        //            msg.Append("Spawned Objects:\n");

        //            foreach (var spawn in profile.Spawned.Values)
        //            {
        //                msg.Append($"0x{spawn.Guid}: {spawn.Name} - {spawn.WeenieClassId} - {spawn.WeenieType}\n");

        //                var spawnWO = spawn.TryGetWorldObject();
        //                if (spawnWO != null)
        //                {
        //                    if (spawnWO.Location != null)
        //                        msg.Append($" LOC: {spawnWO.Location.ToLOCString()}\n");

        //                    else if (spawnWO.ContainerId == generator.Guid.Full)
        //                        msg.Append($" Contained by Generator\n");

        //                    else if (spawnWO.WielderId == generator.Guid.Full)
        //                        msg.Append($" Wielded by Generator\n");

        //                    else
        //                        msg.Append($" Location Unknown\n");

        //                }
        //                else
        //                    msg.Append($" LOC: Unknown, WorldObject could not be found\n");

        //            }
        //            msg.Append($"--====--\n");

        //        }

        //        if (profile.SpawnQueue.Count > 0)
        //        {
        //            msg.Append("Pending Spawn Times:\n");

        //            foreach (var spawn in profile.SpawnQueue)
        //            {
        //                msg.Append($"{spawn.ToLocalTime()}\n");

        //            }
        //            msg.Append($"--====--\n");
        //        }

        //        if (profile.RemoveQueue.Count > 0)
        //        {
        //            msg.Append("Pending Removed Objects:\n");

        //            foreach (var spawn in profile.RemoveQueue)
        //            {
        //                var action = "";
        //                switch ((RegenerationType)profile.Biota.WhenCreate)
        //                {
        //                    case RegenerationType.Death:
        //                        action = "died";
        //                        break;
        //                    case RegenerationType.Destruction:
        //                        action = "destroyed";
        //                        break;
        //                    case RegenerationType.PickUp:
        //                        action = "picked up";
        //                        break;
        //                    case RegenerationType.Undef:
        //                    default:
        //                        action = "despawned";
        //                        break;
        //                }

        //                msg.Append($"0x{spawn.objectGuid:X8} {action} at {spawn.time.AddSeconds(-profile.Delay).ToLocalTime()} and will be removed from profile at {spawn.time.ToLocalTime()}\n");

        //            }
        //            msg.Append($"--====--\n");

        //        }
        //        msg.Append($"===============================================\n");

        //    }
        //    return msg.ToString();
        //}
    }
}
