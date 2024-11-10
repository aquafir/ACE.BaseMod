namespace QualityOfLife;

[HarmonyPatchCategory(nameof(Features.Tailoring))]
public class Tailoring
{
    //Rewrites Tailoring handling
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ACE.Server.Entity.Tailoring), nameof(ACE.Server.Entity.Tailoring.VerifyUseRequirements), new Type[] { typeof(Player), typeof(WorldObject), typeof(WorldObject) })]
    public static bool PreVerifyUseRequirements(Player player, WorldObject source, WorldObject target, ref ACE.Server.Entity.Tailoring __instance, ref WeenieError __result)
    {
        if (source == target)
            __result = WeenieError.YouDoNotPassCraftingRequirements;

        // ensure both source and target are in player's inventory
#if REALM
        else if (player.FindObject(source.Guid, Player.SearchLocations.MyInventory) == null)
            __result = WeenieError.YouDoNotPassCraftingRequirements;

        else if (player.FindObject(target.Guid, Player.SearchLocations.MyInventory) == null)
            __result = WeenieError.YouDoNotPassCraftingRequirements;
#else
        else if (player.FindObject(source.Guid.Full, Player.SearchLocations.MyInventory) == null)
            __result = WeenieError.YouDoNotPassCraftingRequirements;

        else if (player.FindObject(target.Guid.Full, Player.SearchLocations.MyInventory) == null)
            __result = WeenieError.YouDoNotPassCraftingRequirements;
#endif

        // verify not retained item
        else if (target.Retained)
        {
            player.Session.Network.EnqueueSend(new GameMessageSystemChat("You must use Sandstone Salvage to remove the retained property before tailoring.", ChatMessageType.Craft));
            __result = WeenieError.YouDoNotPassCraftingRequirements;
        }

        //skip society armor check
        // verify not society armor
        //if (source.IsSocietyArmor || target.IsSocietyArmor) { 
        //__result = WeenieError.YouDoNotPassCraftingRequirements;

        else
            __result = WeenieError.None;

        //skip original
        return false;
    }
}

//public class TailoringSettings
//{
//}