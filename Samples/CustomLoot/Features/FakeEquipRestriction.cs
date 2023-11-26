using ACE.Server.Command;
using ACE.Server.Managers;
using ACE.Server.Network;

namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakeEquipRestriction))]
internal class FakeEquipRestriction
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.CheckWieldRequirements), new Type[] { typeof(WorldObject) })]
    public static bool PreCheckWieldRequirements(WorldObject item, ref Player __instance, ref WeenieError __result)
    {
        Debugger.Break();
        if (!__instance.CurrentLandblock.IsDungeon)
            return true;

        //var cs = item.GetProperty(FakeBool.CorpseSpawnedDungeon);
        //if (!(item.GetProperty(FakeBool.CorpseSpawnedDungeon) ?? false))
        //    return true;

        var lb = item.GetProperty(FakeDID.LocationLockId) ?? 0;
        if(lb != __instance.CurrentLandblock.Id.Raw) {
            __instance.SendMessage($"Unable to wield item that spawned in a different dungeon.\nFound: {lb:X4}\nIn: {__instance.CurrentLandblock.Id.Raw:X4}");

            //What message to use?
            __result = WeenieError.YouDoNotOwnThatItem; 
            return false;
        }
        __instance.SendMessage($"Able to wield item that spawned in a matching dungeon.\nFound: {lb:X4}\nIn: {__instance.CurrentLandblock.Id.Raw:X4}");

        //Do regular checks
        return true;
    }

    [CommandHandler("t1", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld, 0)]
    public static void T(Session session, params string[] parameters)
    {
        var player = session.Player;

        

        player.SendMessage(player.CurrentLandblock.IsDungeon.ToString());
    }


    //    [HarmonyPrefix]
    //    [HarmonyPatch(typeof(Creature), nameof(Creature.AddItemToEquippedItemsRatingCache), new Type[] { typeof(WorldObject) })]
    //    public static void PreAddItemToEquippedItemsRatingCache(WorldObject wo, ref Creature __instance)
    //    {
    //        if (__instance is Player player)
    //        {
    //            player.UpdateEquipmentCache();
    //            //UpdateItem(player, wo, true);

    //        }
    //    }

    //    [HarmonyPrefix]
    //    [HarmonyPatch(typeof(Creature), nameof(Creature.RemoveItemFromEquippedItemsRatingCache), new Type[] { typeof(WorldObject) })]
    //    public static void PreRemoveItemFromEquippedItemsRatingCache(WorldObject wo, ref Creature __instance)
    //    {
    //        if (__instance is Player player)
    //        {
    //            player.UpdateEquipmentCache();
    //            //UpdateItem(player, wo, true);
    //#if DEBUG
    //            player.SendMessage($"\nUnequipped {wo.Name}:\n{wo.DumpItem()}");
    //#endif
    //        }
    //    }

}
