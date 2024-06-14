namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeSpellReflection))]
[HarmonyPatchCategory(nameof(Feature.FakeSpellReflection))]
internal class FakeSpellReflection
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpellProjectile), nameof(SpellProjectile.OnCollideObject), new Type[] { typeof(WorldObject) })]
    public static bool PreOnCollideObject(WorldObject target, ref SpellProjectile __instance)
    {
        if (target is not Player player)
            return true;

        if (__instance.ProjectileSource is not Creature creature)
            return true;

        var reflectChance = player.GetCachedFake(FakeFloat.ItemReflectSpellProjectileChance);
        if (reflectChance > 0 && ThreadSafeRandom.Next(0f, 1.0f) < reflectChance)
        {
            var reflectedSpell = new Spell(__instance.Spell.Id);
            player.TryCastSpell_WithRedirects(reflectedSpell, creature);
            player.SendMessage($"You reflected projectile {reflectedSpell.Name} with {reflectChance:0.0} chance at {creature.Name}");

            return false;
        }

        //Return true to execute original
        return true;
    }


    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Creature), nameof(Creature.CastSpell), new Type[] { typeof(Spell) })]
    //public static bool PreCastSpell(Spell spell, ref Creature __instance)
    //{
    //    Debugger.Break();

    //    //Return true to execute original
    //    return true;
    //}



    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.TryResistSpell), new Type[] { typeof(WorldObject), typeof(Spell), typeof(WorldObject), typeof(bool) })]
    public static bool PreTryResistSpell(WorldObject target, Spell spell, WorldObject itemCaster, bool projectileHit, ref WorldObject __instance, ref bool __result)
    {
        if (target is not Player player)
            return true;

        //Might want to check for source of item caster?
        if (__instance is not Creature creature)
            return true;

        //Limit by type
        if (spell.IsProjectile)
            return true;

        var reflectChance = player.GetCachedFake(FakeFloat.ItemReflectSpellChance);
        if (reflectChance > 0 && ThreadSafeRandom.Next(0f, 1.0f) < reflectChance)
        {
            var reflectedSpell = new Spell(spell.Id);
            player.TryCastSpell_WithRedirects(reflectedSpell, creature);
            player.SendMessage($"You reflected {reflectedSpell.Name} with {reflectChance:0.0} chance at {creature.Name}");
            __result = true;
            return false;
        }
        //Return true to execute original
        return true;
    }

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Creature), nameof(Creature.CastSpell), new Type[] { typeof(Spell) })]
    //public static bool PreCastSpell(Spell spell, ref Creature __instance)
    //{
    //    if(spell.)
    //    //Return false to override
    //    //return false;

    //    //Return true to execute original
    //    return true;
    //}

}
