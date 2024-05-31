using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects.Entity;
using ACE.Server.WorldObjects.Managers;

namespace Expansion.Features;

[HarmonyPatchCategory(nameof(Feature.FakeAttributes))]
internal class FakeAttributes
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreatureAttribute), nameof(CreatureAttribute.GetCurrent), new Type[] { typeof(bool) })]
    public static void PostGetCurrent(bool enchanted, ref CreatureAttribute __instance, ref uint __result)
    {
        if (__instance.creature is not Player player)
            return;

        var mod = __instance.Attribute switch
        {
            PropertyAttribute.Undef => 0,
            PropertyAttribute.Strength => player.GetCachedFake(FakeInt.ItemStrengthMod),
            PropertyAttribute.Endurance => player.GetCachedFake(FakeInt.ItemEnduranceMod),
            PropertyAttribute.Quickness => player.GetCachedFake(FakeInt.ItemQuicknessMod),
            PropertyAttribute.Coordination => player.GetCachedFake(FakeInt.ItemCoordinationMod),
            PropertyAttribute.Focus => player.GetCachedFake(FakeInt.ItemFocusMod),
            PropertyAttribute.Self => player.GetCachedFake(FakeInt.ItemSelfMod),
            _ => 0,
        };

        if (mod != 0)
        {
            __result += (uint)mod;
            //player.Session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(player, __instance));
            //player.SendMessage($"Updated {__instance} + {mod}");
        }

        //Debugger.Break();
    }


    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(EnchantmentManagerWithCaching), nameof(EnchantmentManagerWithCaching.GetAttributeMod_Multiplier), new Type[] { typeof(PropertyAttribute) })]
    //public static void PostGetAttributeMod_Multiplier(PropertyAttribute attribute, ref EnchantmentManagerWithCaching __instance, ref float __result)
    //{
    //    //Your code here
    //}


    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(EnchantmentManagerWithCaching), nameof(EnchantmentManagerWithCaching.GetAttributeMod_Additive), new Type[] { typeof(PropertyAttribute) })]
    //public static void PostGetAttributeMod_Additive(PropertyAttribute attribute, ref EnchantmentManagerWithCaching __instance, ref int __result)
    //{
    //    if (attribute == PropertyAttribute.Strength)
    //        __result += 10000;
    //}

}
