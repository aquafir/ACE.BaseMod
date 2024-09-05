//namespace Expansion.Features;

//[CommandCategory(nameof(Feature.OverrideSpellProjectiles))]
//[HarmonyPatchCategory(nameof(Feature.OverrideSpellProjectiles))]
//internal class OverrideSpellProjectiles
//{
//    [HarmonyPrefix]
//    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CreateSpellProjectiles), new Type[] { typeof(Spell), typeof(WorldObject), typeof(WorldObject), typeof(bool), typeof(bool), typeof(uint) })]
//    public static bool PreCreateSpellProjectiles(Spell spell, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, uint lifeProjectileDamage, ref WorldObject __instance, ref List<SpellProjectile> __result)
//    {
//        if (spell.NumProjectiles == 0 || __instance is not Player player)
//            return true;    //Could skip original execution / set __result -- nothing is done with the returned list of SpellProjectile


//        var spellType = SpellProjectile.GetProjectileSpellType(spell.Id);

//        __result = __instance.PositionSpellProjectiles(spell, spellType, target, weapon, isWeaponSpell, fromProc, lifeProjectileDamage);
//        //__result = __instance.LaunchSpellProjectiles(__result, spellType, spell, target, weapon, isWeaponSpell, fromProc, lifeProjectileDamage);

//        //__result = __instance.LaunchSpellProjectiles(spell, spellType, target, spellType, weapon, isWeaponSpell, fromProc, origins, velocity, lifeProjectileDamage);

//        return false;
//    }

//    //Support changes to the creation and launching of spell projectiles
//    //[HarmonyPrefix]
//    //[HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CreateSpellProjectiles), new Type[] { typeof(Spell), typeof(WorldObject), typeof(WorldObject), typeof(bool), typeof(bool), typeof(uint) })]
//    //public static bool PreCreateSpellProjectiles(Spell spell, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, uint lifeProjectileDamage, ref WorldObject __instance, ref List<SpellProjectile> __result)
//    //{
//    //    if (spell.NumProjectiles == 0)
//    //        return true;    //Could skip original execution / set __result -- nothing is done with the returned list of SpellProjectile

//    //    __result = __instance.PositionSpellProjectiles(spell, target, weapon, isWeaponSpell, fromProc, lifeProjectileDamage);

//    //    return false;
//    //}

//    //[HarmonyPrefix]
//    //[HarmonyPatch(typeof(WorldObject), nameof(WorldObject.LaunchSpellProjectiles), new Type[] { typeof(Spell), typeof(WorldObject), typeof(ProjectileSpellType), typeof(WorldObject), typeof(bool), typeof(bool), typeof(List<Vector3>), typeof(Vector3), typeof(uint) })]
//    //public static bool PreLaunchSpellProjectiles(Spell spell, WorldObject target, ProjectileSpellType spellType, WorldObject weapon, bool isWeaponSpell, bool fromProc, List<Vector3> origins, Vector3 velocity, uint lifeProjectileDamage, ref WorldObject __instance, ref List<SpellProjectile> __result)
//    //{
//    //    __result = __instance.CreateSpellProjectiles(spell, target, spellType, weapon, isWeaponSpell, fromProc, origins, velocity, lifeProjectileDamage);
//    //    return false;
//    //}

//}


