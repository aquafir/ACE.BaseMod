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

public static class PositionExtension
{
    public static ACE.Entity.Position Shifted(this ACE.Entity.Position pos, float dx, float dy, float dz)
    {
        return new ACE.Entity.Position(pos.LandblockId.Raw, pos.PositionX + dx, pos.PositionY + dy, pos.PositionZ + dz, 0f, 0f, 0f, 0f);
        //float qw = RotationW; // north
        //float qz = RotationZ; // south

        //double x = 2 * qw * qz;
        //double y = 1 - 2 * qz * qz;

        //var heading = Math.Atan2(x, y);
        //var dx = -1 * Convert.ToSingle(Math.Sin(heading) * distanceInFront);
        //var dy = Convert.ToSingle(Math.Cos(heading) * distanceInFront);

        //// move the Z slightly up and let gravity pull it down.  just makes things easier.
        //var bumpHeight = 0.05f;
        //if (rotate180)
        //{
        //    var rotate = new Quaternion(0, 0, qz, qw) * Quaternion.CreateFromYawPitchRoll(0, 0, (float)Math.PI);
        //    return new Position(LandblockId.Raw, PositionX + dx, PositionY + dy, PositionZ + bumpHeight, 0f, 0f, rotate.Z, rotate.W);
        //}
        //else
        //    return new Position(LandblockId.Raw, PositionX + dx, PositionY + dy, PositionZ + bumpHeight, 0f, 0f, qz, qw);
    }

}