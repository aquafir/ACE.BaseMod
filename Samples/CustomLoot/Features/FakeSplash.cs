﻿using static ACE.Server.WorldObjects.Player;
using System.Text;
using ACE.DatLoader.Entity;
using ACE.DatLoader;
using ACE.Entity;
using ACE.Server.Command;
using ACE.Server.Network;

namespace CustomLoot.Features;

[HarmonyPatchCategory(nameof(Feature.FakeSplash))]
internal class FakeSplash
{
    //Players last splash / split
    private static readonly Dictionary<Player, DateTime> _lastSplash = new();
    private static readonly Dictionary<Player, DateTime> _lastSplit = new();

    #region Commands
    [CommandHandler("meta", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, 1)]
    public static void HandleMeta(Session session, params string[] parameters)
    {
        //Get some scale to adjust spells by
        if (!double.TryParse(parameters[0], out var metaScale))
            return;

        var player = session.Player;

        if (!_metaScale.ContainsKey(player))
        {
            _metaScale.Add(player, metaScale);
        }
        //Clear if modified spells (if the scale is changing?)
        if (_playerSpells.ContainsKey(player))
            _playerSpells[player] = new();
        if (_playerSpellBases.ContainsKey(player))
            _playerSpellBases[player] = new();


        _metaScale[player] = metaScale;

        player.SendMessage($"Scaling spells by {metaScale}");
    }

    [CommandHandler("splash", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, -1)]
    public static void HandleSplash(Session session, params string[] parameters)
    {
        var player = session.Player;


        if (player.CurrentAppraisalTarget is null)
            return;

        var targetGuid = new ObjectGuid(player.CurrentAppraisalTarget.Value);
        var selection = session.Player.CurrentLandblock?.GetObject(targetGuid);

        var targets = player.GetSplashTargets(selection, S.Settings.SpellSettings.SplashCount, S.Settings.SpellSettings.SplashRange);

        var sb = new StringBuilder($"\nSplash Targets:");

        foreach (var target in targets)
        {
            sb.Append($"\n  {target?.Name} - {target?.GetDistance(selection)}");
        }

        player.SendMessage(sb.ToString());
    }
    #endregion

