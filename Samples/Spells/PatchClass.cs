using ACE.DatLoader.Entity;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Network;
using ACE.Server.WorldObjects;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using static ACE.Server.WorldObjects.Player;

namespace Spells
{
    [HarmonyPatch]
    public class PatchClass
    {
        private static Random gen = new();

        #region Settings
        //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
        const int RETRIES = 10;

        public static Settings Settings = new();
        private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
        private static FileInfo settingsInfo = new(settingsPath);

        private static JsonSerializerOptions _serializeOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public static void SaveSettings()
        {
            string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

            if (!settingsInfo.RetryWrite(jsonString, RETRIES))
            {
                ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
            }
        }

        private static void LoadSettings()
        {
            if (settingsInfo.Exists)
            {
                ModManager.Log($"Loading settings from {settingsPath}...");

                if (!settingsInfo.RetryRead(out string jsonString, RETRIES))
                {
                    Mod.State = ModState.Error;
                    return;
                }

                try
                {
                    Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
                }
                catch (Exception)
                {
                    ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
                    Mod.State = ModState.Error;
                    return;
                }
            }
            else
            {
                ModManager.Log($"Creating {settingsInfo}...");
                SaveSettings();
            }
        }
        #endregion

        #region Start/Shutdown
        public static void Start()
        {
            Mod.State = ModState.Loading;
            LoadSettings();

            if (!SpellHelper.TryInitializeSpellGroups())
                Mod.State = ModState.Error;

            if (Mod.State == ModState.Error)
            {
                ModManager.DisableModByPath(Mod.ModPath);
                return;
            }

            Mod.State = ModState.Running;
        }

