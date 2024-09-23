namespace Expansion.Features;

[CommandCategory(nameof(Feature.ProcOnHit))]
[HarmonyPatchCategory(nameof(Feature.ProcOnHit))]
internal class ProcOnHit
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Cloak), nameof(Cloak.TryProcSpell), new Type[] { typeof(Creature), typeof(WorldObject), typeof(WorldObject), typeof(float) })]
    public static bool PreTryProcSpell(Creature defender, WorldObject attacker, WorldObject cloak, float damage_percent, ref Cloak __instance, ref bool __result)
    {
        //Override to skip cloak check
        __result = false;


        if (defender is Player wielder)
        {
            //Get proccing non-cloaks
            var equipped = wielder.EquippedObjects.Values.Where(i => i.HasProc && !Aetheria.IsAetheria(i.WeenieClassId) && !Cloak.IsCloak(i));
            var count = equipped.Count();

            foreach (var c in equipped)
            {
                if (!Cloak.RollProc(c, damage_percent))
                    continue;

                if (Cloak.HandleProcSpell(defender, attacker, c))
                    __result = true;
            }
        }

        //Override
        return false;
    }
}
