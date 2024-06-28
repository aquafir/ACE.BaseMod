namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeSpellChain))]
[HarmonyPatchCategory(nameof(Feature.FakeSpellChain))]
internal class FakeSpellChain
{
    //Relies on OverrideSpellProjectiles
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SpellProjectile), nameof(SpellProjectile.OnCollideObject), new Type[] { typeof(WorldObject) })]
    public static void PostOnCollideObject(WorldObject target, ref SpellProjectile __instance)
    {
        if (__instance.ProjectileSource is not Player player)
            return;

        var chainCount = __instance.GetProperty(FakeInt.SpellChainCount) ?? 0;
        chainCount++;
        if (chainCount > 5)
        {
            player.SendMessage($"Max chain count hit.");
            return;
        }

        var chance = __instance.GetProperty(FakeFloat.SpellChainChance) ?? 2;
        if (chance > 0 && ThreadSafeRandom.Next(0f, 1.0f) < chance)
        {
            //Todo: update splash
            var t = player.GetSplashTargets(target, TargetExclusionFilter.OnlyCreature).Take(1).FirstOrDefault();

            if (t is null)
                player.SendMessage($"Your {__instance.Spell.Name} wants to chain after hitting {target.Name} with {chance:P2} odds but failed.");
            else
            {
                //Create projectiles to a neighbor and increment chain count
                var projectiles = player.CreateSpellProjectiles(new Spell(__instance.Spell.Id), t, player, __instance.IsWeaponSpell, __instance.FromProc, __instance.LifeProjectileDamage);
                foreach (var projectile in projectiles)
                {
                    projectile.SetProperty(FakeInt.SpellChainCount, chainCount);

                    //Halve chance
                    projectile.SetProperty(FakeFloat.SpellChainChance, chance / 2);
                }

                player.SendMessage($"{__instance.Name} chained from {target.Name} to {t.Name} for the {chainCount}th time with {chance:P2}% chance");
            }
        }
    }






    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(SpellProjectile), nameof(SpellProjectile.OnCollideObject), new Type[] { typeof(WorldObject) })]
    //public static void PostOnCollideObject(WorldObject target, ref SpellProjectile __instance)
    //{
    //    if (__instance.ProjectileSource is not Player player)
    //        return;

    //    var chance = __instance.GetProperty(FakeFloat.SpellChainChance) ?? 0;
    //    //Debugger.Break();
    //    if (chance > 0 && ThreadSafeRandom.Next(0f, 1.0f) < chance)
    //    {
    //        var t = player.GetSplashTargets(target, 2, 100).Skip(1).FirstOrDefault();


    //        if (t is null)
    //            player.SendMessage($"Your {__instance.Spell.Name} wants to chain after hitting {target.Name} with {chance:P2} odds but failed.");
    //        else
    //        {
    //            //Halve chance
    //            __instance.SetProperty(FakeFloat.SpellChainChance, chance / 2);

    //            player.SendMessage($"Chaining {__instance.Spell.Name} to {target.Name} after hitting {target.Name} with {chance:P2}.");

    //            //__instance.TryCastSpell_WithRedirects
    //            //target.TryCastSpell_WithRedirects(new Spell(__instance.Spell.Id), t);
    //            player.TryCastSpell_WithRedirects(new Spell(__instance.Spell.Id), t, target, target);

    //        }

    //        //Halve chance
    //        //__instance.SetProperty(FakeFloat.SpellChainChance, chance / 2);
    //    }

    //}


    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CreateSpellProjectiles), new Type[] { typeof(Spell), typeof(WorldObject), typeof(WorldObject), typeof(bool), typeof(bool), typeof(uint) })]
    //public static void PostCreateSpellProjectiles(Spell spell, WorldObject target, WorldObject weapon, bool isWeaponSpell, bool fromProc, uint lifeProjectileDamage, ref WorldObject __instance, ref List<SpellProjectile> __result)
    //{
    //    //When launching see if the player casting chains
    //    if (__instance is not Player player)
    //        return;

    //    //Only tracking spells?
    //    if (spell.NonTracking)
    //        return;

    //    //Pass on chain chance to each projectile
    //    var chainChance = player.GetCachedFake(FakeFloat.SpellChainChance);
    //    if (chainChance > 0)
    //    {
    //        foreach (var projectile in __result)
    //            projectile.SetProperty(FakeFloat.SpellChainChance, chainChance);

    //        player.SendMessage($"Set {__result.Count} {spell.Name} projectiles to {chainChance:P2} chance of chaining.");
    //    }
    //}
}
