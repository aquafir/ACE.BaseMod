namespace Tower;

[CommandCategory(nameof(Feature.MeleeMagic))]
[HarmonyPatchCategory(nameof(Feature.MeleeMagic))]
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

        if (!Settings.EnabledForPvP && target is Player)
            return;

        if (!__instance.TryGetMeleeMagicSpell(__result, out var spellId))
            return;

        var spell = new Spell(spellId);
        var weapon = __instance.GetEquippedWeapon();
        __instance.TryCastSpell_Inner(spell, target, weapon, weapon, fromProc: true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), nameof(WorldObject.GetCasterElementalDamageModifier), new Type[] { typeof(WorldObject), typeof(Creature), typeof(Creature), typeof(DamageType) })]
    public static bool PreGetCasterElementalDamageModifier(WorldObject weapon, Creature wielder, Creature target, DamageType damageType, ref float __result)
    {
        if (!Settings.EnabledForPvP && target is not Player)
            return true;

        //Skip original method when checks redundant
        if (wielder is null || weapon is null || weapon.W_DamageType != damageType)
        {
            __result = 1f;
            return false;
        }

        //Use original code if not a valid non-Caster ElementalDamageModifier?
        if (!weapon.WeenieType.HasAny(WeenieType.MissileLauncher | WeenieType.Missile | WeenieType.MeleeWeapon))
            return true;


        //Reimplementation 
        var elementalDamageMod = weapon.ElementalDamageMod ?? 1.0f;

        // additive to base multiplier
        var wielderEnchantments = wielder.EnchantmentManager.GetElementalDamageMod();
        var weaponEnchantments = weapon.EnchantmentManager.GetElementalDamageMod();

        var enchantments = wielderEnchantments + weaponEnchantments;

        var modifier = (float)(elementalDamageMod + enchantments);

        if (modifier > 1.0f && target is Player)
            modifier = 1.0f + (modifier - 1.0f) * WorldObject.ElementalDamageBonusPvPReduction;

        __result = modifier;

        return false;
    }


    [CommandHandler("mm", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleLeaderboard(Session session, params string[] parameters)
    {
        var player = session.Player;

        if (player is null)
            return;

        if (!player.TryGetMeleeMagicGroup(out var group))
        {
            player.SendMessage($"No current MeleeMagic group.");
            return;
        }

        var sb = new StringBuilder();

        foreach (var height in group.Pools)
        {
            sb.Append($"\n{height.Key}");

            foreach (var pool in height.Value)
            {
                sb.Append($"\n  {pool.LimitingSkill} @ {pool.MinimumSlider:P2} power");
                foreach (var spell in pool.Spells)
                {
                    sb.Append($"\n    {spell.Key,-5}{spell.Value}");
                }
            }
        }

        player.SendMessage($"{sb}");
    }
}
