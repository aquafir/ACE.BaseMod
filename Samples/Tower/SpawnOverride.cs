using ACE.DatLoader.Entity;
using static ACE.Server.Physics.Common.LandDefs;

namespace Tower;

[HarmonyPatch]
internal class SpawnOverride
{
    private const int RETRIES = 10;
    static Random gen = new Random();

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(GeneratorProfile), nameof(GeneratorProfile.Spawn_Default), new Type[] { typeof(WorldObject) })]
    //public static bool PreSpawn_Default(WorldObject obj, ref GeneratorProfile __instance, ref bool __result)
    //{
    //    if(obj)
    //    //Return false to override
    //    //return false;

    //    //Return true to execute original
    //    return true;
    //}


    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(GeneratorProfile), nameof(GeneratorProfile.Spawn_Default), new Type[] { typeof(WorldObject) })]
    //public static void PostSpawn_Default(WorldObject obj, ref GeneratorProfile __instance, ref bool __result)
    //{
    //    //Check after for fail?
    //    if (__result)
    //        return;

    //    //Restrict to range you're using
    //    if (obj.WeenieClassId < 90000)
    //        return;
        
    //    for(var i = 0; i < RETRIES; i++)
    //    {
    //        obj.Location = obj.Location.Shifted(gen.Next(10), gen.Next(10), 0);

    //        if (obj.EnterWorld())
    //            return;
    //    }
    //}
}