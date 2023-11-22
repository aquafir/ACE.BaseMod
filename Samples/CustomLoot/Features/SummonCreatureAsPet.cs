using ACE.Database;
using ACE.Server.Managers;
using System.Runtime.CompilerServices;

namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.SummonCreatureAsPet))]
public static class SummonCreatureAsPet
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PetDevice), nameof(PetDevice.SummonCreature), new Type[] { typeof(Player), typeof(uint) })]
    public static bool PreSummonCreature(Player player, uint wcid, ref PetDevice __instance, ref bool? __result)
    {
        var guid = GuidManager.NewDynamicGuid();

        var weenie = DatabaseManager.World.GetCachedWeenie(wcid);

        if (weenie == null)
        {
            __result = null;
            return false;
        }
        //var worldObject = CreateWorldObject(weenie, guid);
        //var wo = new CombatPet(weenie, guid);
        var wo = new Pet(weenie, guid);

        //Could wipe others?         /* PhysicsState - Ethereal, ReportCollisions, Gravity */
        wo.SetPhysicsState(PhysicsState.Ethereal | PhysicsState.ReportCollisions | PhysicsState.Gravity, true);
        /* TargetingTactic - Nearest */
        wo.TargetingTactic = TargetingTactic.Nearest;

        if (wo == null)
            GuidManager.RecycleDynamicGuid(guid);

        __result = true;
        if (wo == null)
        {
            //            log.Error($"{player.Name}.SummonCreature({wcid}) - couldn't find wcid for PetDevice {WeenieClassId} - {WeenieClassName}");
            return false;
        }

        //var pet = wo as Pet;

        //     if (pet == null)
        //     {
        ////         log.Error($"{player.Name}.SummonCreature({wcid}) - PetDevice {WeenieClassId} - {WeenieClassName} tried to summon {wo.WeenieClassId} - {wo.WeenieClassName} of unknown type {wo.WeenieType}");
        //         return false;
        //     }
        __result = wo.Init(player, __instance);

        if (__result != true) wo.Destroy();

        return false;
    }

    //public static int 
}
