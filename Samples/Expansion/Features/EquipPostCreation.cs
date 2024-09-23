using ACE.Server.Network.Handlers;
using ACE.Server.Network.Enum;

namespace Expansion.Features;

[CommandCategory(nameof(Feature.EquipPostCreation))]
[HarmonyPatchCategory(nameof(Feature.EquipPostCreation))]
internal class EquipPostCreation
{
    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(PlayerFactory), nameof(PlayerFactory.Create), new Type[] { typeof(CharacterCreateInfo), typeof(Weenie), typeof(ObjectGuid), typeof(uint), typeof(WeenieType), typeof(Player) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
    //public static void PostCreate(CharacterCreateInfo characterCreateInfo, Weenie weenie, ObjectGuid guid, uint accountId, WeenieType weenieType, Player player, ref CreateResult __result)
    //{
    //    if (__result != CreateResult.Success || player is null)
    //        return;

    //    Debugger.Break();
    //    try
    //    {
    //        foreach (var equipment in player.Inventory.Values.Where(x => x.ValidLocations != null))
    //            player.TryEquipObject(equipment, equipment.ValidLocations ?? 0);

    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //}


    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterHandler), nameof(CharacterHandler.SendCharacterCreateResponse), new Type[] { typeof(Session), typeof(CharacterGenerationVerificationResponse), typeof(ObjectGuid), typeof(string) })]
    public static void PostSendCharacterCreateResponse(Session session, CharacterGenerationVerificationResponse response, ObjectGuid guid, string charName)
    {
        if (response != CharacterGenerationVerificationResponse.Ok)
            return;

        var action = new ActionChain();
        action.AddDelaySeconds(10);
        action.AddAction(WorldManager.DelayManager, () =>
        {
            var player = PlayerManager.GetOnlinePlayer(guid);
            if (player is null)
                return;

            foreach (var equipment in player.Inventory.Values.Where(x => x.ValidLocations != null))
                player.TryEquipObjectWithNetworking(equipment, equipment.ValidLocations ?? 0);
        });
        action.EnqueueChain();
    }


    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(ShardDatabase), nameof(ShardDatabase.AddCharacterInParallel), new Type[] { typeof(Biota), typeof(ReaderWriterLockSlim), typeof(IEnumerable<(Biota biota, ReaderWriterLockSlim rwLock)>), typeof(Character), typeof(ReaderWriterLockSlim) })]
    //public static void PostAddCharacterInParallel(Biota biota, ReaderWriterLockSlim biotaLock, IEnumerable<(Biota biota, ReaderWriterLockSlim rwLock)> possessions, Character character, ReaderWriterLockSlim characterLock, ref ShardDatabase __instance, ref bool __result)
    //{
    //    //Skip failures
    //    if (!__result)
    //        return;        
    //}



}
