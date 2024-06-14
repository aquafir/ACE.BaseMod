namespace Expansion.Features;

[CommandCategory(nameof(Feature.ProcOnAttack))]
[HarmonyPatchCategory(nameof(Feature.ProcOnAttack))]
internal class ProcOnAttack
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.TryProcEquippedItems), new Type[] { typeof(WorldObject), typeof(Creature), typeof(bool), typeof(WorldObject) })]
    public static bool PostTryProcEquippedItems(WorldObject attacker, Creature target, bool selfTarget, WorldObject weapon, ref WorldObject __instance)
    {
        // handle procs directly on this item -- ie. phials
        // this could also be monsters with the proc spell directly on the creature
        if (__instance.HasProc && __instance.ProcSpellSelfTargeted == selfTarget)
        {
            // projectile
            // monster
            __instance.TryProcItem(attacker, target, selfTarget);
        }

        // handle proc spells for weapon
        // this could be a melee weapon, or a missile launcher
        if (weapon != null && weapon.HasProc && weapon.ProcSpellSelfTargeted == selfTarget)
        {
            // weapon
            weapon.TryProcItem(attacker, target, selfTarget);
        }

        if (attacker != __instance && attacker.HasProc && attacker.ProcSpellSelfTargeted == selfTarget)
        {
            // handle special case -- missile projectiles from monsters w/ a proc directly on the mob
            // monster
            attacker.TryProcItem(attacker, target, selfTarget);
        }

        // handle aetheria procs
        if (attacker is Player wielder)
        {
            var equipped = wielder.EquippedObjects.Values.Where(i => i.HasProc && i.CloakWeaveProc != 1 && i.ProcSpellSelfTargeted == selfTarget && i != weapon);

            foreach (var item in equipped)
                item.TryProcItem(attacker, target, selfTarget);
        }

        return false;
    }

    //Terse, but less efficient way
    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(WorldObject), nameof(WorldObject.TryProcEquippedItems), new Type[] { typeof(WorldObject), typeof(Creature), typeof(bool), typeof(WorldObject) })]
    //public static void TitaniumWeinerPostTryProcEquippedItems(WorldObject attacker, Creature target, bool selfTarget, WorldObject weapon, ref WorldObject __instance)
    //{
    //    // handle non-aetheria procs
    //    if (attacker is Player wielder)
    //    {
    //        //Aetheria and weapons already handled
    //        var equipped = wielder.EquippedObjects.Values.Where(i => i.HasProc && i.CloakWeaveProc != 1 && i.ProcSpellSelfTargeted == selfTarget && i != weapon);
    //        foreach (var equip in equipped)
    //            equip.TryProcItem(attacker, target, selfTarget);
    //    }
    //}
}
