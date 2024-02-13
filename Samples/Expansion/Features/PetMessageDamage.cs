namespace Expansion.Features;

[HarmonyPatchCategory(nameof(Feature.PetMessageDamage))]
internal class PetMessageDamage
{
    //Creature.MeleeAttack calls on dealing/receiving combat pet damage
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.TakeDamage), new Type[] { typeof(WorldObject), typeof(DamageType), typeof(float), typeof(bool) })]
    public static void PostTakeDamage(WorldObject source, DamageType damageType, float amount, bool crit, ref Creature __instance, ref uint __result)
    {
        if (source is Pet target)
        {
            //var hp = __instance.Health.Current;
            //var alive = __instance.IsAlive;
            if (!__instance.IsAlive)
                target.P_PetOwner.SendMessage($"Your {source.Name} has slain {__instance.Name}.");
            else
                target.P_PetOwner.SendMessage($"Your {source.Name} has {(crit ? "critically " : "")}hit {__instance.Name} for {(int)amount} {damageType} damage.", ChatMessageType.CombatSelf);
            //target.P_PetOwner.SendMessage($"Your pet has {(crit ? "critically " : "")}hit {target.Name} for {amount}.", ChatMessageType.Combat);
        }
        if (__instance is Pet pet)
        {
            pet.P_PetOwner.SendMessage($"{pet.Name} has been {(crit ? "critically " : "")}hit for {(int)amount} by {source.Name} {damageType} damage.", ChatMessageType.CombatEnemy);
        }
    }

    //Refactored DamageTarget
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(SpellProjectile), nameof(SpellProjectile.DamageTarget), new Type[] { typeof(Creature), typeof(float), typeof(bool), typeof(bool), typeof(bool) })]
    //public static bool PreDamageTarget(Creature target, float damage, bool critical, bool critDefended, bool overpower, ref SpellProjectile __instance)
    //{
    //    var targetPlayer = target as Player;

    //    if (targetPlayer != null && targetPlayer.Invincible || target.IsDead)
    //        return false;

    //    var sourceCreature = __instance.ProjectileSource as Creature;
    //    var sourcePlayer = __instance.ProjectileSource as Player;

    //    var pkBattle = sourcePlayer != null && targetPlayer != null;

    //    var amount = 0u;
    //    var percent = 0.0f;

    //    var damageRatingMod = 1.0f;
    //    var heritageMod = 1.0f;
    //    var sneakAttackMod = 1.0f;
    //    var critDamageRatingMod = 1.0f;
    //    var pkDamageRatingMod = 1.0f;

    //    var damageResistRatingMod = 1.0f;
    //    var critDamageResistRatingMod = 1.0f;
    //    var pkDamageResistRatingMod = 1.0f;

    //    WorldObject equippedCloak = null;

    //    // handle life projectiles for stamina / mana
    //    if (__instance.Spell.Category == SpellCategory.StaminaLowering)
    //    {
    //        percent = damage / target.Stamina.MaxValue;
    //        amount = (uint)-target.UpdateVitalDelta(target.Stamina, (int)-Math.Round(damage));
    //    }
    //    else if (__instance.Spell.Category == SpellCategory.ManaLowering)
    //    {
    //        percent = damage / target.Mana.MaxValue;
    //        amount = (uint)-target.UpdateVitalDelta(target.Mana, (int)-Math.Round(damage));
    //    }
    //    else
    //    {
    //        // for possibly applying sneak attack to magic projectiles,
    //        // only do this for health-damaging projectiles?
    //        if (sourcePlayer != null)
    //        {
    //            // TODO: use target direction vs. projectile position, instead of player position
    //            // could sneak attack be applied to void DoTs?
    //            sneakAttackMod = sourcePlayer.GetSneakAttackMod(target);
    //            //Console.WriteLine("Magic sneak attack:  + sneakAttackMod);
    //            heritageMod = sourcePlayer.GetHeritageBonus(sourcePlayer.GetEquippedWand()) ? 1.05f : 1.0f;
    //        }

    //        var damageRating = sourceCreature?.GetDamageRating() ?? 0;
    //        damageRatingMod = Creature.AdditiveCombine(Creature.GetPositiveRatingMod(damageRating), heritageMod, sneakAttackMod);

    //        damageResistRatingMod = target.GetDamageResistRatingMod(CombatType.Magic);

    //        if (critical)
    //        {
    //            critDamageRatingMod = Creature.GetPositiveRatingMod(sourceCreature?.GetCritDamageRating() ?? 0);
    //            critDamageResistRatingMod = Creature.GetNegativeRatingMod(target.GetCritDamageResistRating());

    //            damageRatingMod = Creature.AdditiveCombine(damageRatingMod, critDamageRatingMod);
    //            damageResistRatingMod = Creature.AdditiveCombine(damageResistRatingMod, critDamageResistRatingMod);
    //        }

    //        if (pkBattle)
    //        {
    //            pkDamageRatingMod = Creature.GetPositiveRatingMod(sourceCreature?.GetPKDamageRating() ?? 0);
    //            pkDamageResistRatingMod = Creature.GetNegativeRatingMod(target.GetPKDamageResistRating());

    //            damageRatingMod = Creature.AdditiveCombine(damageRatingMod, pkDamageRatingMod);
    //            damageResistRatingMod = Creature.AdditiveCombine(damageResistRatingMod, pkDamageResistRatingMod);
    //        }

    //        damage *= damageRatingMod * damageResistRatingMod;

    //        percent = damage / target.Health.MaxValue;

    //        //Console.WriteLine($"Damage rating: " + Creature.ModToRating(damageRatingMod));

    //        equippedCloak = target.EquippedCloak;

    //        if (equippedCloak != null && Cloak.HasDamageProc(equippedCloak) && Cloak.RollProc(equippedCloak, percent))
    //        {
    //            var reducedDamage = Cloak.GetReducedAmount(__instance.ProjectileSource, damage);

    //            Cloak.ShowMessage(target, __instance.ProjectileSource, damage, reducedDamage);

    //            damage = reducedDamage;
    //            percent = damage / target.Health.MaxValue;
    //        }

    //        amount = (uint)-target.UpdateVitalDelta(target.Health, (int)-Math.Round(damage));
    //        target.DamageHistory.Add(__instance.ProjectileSource, __instance.Spell.DamageType, amount);

    //        //if (targetPlayer != null && targetPlayer.Fellowship != null)
    //        //targetPlayer.Fellowship.OnVitalUpdate(targetPlayer);
    //    }

    //    amount = (uint)Math.Round(damage);    // full amount for debugging

    //    // show debug info
    //    if (sourceCreature != null && sourceCreature.DebugDamage.HasFlag(Creature.DebugDamageType.Attacker))
    //    {
    //        SpellProjectile.ShowInfo(sourceCreature, heritageMod, sneakAttackMod, damageRatingMod, damageResistRatingMod, critDamageRatingMod, critDamageResistRatingMod, pkDamageRatingMod, pkDamageResistRatingMod, damage);
    //    }
    //    if (target.DebugDamage.HasFlag(Creature.DebugDamageType.Defender))
    //    {
    //        SpellProjectile.ShowInfo(target, heritageMod, sneakAttackMod, damageRatingMod, damageResistRatingMod, critDamageRatingMod, critDamageResistRatingMod, pkDamageRatingMod, pkDamageResistRatingMod, damage);
    //    }

    //    if (target.IsAlive)
    //    {
    //        string verb = null, plural = null;
    //        Strings.GetAttackVerb(__instance.Spell.DamageType, percent, ref verb, ref plural);
    //        var type = __instance.Spell.DamageType.GetName().ToLower();

    //        var critMsg = critical ? "Critical hit! " : "";
    //        var sneakMsg = sneakAttackMod > 1.0f ? "Sneak Attack! " : "";
    //        var overpowerMsg = overpower ? "Overpower! " : "";

    //        var nonHealth = __instance.Spell.Category == SpellCategory.StaminaLowering || __instance.Spell.Category == SpellCategory.ManaLowering;

    //        if (sourcePlayer != null)
    //        {
    //            var critProt = critDefended ? " Your critical hit was avoided with their augmentation!" : "";

    //            var attackerMsg = $"{critMsg}{overpowerMsg}{sneakMsg}You {verb} {target.Name} for {amount} points with {__instance.Spell.Name}.{critProt}";

    //            // could these crit / sneak attack?
    //            if (nonHealth)
    //            {
    //                var vital = __instance.Spell.Category == SpellCategory.StaminaLowering ? "stamina" : "mana";
    //                attackerMsg = $"With {__instance.Spell.Name} you drain {amount} points of {vital} from {target.Name}.";
    //            }

    //            if (!sourcePlayer.SquelchManager.Squelches.Contains(target, ChatMessageType.Magic))
    //                sourcePlayer.Session.Network.EnqueueSend(new GameMessageSystemChat(attackerMsg, ChatMessageType.Magic));
    //        }

    //        if (targetPlayer != null)
    //        {
    //            var critProt = critDefended ? " Your augmentation allows you to avoid a critical hit!" : "";

    //            var defenderMsg = $"{critMsg}{overpowerMsg}{sneakMsg}{__instance.ProjectileSource.Name} {plural} you for {amount} points with {__instance.Spell.Name}.{critProt}";

    //            if (nonHealth)
    //            {
    //                var vital = __instance.Spell.Category == SpellCategory.StaminaLowering ? "stamina" : "mana";
    //                defenderMsg = $"{__instance.ProjectileSource.Name} casts {__instance.Spell.Name} and drains {amount} points of your {vital}.";
    //            }

    //            if (!targetPlayer.SquelchManager.Squelches.Contains(__instance.ProjectileSource, ChatMessageType.Magic))
    //                targetPlayer.Session.Network.EnqueueSend(new GameMessageSystemChat(defenderMsg, ChatMessageType.Magic));

    //            if (sourceCreature != null)
    //                targetPlayer.SetCurrentAttacker(sourceCreature);
    //        }

    //        if (!nonHealth)
    //        {
    //            if (equippedCloak != null && Cloak.HasProcSpell(equippedCloak))
    //                Cloak.TryProcSpell(target, __instance.ProjectileSource, equippedCloak, percent);

    //            target.EmoteManager.OnDamage(sourcePlayer);

    //            if (critical)
    //                target.EmoteManager.OnReceiveCritical(sourcePlayer);
    //        }
    //    }
    //    else
    //    {
    //        var lastDamager = __instance.ProjectileSource != null ? new DamageHistoryInfo(__instance.ProjectileSource) : null;
    //        target.OnDeath(lastDamager, __instance.Spell.DamageType, critical);
    //        target.Die();
    //    }
    //    return false;
    //}

}
