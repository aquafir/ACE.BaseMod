namespace Expansion.Features;

[CommandCategory(nameof(Feature.UnarmedWeapon))]
[HarmonyPatchCategory(nameof(Feature.UnarmedWeapon))]
public class UnarmedWeapon
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetWeapon), new Type[] { typeof(Creature), typeof(bool) })]
    public static void PostGetWeapon(Creature wielder, bool forceMainHand, ref WorldObject __instance, ref WorldObject __result)
    {
        if (__result is not null || wielder is not Player player)
            return;

        //var unarmedSlot = player.GetPowerAccuracyBar() >= Player.KickThreshold ?
        var unarmedSlot = player.PowerLevel >= Player.KickThreshold ?
            player.EquippedObjects.Values.FirstOrDefault(e => e.CurrentWieldedLocation?.HasFlag(EquipMask.FootWear) ?? false) :
            player.EquippedObjects.Values.FirstOrDefault(e => e.CurrentWieldedLocation?.HasFlag(EquipMask.HandWear) ?? false);

        __result = unarmedSlot;
        //        player.SendMessage($"Unarmed weapon is {unarmedSlot?.Name}");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.TryProcEquippedItems), new Type[] { typeof(WorldObject), typeof(Creature), typeof(bool), typeof(WorldObject) })]
    public static void PostTryProcEquippedItems(WorldObject attacker, Creature target, bool selfTarget, WorldObject weapon, ref WorldObject __instance)
    {
        if (attacker is not Player player)
            return;

        if (WorldObject.GetWeapon(player) is not WorldObject altWeapon)
            return;

        altWeapon.TryProcItem(attacker, target, selfTarget);
    }

    //Ended up having issues
    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(Creature), nameof(Creature.GetEquippedMeleeWeapon), new Type[] { typeof(bool) })]
    //public static void PostGetEquippedMeleeWeapon(bool forceMainHand, ref Creature __instance, ref WorldObject __result)
    //{
    //    if (__result is not null || __instance is not Player player)
    //        return;
    //    //player.GetPowerAccuracyBar() will cause infinite loop
    //    var unarmedSlot = player.PowerLevel >= Player.KickThreshold ?
    //        player.EquippedObjects.Values.FirstOrDefault(e => e.CurrentWieldedLocation?.HasFlag(EquipMask.FootWear) ?? false) :
    //        player.EquippedObjects.Values.FirstOrDefault(e => e.CurrentWieldedLocation?.HasFlag(EquipMask.HandWear) ?? false);

    //    __result = unarmedSlot;
    //    player.SendMessage($"Unarmed weapon is {unarmedSlot?.Name}");
    //}
}