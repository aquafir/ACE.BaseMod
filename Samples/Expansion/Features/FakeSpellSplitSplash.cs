using System.Collections.Concurrent;

namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeSpellSplitSplash))]
[HarmonyPatchCategory(nameof(Feature.FakeSpellSplitSplash))]
public class FakeSpellSplitSplash
{
    /// <summary>
    /// Splits or splashes a spell
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(Player.HandleCastSpell), new Type[] {
        typeof(Spell), typeof(WorldObject),typeof(WorldObject),typeof(WorldObject), typeof(bool), typeof(bool), typeof(bool)})]
    public static void HandleCastSpell(Spell spell, WorldObject target, WorldObject itemCaster = null, WorldObject weapon = null, bool isWeaponSpell = false, bool fromProc = false, bool equip = false, WorldObject __instance = null)
    {
        //Only players split?
        if (__instance is not Player player)
            return;

        //Require targeted spells?
        if (target is null)
            return;

        if (spell.IsSelfTargeted)
            return;

        //Check split projectiles
        //Debugger.Break();
        if (spell.IsProjectile)
        {
            //Todo: use something like this?
            //if (player.GetProperty(FakeBool.CurrentlySpellSplit) ?? false)  return;

            //Check any split
            var splitCount =  Math.Max(PatchClass.Settings.SpellSettings.SplitCount, player.GetCachedFake(FakeInt.ItemSpellSplitCount)); 
            if (splitCount < 1) return;

            //Gate by cooldown
            var time = player.GetProperty(FakeFloat.TimestampLastSpellSplit) ?? 0.1;
            var current = Time.GetUnixTime();
            var delta = current - time;

            //scale?
            var scaledInterval = PatchClass.Settings.SpellSettings.SplitCooldown; //(1 - player.GetCachedFake(FakeFloat.ItemSpellSplitCooldownScale)) * S.Settings.SpellSettings.SplitCooldown;
            if (delta < scaledInterval)
                return;

            var rangeScale = PatchClass.Settings.SpellSettings.SplitRange; //(1 + (float)player.GetCachedFake(FakeFloat.ItemSpellSplitRangeScale)) * S.Settings.SpellSettings.SplitRange;
            //var targets = player.GetSplashTargets(target, rangeScale).Where(x => x is not Player).Take(splitCount).ToList();
            var targets = player.GetSplashTargets(target, TargetExclusionFilter.OnlyVisibleDamageableCreature, rangeScale).Take(splitCount).ToList();


            if (targets.Count < 1)
                return;

            //Splitting is going to occur, so set the cooldown
            player.SetProperty(FakeFloat.TimestampLastSpellSplit, current);

            for (var i = 0; i < targets.Count; i++)
            {
                if (!player.IsInvalidTarget(spell, targets[i]))
                    __instance.TryCastSpell_WithRedirects(spell, targets[i], itemCaster, weapon, isWeaponSpell, fromProc);
            }
        }
        //Non-projectile but harmful splashes
        else
        {
            //Check any splash
            var splashCount = Math.Max(PatchClass.Settings.SpellSettings.SplashCount, player.GetCachedFake(FakeInt.ItemSpellSplashCount));  
            if (splashCount < 1) return;

            //Gate by cooldown
            var time = player.GetProperty(FakeFloat.TimestampLastSpellSplash) ?? 0.1;
            var current = Time.GetUnixTime();
            var delta = current - time;

            var scaledInterval = PatchClass.Settings.SpellSettings.SplitCooldown;//(1 - player.GetCachedFake(FakeFloat.ItemSpellSplashCooldownScale)) * S.Settings.SpellSettings.SplitCooldown;
            if (delta < scaledInterval)
                return;

            var rangeScale = PatchClass.Settings.SpellSettings.SplitRange;//(1 + (float)player.GetCachedFake(FakeFloat.ItemSpellSplashRangeScale)) * S.Settings.SpellSettings.SplitRange;
            var targets = spell.IsHarmful ?
                player.GetSplashTargets(target, TargetExclusionFilter.OnlyDamageableCreature, rangeScale).Take(splashCount).ToList() :
                player.GetSplashTargets(target, TargetExclusionFilter.OnlyPlayer, rangeScale).Take(splashCount).ToList();

            if (targets.Count < 1)
                return;

            //Splashing is going to occur, so set the cooldown
            player.SetProperty(FakeFloat.TimestampLastSpellSplash, current);

            for (var i = 0; i < targets.Count; i++)
            {
                if (!player.IsInvalidTarget(spell, targets[i]))
                    __instance.TryCastSpell_WithRedirects(spell, targets[i], itemCaster, weapon, isWeaponSpell, fromProc);
            }
        }
    }
}
