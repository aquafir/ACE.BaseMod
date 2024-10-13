namespace QualityOfLife;

[HarmonyPatchCategory(nameof(Features.Recklessness))]
public class Recklessness
{
    //Rewrites Recklessness handling
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetRecklessnessMod))]
    public static bool PreGetRecklessnessMod(ref Player __instance, ref float __result)
    {
        // ensure melee or missile combat mode
        if (__instance.CombatMode != CombatMode.Melee && __instance.CombatMode != CombatMode.Missile)
        {
            __result = 1.0f;
            return false;
        }

        var skill = __instance.GetCreatureSkill(Skill.Recklessness);

        // recklessness skill must be either trained or specialized to use
        if (skill.AdvancementClass < SkillAdvancementClass.Trained)
        {
            __result = 1.0f;
            return false;
        }

        // recklessness is active when attack bar is between 20% and 80% (according to wiki)
        // client attack bar range seems to indicate this might have been updated, between 10% and 90%?
        var powerAccuracyBar = __instance.GetPowerAccuracyBar();
        //if (powerAccuracyBar < 0.2f || powerAccuracyBar > 0.8f)
        if (powerAccuracyBar < PatchClass.Settings.Recklessness.PowerLow || powerAccuracyBar > PatchClass.Settings.Recklessness.PowerHigh)
        {
            __result = 1.0f;
            return false;
        }

        // recklessness only applies to non-critical hits,
        // which is handled outside of this method.

        // damage rating is increased by 20 for specialized, and 10 for trained.
        // incoming non-critical damage from all sources is increased by the same.
        var damageRating = skill.AdvancementClass == SkillAdvancementClass.Specialized ?
            PatchClass.Settings.Recklessness.RatingSpecialized :
            PatchClass.Settings.Recklessness.RatingTrained;

        // if recklessness skill is lower than current attack skill (as determined by your equipped weapon)
        // then the damage rating is reduced proportionately. The damage rating caps at 10 for trained
        // and 20 for specialized, so there is no reason to raise the skill above your attack skill.
        var attackSkill = __instance.GetCreatureSkill(__instance.GetCurrentAttackSkill());

        if (skill.Current < attackSkill.Current)
        {
            var scale = (float)skill.Current / attackSkill.Current;
            damageRating = (int)Math.Round(damageRating * scale);
        }

        // The damage rating adjustment for incoming damage is also adjusted proportinally if your Recklessness skill
        // is lower than your active attack skill

        var recklessnessMod = __instance.GetDamageRating(damageRating);    // trained DR 1.10 = 10% additional damage
                                                                           // specialized DR 1.20 = 20% additional damage
        __result = recklessnessMod;
        return false;
    }
}

public class RecklessnessSettings
{
    public float PowerLow { get; set; } = .2f;
    public float PowerHigh { get; set; } = .8f;
    public int RatingTrained { get; set; } = 10;
    public int RatingSpecialized { get; set; } = 20;
}