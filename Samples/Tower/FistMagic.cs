namespace Tower;

//[CommandCategory(nameof(FistMagic))]
//[HarmonyPatchCategory(nameof(FistMagic))]
[HarmonyPatch]
public class FistMagic
{
    /// <summary>
    /// Checks Power/Accuracy bar and height to trigger a spell on UA
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    public static void AfterDamage(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    {
        //if (!Settings.FistMagic)
        //    return;

        //Todo: fix checking attack type instead of equipped?  //__instance?.AttackType == AttackType.Punches
        //var watch = Stopwatch.StartNew();

        var weapon = __instance.GetEquippedWeapon();

        var validWeapon = weapon is null || weapon?.W_WeaponType == WeaponType.Unarmed;

        if (!validWeapon)
            return;

        //On quick attacks sometimes __result was null?
        if (__result is null)
            return;

        //Require life/war trained?
        //if (!__instance.Skills.TryGetValue(Skill.LifeMagic, out var life) || !__instance.Skills.TryGetValue(Skill.LifeMagic, out var war))
        //    return;

        //Power/accuracy gate?
        var power = __instance.GetPowerAccuracyBar();

        foreach (var pool in PatchClass.Settings.FistMagicPools)
        {
            if (!__instance.Skills.TryGetValue(pool.LimitingSkill, out var skill))
                continue;

//            __instance.SendMessage($"Base {pool.LimitingSkill} is {skill.Base}");

            //Try to get the most restrictive spell
            var spellPair = pool.Spells.Where(x => x.MinPower <= power && x.Base <= skill.Base).OrderByDescending(x => x.Base).FirstOrDefault();

            if (spellPair is null)
                continue;

  //          __instance.SendMessage($"Found spell pair");

            var spell = new ACE.Server.Entity.Spell(spellPair.Spell);
            __instance.TryCastSpell_WithRedirects(spell, target);
        }

        //watch.Stop();
        //__instance.SendMessage($"{watch.ElapsedTicks} ticks / {watch.ElapsedMilliseconds} ms");

        //Not using buckets
        //var randomId = Settings.FistPool[gen.Next(Settings.FistPool.Length)];
        //var powerResult = (int)(__instance.GetPowerAccuracyBar() * Settings.FistBuckets);
        //var heightResult = ((int)__result.AttackHeight - 1) * Settings.FistBuckets;
        //var attackBucket = powerResult + heightResult;
        //if (target.WeenieClassId % Settings.TotalBuckets == attackBucket)
    }
}

//Fist pools have 
public class FistPool
{
    public Skill LimitingSkill { get; set; }
    public List<SkillSpellPair> Spells { get; set; }
}

public record SkillSpellPair(int Base, SpellId Spell, float MinPower = 0);
