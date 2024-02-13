namespace Expansion.Features;

[HarmonyPatchCategory(nameof(Feature.FakeSpellChain))]
internal class FakeSpellChain
{
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(WorldObject), nameof(Player.HandleCastSpell), new Type[] {
    //    typeof(Spell), typeof(WorldObject),typeof(WorldObject),typeof(WorldObject), typeof(bool), typeof(bool), typeof(bool)})]
    //public static void HandleCastSpell(Spell spell, WorldObject target, WorldObject itemCaster = null, WorldObject weapon = null, bool isWeaponSpell = false, bool fromProc = false, bool equip = false, WorldObject __instance = null)
    //{
    //    //Shouldn't be needed?
    //    //if (__instance is not Player player)
    //    //    return;

    //    //if (!spell.IsProjectile)
    //    //    return;


    //}

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SpellProjectile), nameof(SpellProjectile.OnCollideObject), new Type[] { typeof(WorldObject) })]
    public static void PostOnCollideObject(WorldObject target, ref SpellProjectile __instance)
    {
        if (__instance.ProjectileSource is not Player player)
            return;

        var chance = __instance.GetProperty(FakeFloat.SpellChainChance) ?? 0;
        //Debugger.Break();
        if (chance > 0 && ThreadSafeRandom.Next(0f, 1.0f) < chance)
        {
            var t = player.GetSplashTargets(target, 2, 100).Skip(1).FirstOrDefault();


            if (t is null)
                player.SendMessage($"Your {__instance.Spell.Name} wants to chain after hitting {target.Name} with {chance:P2} odds but failed.");
            else
            {
                //Halve chance
                __instance.SetProperty(FakeFloat.SpellChainChance, chance / 2);

                player.SendMessage($"Chaining {__instance.Spell.Name} to {target.Name} after hitting {target.Name} with {chance:P2}.");

                //__instance.TryCastSpell_WithRedirects
                //target.TryCastSpell_WithRedirects(new Spell(__instance.Spell.Id), t);
                player.TryCastSpell_WithRedirects(new Spell(__instance.Spell.Id), t, target, target);

            }

            //Halve chance
            //__instance.SetProperty(FakeFloat.SpellChainChance, chance / 2);
        }

    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CreateSpellProjectiles), new Type[] { typeof(Spell), typeof(WorldObject), typeof(WorldObject), typeof(bool), typeof(bool), typeof(uint) })]
    public static void PostCreateSpellProjectiles(Spell spell, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, uint lifeProjectileDamage, ref WorldObject __instance, ref List<SpellProjectile> __result)
    {
        //When launching see if the player casting chains
        if (__instance is not Player player)
            return;

        //Only tracking spells?
        if (spell.NonTracking)
            return;

        //Pass on chain chance to each projectile
        var chainChance = player.GetCachedFake(FakeFloat.SpellChainChance);
        if (chainChance > 0)
        {
            foreach (var projectile in __result)
                projectile.SetProperty(FakeFloat.SpellChainChance, chainChance);

            player.SendMessage($"Set {__result.Count} {spell.Name} projectiles to {chainChance:P2} chance of chaining.");
        }
    }
}