    #region Patches
    //This is probably where a pre-cast random should happen to effect difficulty?
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.GetCastingPreCheckStatus), new Type[] { typeof(Spell), typeof(uint), typeof(bool) })]
    public static void PreGetCastingPreCheckStatus(Spell spell, uint magicSkill, bool isWeaponSpell, ref Player __instance)
    {
        MetaSpellSwap(spell, __instance);
        //Debugger.Break();
    }
    #region Unused Meta Targets
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(WorldObject), "HandleCastSpell", 
    //    new Type[] { typeof(Spell), typeof(WorldObject), typeof(WorldObject), typeof(WorldObject), typeof(bool), typeof(bool), typeof(bool) })]
    //public static void MetaCast_HandleCastSpell(ref Spell spell, WorldObject target, WorldObject itemCaster = null, WorldObject weapon = null, bool isWeaponSpell = false, bool fromProc = false, bool equip = false, WorldObject __instance = null)
    //{
    //    var player = __instance as Player;
    //    if (player is not null)
    //    {
    //        MetaSpellSwap(spell, player);
    //        player.SendMessage($"{spell._spellBase.Power} power");
    //    }

    //}

    ///// <summary>
    ///// Adjust spell at creation using /meta
    ///// </summary>
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Player), nameof(Player.DoCastSpell_Inner), new Type[] { typeof(Spell), typeof(WorldObject), typeof(uint), typeof(WorldObject), typeof(CastingPreCheckStatus), typeof(bool), })]
    //public static void MetaCast_DoCastSpell_Inner(ref Spell spell, WorldObject casterItem, uint manaUsed, WorldObject target, CastingPreCheckStatus castingPreCheckStatus, bool finishCast, ref Player __instance)
    //{
    //    MetaSpellSwap(spell, __instance);
    //    __instance.SendMessage($"{spell._spellBase.Power} power - {spell.Power} - {spell.PowerMod}");
    //}

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(Player), nameof(Player.GetCastingPreCheckStatus), new Type[] { typeof(Spell), typeof(uint), typeof(bool) })]
    //public static void GetCastingPreCheckStatus(Spell spell, uint magicSkill, bool isWeaponSpell)
    //{
    //    Debugger.Break();
    //}

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Proficiency), nameof(Proficiency.OnSuccessUse), new Type[] {
    //    typeof(Player), typeof(CreatureSkill),typeof(uint)})]
    //public static void OnSuccessUse(Player player, CreatureSkill skill, uint difficulty)
    //{
    //   // Debugger.Break();
    //}

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(WorldObject), nameof(WorldObject.CalculateProjectileOrigins), new Type[] {
    //    typeof(Spell), typeof(ProjectileSpellType),typeof(WorldObject)})]
    //public static void CalculateProjectileOrigins(Spell spell, ProjectileSpellType spellType, WorldObject target, ref List<Vector3> __result)
    //{
    //    //Debugger.Break();
    //} 
    #endregion

    /// <summary>
    /// Splits or splashes a spell
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(WorldObject), "HandleCastSpell", new Type[] {
        typeof(Spell), typeof(WorldObject),typeof(WorldObject),typeof(WorldObject), typeof(bool), typeof(bool), typeof(bool)})]
    public static void HandleCastSpell(Spell spell, WorldObject target, WorldObject itemCaster = null, WorldObject weapon = null, bool isWeaponSpell = false, bool fromProc = false, bool equip = false, WorldObject __instance = null)
    {
        //Should rework this to not patch the method if not enabled
        if (!S.Settings.SpellSettings.SplitSpells)
            return;

        //Only players split
        if (__instance is not Player)
            return;

        //Non-null player
        var player = __instance as Player;
        if (player is null)
            return;

        //Check split projectiles
        if (spell.IsProjectile)
        {
            //Gate by cooldown
            if (!_lastSplit.TryGetValue(player, out var time))
            {
                time = DateTime.MinValue;
                _lastSplit.Add(player, time);
            }

            var delta = DateTime.Now - time;
            if (delta < S.Settings.SpellSettings.SplitCooldown)
                return;

            var targets = player.GetSplashTargets(target, S.Settings.SpellSettings.SplitCount, S.Settings.SpellSettings.SplitRange);

            if (targets.Count < 1)
                return;

            //Splitting is going to occur, so set the cooldown
            _lastSplit[player] = DateTime.Now;

            //Bit of debug
            var sb = new StringBuilder($"\nSplit Targets:");
            foreach (var t in targets)
                sb.Append($"\n  {t?.Name} - {t?.GetDistance(target)}");
            player.SendMessage(sb.ToString());

            //var splitTo = Math.Min(S.Settings.SpellSettings.SplitCount, targets.Count);
            for (var i = 0; i < targets.Count; i++)
            {
                __instance.TryCastSpell_WithRedirects(spell, targets[i], itemCaster, weapon, isWeaponSpell, fromProc);
            }
        }
        //Non-profile but harmful splashes
        else if (spell.IsHarmful)
        {
            //Gate by cooldown
            if (!_lastSplash.TryGetValue(player, out var time))
            {
                time = DateTime.MinValue;
                _lastSplash.Add(player, time);
            }

            var delta = DateTime.Now - time;
            if (delta < S.Settings.SpellSettings.SplashCooldown)
                return;

            var targets = player.GetSplashTargets(target, S.Settings.SpellSettings.SplashCount, S.Settings.SpellSettings.SplashRange);

            if (targets.Count < 1)
                return;

            //Splashing is going to occur, so set the cooldown
            _lastSplash[player] = DateTime.Now;

            //Bit of debug
            var sb = new StringBuilder($"\nSplash Targets:");
            foreach (var t in targets)
                sb.Append($"\n  {t?.Name} - {t?.GetDistance(target)}");
            player.SendMessage(sb.ToString());

            for (var i = 0; i < targets.Count; i++)
            {
                __instance.TryCastSpell_WithRedirects(spell, targets[i], itemCaster, weapon, isWeaponSpell, fromProc);
            }
        }
    }

    /// <summary>
    /// Adjust a spell before animations
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.CreatePlayerSpell), new Type[] { typeof(WorldObject), typeof(TargetCategory), typeof(uint), typeof(WorldObject) })]
    private static bool CreatePlayerSpell(WorldObject target, TargetCategory targetCategory, ref uint spellId, WorldObject casterItem, ref Player __instance)
    {
        //This occurs before cast
        if (!S.Settings.SpellSettings.ReplaceBeforeCast)
            return true;

        //Something needs to be overridden
        if (!(S.Settings.SpellSettings.DifferentInDungeon || S.Settings.SpellSettings.RandomizeSpells))
            return true;

        var spell = new Spell(spellId);

        //Get the current spell's group
        var comps = S.Settings.SpellSettings.GroupType switch
        {
            SpellGroupType.Related => spell.GetRelatedSpells().ToArray(),
            SpellGroupType.Comparable => spell.GetComparableSpells().ToArray()
        };

        //var sb = new StringBuilder($"Spells comparable to {spellId}:\r\n");
        //foreach (var s in comps.Select(x => new Spell(x)))
        //    sb.AppendLine($"\t{s.Id}\t{s.Name}");
        //ModManager.Log(sb.ToString());

        ModManager.Log($"LB {__instance.CurrentLandblock.Id.Raw:X8} is {(__instance.CurrentLandblock.HasDungeon ? "has dungeon" : "outdoors")}");

        //Checking if a landblock has a dungeon.  Todo: Replace with a proper way of seeing if position is in one
        if (__instance.CurrentLandblock.HasDungeon && S.Settings.SpellSettings.DifferentInDungeon)
        {
            var dungeonOffset = (int)((spell.Id + __instance.CurrentLandblock.Id.Raw) % comps.Length);
            spellId = comps[dungeonOffset];
            ModManager.Log($"{spell.Name ?? "-"} becomes {new Spell(spellId).Name ?? "-"}");
        }
        else if (S.Settings.SpellSettings.RandomizeSpells)
        {
            if (!comps.TryGetRandom(out spellId))
                return true;
            ModManager.Log($"{spell.Name ?? "-"} randomly {new Spell(spellId).Name ?? "-"}");
        }

        return true;
    }

    /// <summary>
    /// Adjust a spell after animations
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.DoCastSpell_Inner), new Type[] { typeof(Spell), typeof(WorldObject), typeof(uint), typeof(WorldObject), typeof(CastingPreCheckStatus), typeof(bool), })]
    public static bool DoCastSpell_Inner(ref Spell spell, WorldObject casterItem, uint manaUsed, WorldObject target, CastingPreCheckStatus castingPreCheckStatus, bool finishCast, ref Player __instance)
    {
        //This occurs after cast
        if (S.Settings.SpellSettings.ReplaceBeforeCast)
            return true;

        //Something needs to be overridden
        if (!(S.Settings.SpellSettings.DifferentInDungeon || S.Settings.SpellSettings.RandomizeSpells))
            return true;

        if (spell.School == MagicSchool.CreatureEnchantment && __instance.Skills[Skill.CreatureEnchantment].AdvancementClass == SkillAdvancementClass.Specialized)
        {
            //Todo: Splashing debuff?
        }

        //Get the current spell's group
        var comps = S.Settings.SpellSettings.GroupType switch
        {
            SpellGroupType.Related => spell.GetRelatedSpells().ToArray(),
            SpellGroupType.Comparable => spell.GetComparableSpells().ToArray()
        };

        //var sb = new StringBuilder($"Spells comparable to {spellId}:\r\n");
        //foreach (var s in comps.Select(x => new Spell(x)))
        //    sb.AppendLine($"\t{s.Id}\t{s.Name}");
        //ModManager.Log(sb.ToString());

        ModManager.Log($"LB {__instance.CurrentLandblock.Id.Raw:X8} is {(__instance.CurrentLandblock.HasDungeon ? "has dungeon" : "outdoors")}");

        //Checking if a landblock has a dungeon.  Todo: Replace with a proper way of seeing if position is in one
        if (__instance.CurrentLandblock.HasDungeon && S.Settings.SpellSettings.DifferentInDungeon)
        {
            var dungeonOffset = (int)((spell.Id + __instance.CurrentLandblock.Id.Raw) % comps.Length);
            var dungeonId = comps[dungeonOffset];
            ModManager.Log($"{spell.Name ?? "-"} becomes {new Spell(dungeonId).Name ?? "-"}");
            spell.Init(dungeonId);
        }
        else if (S.Settings.SpellSettings.RandomizeSpells)
        {
            if (!comps.TryGetRandom(out var randomId))
                return true;
            ModManager.Log($"{spell.Name ?? "-"} randomly {new Spell(randomId).Name ?? "-"}");
            spell.Init(randomId);
        }

        spell.Formula.CurrentFormula = new();   //Lazy way of preventing this from throwing a null

        return true;
    }

    /// <summary>
    /// Checks Power/Accuracy bar and height to trigger a spell on UA
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
    public static void AfterDamage(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
    {
        if (!S.Settings.SpellSettings.FistMagic)
            return;

        //Todo: fix checking attack type instead of equipped?  //__instance?.AttackType == AttackType.Punches
        if (__instance.GetEquippedWeapon() is not null)
            return;

        //On quick attacks sometimes __result was null?
        if (__result is null)
            return;

        //Add ring cast if you hit the right height/power
        if (!S.Settings.SpellSettings.FistPool.TryGetRandom(out var randomId))
            return;
        var spell = new Spell(randomId);

        var powerResult = (int)(__instance.GetPowerAccuracyBar() * S.Settings.SpellSettings.FistBuckets);
        var heightResult = ((int)__result.AttackHeight - 1) * S.Settings.SpellSettings.FistBuckets;
        var attackBucket = powerResult + heightResult;

        ModManager.Log($"Attacked bucket {attackBucket} ({powerResult} + {heightResult}), target {target.WeenieClassId % S.Settings.SpellSettings.TotalBuckets}");
        if (target.WeenieClassId % S.Settings.SpellSettings.TotalBuckets == attackBucket)
            __instance.TryCastSpell_WithRedirects(spell, target);
    }

    #endregion

    #region Meta Spells
    //Meta scale of cost/damage made with /meta command
    private static readonly Dictionary<Player, double> _metaScale = new();
    //Tuple for a players cloned copy of a spell/base
    private static readonly Dictionary<Player, Dictionary<Spell, ACE.Database.Models.World.Spell>> _playerSpells = new();
    private static readonly Dictionary<Player, Dictionary<Spell, ACE.DatLoader.Entity.SpellBase>> _playerSpellBases = new();

    /// <summary>
    /// Filters which spells are adjusted with /meta
    /// </summary>
    private static bool IsModifiable(Spell spell)
    {
        //if (!spell.IsProjectile)
        //    return false;

        return true;
    }
    /// <summary>
    /// Creates or finds a collection of modified Spell/SpellBase for a player
    /// </summary>
    private static bool TryGetPlayerCustomSpells(Player player, out Dictionary<Spell, ACE.Database.Models.World.Spell> dbSpells, out Dictionary<Spell, SpellBase> spellBases)
    {
        //Track tracking a players variations
        if (!_playerSpells.TryGetValue(player, out dbSpells))
        {
            player.SendMessage($"Tracking custom variations of spells for you...");
            dbSpells = new();
            _playerSpells.Add(player, dbSpells);
        }
        if (!_playerSpellBases.TryGetValue(player, out spellBases))
        {
            spellBases = new();
            _playerSpellBases.Add(player, spellBases);
        }

        //May want to make this fail on a throw?
        return true;
    }
    /// <summary>
    /// Creates or finds a variant of a spell for a player
    /// </summary>
    private static bool TryMakeSpellVariant(Spell spell, Player player, out ACE.Database.Models.World.Spell dbSpell, double metaScale = 1)
    {
        dbSpell = null;

        if (!_playerSpells.TryGetValue(player, out var dbSpells))
            return false;

        //Adjusting _spell adjusts ALL spell uses of that type, so create a shallow copy of the DB's Spell and SpellBase?
        if (!dbSpells.TryGetValue(spell, out dbSpell))
        {
            player.SendMessage($"Created a custom variation of {spell.Name} - {spell.Id}...");

            //Set modified properties for DB Spell
            dbSpell = spell._spell.Clone();

            dbSpells.Add(spell, dbSpell);
        }

        ModifySpellCopy(spell, dbSpell, metaScale);

        //May want to make this fail or switch to null checking
        return true;
    }
    /// <summary>
    /// Mutates a database template for a spell
    /// </summary>
    private static void ModifySpellCopy(Spell spell, ACE.Database.Models.World.Spell dbSpell, double metaScale)
    {
        //Do stuff for projectiles                    
        if (spell.IsProjectile)
        {
            dbSpell.BaseIntensity = (int)(metaScale * spell._spell.BaseIntensity ?? 1);

            var pType = SpellProjectile.GetProjectileSpellType(dbSpell.Id);
            if (dbSpell.NumProjectiles is not null)
                dbSpell.NumProjectiles = pType switch
                {
                    ProjectileSpellType.Blast => (int)(metaScale * spell._spell.NumProjectiles),
                    ProjectileSpellType.Volley => (int)(metaScale * spell._spell.NumProjectiles),
                    ProjectileSpellType.Wall => (int)(metaScale * spell._spell.NumProjectiles),
                    ProjectileSpellType.Ring => (int)(metaScale * spell._spell.NumProjectiles),
                    ProjectileSpellType.Strike => (int)(metaScale * spell._spell.NumProjectiles),
                    _ => 1
                };

            if (dbSpell.SpreadAngle is not null)
                dbSpell.SpreadAngle = 360f;
            if (dbSpell.PaddingOriginY is not null)
                dbSpell.PaddingOriginY = (float)(metaScale * spell._spell.PaddingOriginY);
            if (dbSpell.PaddingOriginX is not null)
                dbSpell.PaddingOriginX = (float)(metaScale * spell._spell.PaddingOriginX);
            if (dbSpell.PaddingOriginZ is not null)
                dbSpell.PaddingOriginZ = (float)(metaScale * spell._spell.PaddingOriginZ);
        }


    }

    /// <summary>
    /// Creates or finds a variant of a spell for a player
    /// </summary>
    private static bool TryMakeBaseVariant(Spell spell, Player player, out SpellBase spellbase, double metaScale = 1)
    {
        spellbase = null;

        if (!_playerSpellBases.TryGetValue(player, out var spellbases))
            return false;

        //Adjusting _spell adjusts ALL spell uses of that type, so create a shallow copy of the DB's Spell and SpellBase?
        if (!spellbases.TryGetValue(spell, out spellbase))
        {
            player.SendMessage($"Created a custom SpellBase variation of {spell.Name} - {spell.Id}...");

            //Set modified properties for DB Spell
            spellbase = spell._spellBase.DeepClone();

            spellbases.Add(spell, spellbase);
        }

        ModifySpellBaseCopy(spell, spellbase, metaScale);

        //May want to make this fail
        return true;
    }
    /// <summary>
    /// Mutates a database template for a spell
    /// </summary>
    private static void ModifySpellBaseCopy(Spell spell, SpellBase spellbase, double metaScale)
    {
        //Spellbase has private setters so Traverse is used to access
        var trav = Traverse.Create(spellbase);

        //Todo: Think about spell components.  If spellWords field is set I don't think it's an issue?
        trav.Field("spellWords").SetValue($"Abracazoop ({spell._spellBase.GetSpellWords(DatManager.PortalDat.SpellComponentsTable)})");

        //SpellBase PowerMod returns Power or a max of 25 and is used for mana costs
        trav.Property(nameof(SpellBase.Power)).SetValue(Math.Max(25, (uint)(metaScale * 25)));
        trav.Property(nameof(SpellBase.Name)).SetValue($"{spell._spellBase.Name} ({metaScale})");

        var pType = SpellProjectile.GetProjectileSpellType(spell.Id);
        if (pType == ProjectileSpellType.Ring)
            //This will fail if not cast to a single
            trav.Property(nameof(SpellBase.BaseRangeConstant)).SetValue((float)Math.Sqrt(metaScale * spell._spellBase.BaseRangeConstant));

        //Debugger.Break();
    }
    /// <summary>
    /// Replaces a spell with a player's personalized variant of that spell if applicable
    /// </summary>
    private static void MetaSpellSwap(Spell spell, Player __instance)
    {
        if (S.Settings.SpellSettings.MetaEnabled && _metaScale.TryGetValue(__instance, out var metaScale))
        {
            //Filter out spells you don't do anything with
            if (!IsModifiable(spell))
                return;

            if (!TryGetPlayerCustomSpells(__instance, out var playerSpells, out var playerSpellBases))
                return;

            if (!TryMakeSpellVariant(spell, __instance, out var metaSpell, metaScale))
                return;

            if (!TryMakeBaseVariant(spell, __instance, out var metaSpellBase, metaScale))
                return;

            //Swap regular copy for cached variant
            spell._spell = metaSpell;
            spell._spellBase = metaSpellBase;
        }
    }
    #endregion
}