        public static void Shutdown()
        {
            //if (Mod.State == ModState.Running)
            // Shut down enabled mod...

            if (Mod.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion

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

            var targets = player.GetSplashTargets(selection, Settings.SplashCount, Settings.SplashRange);

            var sb = new StringBuilder($"\nSplash Targets:");

            foreach (var target in targets)
            {
                sb.Append($"\n  {target?.Name} - {target?.GetDistance(selection)}");
            }

            player.SendMessage(sb.ToString());
        }
        #endregion

        //Todo: Clean up cached player spells on logout / disconnect

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

            //SpellBase PowerMod returns Power or a max of 25 and is used for mana costs
            trav.Property(nameof(SpellBase.Power)).SetValue(Math.Max(25,(uint)(metaScale * spell._spellBase.Power * 25)));
            trav.Property(nameof(SpellBase.Name)).SetValue($"{spell._spellBase.Name} ({metaScale})");
        }
        /// <summary>
        /// Replaces a spell with a player's personalized variant of that spell if applicable
        /// </summary>
        private static void MetaSpellSwap(Spell spell, Player __instance)
        {
            if (Settings.MetaEnabled && _metaScale.TryGetValue(__instance, out var metaScale))
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

        #region Patches
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


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), nameof(Player.GetCastingPreCheckStatus), new Type[] { typeof(Spell), typeof(uint), typeof(bool) })]
        public static void PreGetCastingPreCheckStatus(Spell spell, uint magicSkill, bool isWeaponSpell, ref Player __instance)
        {
            MetaSpellSwap(spell, __instance);
            //Debugger.Break();
        }

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


        //Players last splash / split
        private static readonly Dictionary<Player, DateTime> _lastSplash = new();
        private static readonly Dictionary<Player, DateTime> _lastSplit = new();

        /// <summary>
        /// Splits or splashes a spell
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorldObject), "HandleCastSpell", new Type[] {
            typeof(Spell), typeof(WorldObject),typeof(WorldObject),typeof(WorldObject), typeof(bool), typeof(bool), typeof(bool)})]
        public static void HandleCastSpell(Spell spell, WorldObject target, WorldObject itemCaster = null, WorldObject weapon = null, bool isWeaponSpell = false, bool fromProc = false, bool equip = false, WorldObject __instance = null)
        {
            //Should rework this to not patch the method if not enabled
            if (!Settings.SplitSpells)
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
                if (delta < Settings.SplitCooldown)
                    return;

                var targets = player.GetSplashTargets(target, Settings.SplitCount, Settings.SplitRange);

                if (targets.Count < 1)
                    return;

                //Splitting is going to occur, so set the cooldown
                _lastSplit[player] = DateTime.Now;

                //Bit of debug
                var sb = new StringBuilder($"\nSplit Targets:");
                foreach (var t in targets)
                    sb.Append($"\n  {t?.Name} - {t?.GetDistance(target)}");
                player.SendMessage(sb.ToString());

                //var splitTo = Math.Min(Settings.SplitCount, targets.Count);
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
                if (delta < Settings.SplashCooldown)
                    return;

                var targets = player.GetSplashTargets(target, Settings.SplashCount, Settings.SplashRange);

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
            if (!Settings.ReplaceBeforeCast)
                return true;

            //Something needs to be overridden
            if (!(Settings.DifferentInDungeon || Settings.RandomizeSpells))
                return true;

            var spell = new Spell(spellId);

            //Get the current spell's group
            var comps = Settings.GroupType switch
            {
                SpellGroupType.Related => spell.GetRelatedSpells(),
                SpellGroupType.Comparable => spell.GetComparableSpells()
            };

            //var sb = new StringBuilder($"Spells comparable to {spellId}:\r\n");
            //foreach (var s in comps.Select(x => new Spell(x)))
            //    sb.AppendLine($"\t{s.Id}\t{s.Name}");
            //ModManager.Log(sb.ToString());

            ModManager.Log($"LB {__instance.CurrentLandblock.Id.Raw:X8} is {(__instance.CurrentLandblock.HasDungeon ? "has dungeon" : "outdoors")}");

            //Checking if a landblock has a dungeon.  Todo: Replace with a proper way of seeing if position is in one
            if (__instance.CurrentLandblock.HasDungeon && Settings.DifferentInDungeon)
            {
                var dungeonOffset = (int)((spell.Id + __instance.CurrentLandblock.Id.Raw) % comps.Count);
                spellId = comps[dungeonOffset];
                ModManager.Log($"{spell.Name ?? "-"} becomes {new Spell(spellId).Name ?? "-"}");
            }
            else if (Settings.RandomizeSpells)
            {
                spellId = comps[gen.Next(comps.Count)];
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
            if (Settings.ReplaceBeforeCast)
                return true;

            //Something needs to be overridden
            if (!(Settings.DifferentInDungeon || Settings.RandomizeSpells))
                return true;

            if (spell.School == MagicSchool.CreatureEnchantment && __instance.Skills[Skill.CreatureEnchantment].AdvancementClass == SkillAdvancementClass.Specialized)
            {
                //Todo: Splashing debuff?
            }

            //Get the current spell's group
            var comps = Settings.GroupType switch
            {
                SpellGroupType.Related => spell.GetRelatedSpells(),
                SpellGroupType.Comparable => spell.GetComparableSpells()
            };

            //var sb = new StringBuilder($"Spells comparable to {spellId}:\r\n");
            //foreach (var s in comps.Select(x => new Spell(x)))
            //    sb.AppendLine($"\t{s.Id}\t{s.Name}");
            //ModManager.Log(sb.ToString());

            ModManager.Log($"LB {__instance.CurrentLandblock.Id.Raw:X8} is {(__instance.CurrentLandblock.HasDungeon ? "has dungeon" : "outdoors")}");

            //Checking if a landblock has a dungeon.  Todo: Replace with a proper way of seeing if position is in one
            if (__instance.CurrentLandblock.HasDungeon && Settings.DifferentInDungeon)
            {
                var dungeonOffset = (int)((spell.Id + __instance.CurrentLandblock.Id.Raw) % comps.Count);
                var dungeonId = comps[dungeonOffset];
                ModManager.Log($"{spell.Name ?? "-"} becomes {new Spell(dungeonId).Name ?? "-"}");
                spell.Init(dungeonId);
            }
            else if (Settings.RandomizeSpells)
            {
                var randomId = comps[gen.Next(comps.Count)];
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
            if (!Settings.FistMagic)
                return;

            //Todo: fix checking attack type instead of equipped?  //__instance?.AttackType == AttackType.Punches
            if (__instance.GetEquippedWeapon() is not null)
                return;

            //On quick attacks sometimes __result was null?
            if (__result is null)
                return;

            //Add ring cast if you hit the right height/power
            var randomId = Settings.FistPool[gen.Next(Settings.FistPool.Length)];
            var spell = new Spell(randomId);

            var powerResult = (int)(__instance.GetPowerAccuracyBar() * Settings.FistBuckets);
            var heightResult = ((int)__result.AttackHeight - 1) * Settings.FistBuckets;
            var attackBucket = powerResult + heightResult;

            ModManager.Log($"Attacked bucket {attackBucket} ({powerResult} + {heightResult}), target {target.WeenieClassId % Settings.TotalBuckets}");
            if (target.WeenieClassId % Settings.TotalBuckets == attackBucket)
                __instance.TryCastSpell_WithRedirects(spell, target);
        }
        #endregion
    }
}