//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ACE.Database.Models.Auth;
//using ACE.Entity.Enum;
//using ACE.Server.Entity;
//using ACE.Server.Network.Enum;
//using Mono.Cecil;

//namespace Expansion.Features;

//[CommandCategory(nameof(Feature.FakeDurability))]
//[HarmonyPatchCategory(nameof(Feature.FakeDurability))]
//public static class FakeDurability
//{
//    [HarmonyPrefix]
//    [HarmonyPatch(typeof(Player), nameof(Player.TakeDamage), new Type[] { typeof(WorldObject), typeof(DamageType), typeof(float), typeof(BodyPart), typeof(bool), typeof(AttackConditions) })]
//    public static bool PreTakeDamage(WorldObject source, DamageType damageType, float _amount, BodyPart bodyPart, bool crit, AttackConditions attackConditions, ref Player __instance, ref int __result)
//    {
//        //BodyPart are a smaller enum
//        //EquipMask can have more equip on the same body part and include untargetables like ammo/weapon/cloak

//        //var coverage = BodyParts.GetCoverageMask(bodyPart);
//        //var damageLocation = bodyPart
//        //var equip = bodyPart switch
//        //{
//        //    BodyPart.Head => __instance.head,
//        //    BodyPart.Chest => throw new NotImplementedException(),
//        //    BodyPart.Abdomen => throw new NotImplementedException(),
//        //    BodyPart.UpperArm => throw new NotImplementedException(),
//        //    BodyPart.LowerArm => throw new NotImplementedException(),
//        //    BodyPart.Hand => throw new NotImplementedException(),
//        //    BodyPart.UpperLeg => throw new NotImplementedException(),
//        //    BodyPart.LowerLeg => throw new NotImplementedException(),
//        //    BodyPart.Foot => throw new NotImplementedException(),
//        //    _ => throw new NotImplementedException(),
//        //};

//        //foreach (var item in __instance.Inventory.Values)
//        //{
//        //    item.
//        //    if((item.CurrentWieldedLocation & BodyParts.GetCoverageMask(bodyPart)) > 0)
//        //    if (item.CurrentWieldedLocation != bodyPart.GetCoverageMap)
//        //}
//        //Return false to override
//        //return false;

//        //Return true to execute original
//        return true;
//    }


////    public static List<WorldObject> GetEquippedAtBodyPart(this BodyPart part, Player player)
////    {
////        List<WorldObject> result = new ();
////        var e = player.EquippedObjects.Values;

////            result = part switch
////            {
////                BodyPart.Head => e.Where(x => (x.CurrentWieldedLocation & (EquipMask.HeadWear)) > 0),
////                BodyPart.Chest => throw new NotImplementedException(),
////                BodyPart.Abdomen => throw new NotImplementedException(),
////                BodyPart.UpperArm => throw new NotImplementedException(),
////                BodyPart.LowerArm => throw new NotImplementedException(),
////                BodyPart.Hand => throw new NotImplementedException(),
////                BodyPart.UpperLeg => throw new NotImplementedException(),
////                BodyPart.LowerLeg => throw new NotImplementedException(),
////                BodyPart.Foot => throw new NotImplementedException(),
////            };


////        var damageLocation = (DamageLocation)iDamageLocation;
////    }
////}

////Todo: think about this and caching
//[Flags]
//enum DurabilityType
//{
//    Hit,
//    Damage,
//    Spell,
//    Crit,
//}
//enum DurabilityBreakType
//{
//    Damaged,
//    Destroyed,
//}