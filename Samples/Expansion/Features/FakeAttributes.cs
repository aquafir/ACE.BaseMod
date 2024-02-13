//using ACE.Server.WorldObjects.Entity;

//namespace Expansion.Features;

//[HarmonyPatchCategory(nameof(Feature.FakeAttributes))]
//internal class FakeAttributes
//{

//    [HarmonyPostfix]
//    [HarmonyPatch(typeof(Creature), nameof(Creature.Strength), MethodType.Getter)]
//    public static void PostGetStrength(ref Creature __instance, ref CreatureAttribute __result)
//    {
////        Debugger.Break();
//        //Your code here
//    }

//    [HarmonyPostfix]
//    [HarmonyPatch(typeof(CreatureAttribute), nameof(CreatureAttribute.GetCurrent), new Type[] { typeof(bool) })]
//    public static void PostGetCurrent(bool enchanted, ref CreatureAttribute __instance, ref uint __result)
//    {
//        if (__instance.creature is not Player player)
//            return;

//        var mod = __instance.Attribute switch
//        {
//            PropertyAttribute.Undef => 0,
//            PropertyAttribute.Strength => player.GetCachedFake(FakeInt.ItemStrengthMod),
//            PropertyAttribute.Endurance => player.GetCachedFake(FakeInt.ItemEnduranceMod),
//            PropertyAttribute.Quickness => player.GetCachedFake(FakeInt.ItemQuicknessMod),
//            PropertyAttribute.Coordination => player.GetCachedFake(FakeInt.ItemCoordinationMod),
//            PropertyAttribute.Focus => player.GetCachedFake(FakeInt.ItemFocusMod),
//            PropertyAttribute.Self => player.GetCachedFake(FakeInt.ItemSelfMod),
//            _ => 0,
//        };
//        __result += (uint)mod;
//        //Debugger.Break();
//    }

//}
