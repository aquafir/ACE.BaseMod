namespace Expansion.Features;

[CommandCategory(nameof(Feature.FakeSpellSplitSplash))]
[HarmonyPatchCategory(nameof(Feature.FakeSpellSplitSplash))]
internal class FakeSpellSplitSplash
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

        //Debugger.Break();
        //Check split projectiles
        if (spell.IsProjectile)
        {
            //Todo: use something like this?
            //if (player.GetProperty(FakeBool.CurrentlySpellSplit) ?? false)  return;

            //Check any split
            var splitCount = player.GetCachedFake(FakeInt.ItemSpellSplitCount);
            if (splitCount < 1) return;

            //Gate by cooldown
            var time = player.GetProperty(FakeFloat.TimestampLastSpellSplit) ?? 0;
            var current = Time.GetUnixTime();
            var delta = current - time;

            //scale?
            var scaledInterval = (1 - player.GetCachedFake(FakeFloat.ItemSpellSplitCooldownScale)) * S.Settings.SpellSettings.SplitCooldown;
            if (delta < scaledInterval)
                return;

            var rangeScale = (1 + (float)player.GetCachedFake(FakeFloat.ItemSpellSplitRangeScale)) * S.Settings.SpellSettings.SplitRange;
            var targets = player.GetSplashTargets(target, splitCount, rangeScale);

            if (targets.Count < 1)
                return;

            //Splitting is going to occur, so set the cooldown
            player.SetProperty(FakeFloat.TimestampLastSpellSplit, current);

            //Bit of debug
            //var sb = new StringBuilder($"\nSplit Targets:");
            //foreach (var t in targets)
            //    sb.Append($"\n  {t?.Name} - {t?.GetDistance(target)}");
            //player.SendMessage(sb.ToString());
            player.SendMessage($"Spell split after {delta:F1}/{scaledInterval:F1} seconds with {targets.Count}/{splitCount} targets within {rangeScale} units");

            //var splitTo = Math.Min(S.Settings.SpellSettings.SplitCount, targets.Count);
            for (var i = 0; i < targets.Count; i++)
            {
                __instance.TryCastSpell_WithRedirects(spell, targets[i], itemCaster, weapon, isWeaponSpell, fromProc);
            }
        }
        //Non-profile but harmful splashes
        else if (spell.IsHarmful)
        {
            //Check any splash
            var splashCount = player.GetCachedFake(FakeInt.ItemSpellSplashCount);
            if (splashCount < 1) return;

            //Gate by cooldown
            var time = player.GetProperty(FakeFloat.TimestampLastSpellSplash) ?? 0;
            var current = Time.GetUnixTime();
            var delta = current - time;

            var scaledInterval = (1 - player.GetCachedFake(FakeFloat.ItemSpellSplashCooldownScale)) * S.Settings.SpellSettings.SplitCooldown;
            if (delta < scaledInterval)
                return;

            var rangeScale = (1 + (float)player.GetCachedFake(FakeFloat.ItemSpellSplashRangeScale)) * S.Settings.SpellSettings.SplitRange;
            var targets = player.GetSplashTargets(target, splashCount, rangeScale);

            if (targets.Count < 1)
                return;

            //Splashing is going to occur, so set the cooldown
            player.SetProperty(FakeFloat.TimestampLastSpellSplash, current);

            //Bit of debug
            //var sb = new StringBuilder($"\nSplash Targets:");
            //foreach (var t in targets)
            //    sb.Append($"\n  {t?.Name} - {t?.GetDistance(target)}");
            //player.SendMessage(sb.ToString());
            player.SendMessage($"Spell splashed after {delta:F1}/{scaledInterval:F1} seconds with {targets.Count}/{splashCount} targets within {rangeScale} units");

            for (var i = 0; i < targets.Count; i++)
            {
                __instance.TryCastSpell_WithRedirects(spell, targets[i], itemCaster, weapon, isWeaponSpell, fromProc);
            }
        }
    }

}
