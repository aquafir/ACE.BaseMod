namespace Tower.MeleeMagic;

//[CommandCategory(nameof(FistMagic))]
//[HarmonyPatchCategory(nameof(FistMagic))]
[HarmonyPatch]
public class MeleeMagic
{
    static MeleeMagicSettings Settings => PatchClass.Settings.MeleeMagic;

    /// <summary>
    /// Checks Power/Accuracy bar and height to trigger a spell on UA
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    public static void AfterDamage(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    {
        //On quick attacks sometimes __result was null?
        if (__result is null || __instance is null)
            return;

        if (Settings.RequireDamage && !__result.HasDamage)
            return;

        if (!__instance.TryGetMeleeMagicSpell(__result, out var spellId))
            return;

        var spell = new ACE.Server.Entity.Spell(spellId);
        __instance.TryCastSpell_WithRedirects(spell, target, fromProc: true);
    }
}
