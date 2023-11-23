using ACE.Entity;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;

namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakeReducedBurden))]
internal class FakeReducedBurden
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Container), nameof(Container.TryAddToInventory), new Type[] { typeof(WorldObject), typeof(Container), typeof(int), typeof(bool), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
    public static void PostTryAddToInventory(WorldObject worldObject, Container container, int placementPosition, bool limitToMainPackOnly, bool burdenCheck, ref Container __instance, ref bool __result)
    {
        //Skip if this isn't a player or unable to add
        if (__instance is not Player player || !__result)
            return;

        //Item added to container has weight offset
        var reduction = container.GetProperty(FakeFloat.ReducedBurden);
        if (reduction is null)
            return;

        var change = (int)(worldObject.EncumbranceVal * reduction);
        player.EncumbranceVal -= change;
        player.SendMessage($"{worldObject.Name} had weight reduced by {change}, {reduction:P2} of {worldObject.EncumbranceVal}");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Container), nameof(Container.TryRemoveFromInventory), new Type[] { typeof(ObjectGuid), typeof(WorldObject), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal })]
    public static void PostTryRemoveFromInventory(ObjectGuid objectGuid, WorldObject item, bool forceSave, ref Container __instance, ref bool __result)
    {
        //Skip if this isn't a player or unable to add
        if (__instance is not Player player || !__result)
            return;

        //Item added to container has weight offset
        var reduction = container.GetProperty(FakeFloat.ReducedBurden);
        if (reduction is null)
            return;

        var change = (int)(worldObject.EncumbranceVal * reduction);
        player.EncumbranceVal -= change;
        player.SendMessage($"{worldObject.Name} had weight reduced by {change}, {reduction:P2} of {worldObject.EncumbranceVal}");
    }

}
