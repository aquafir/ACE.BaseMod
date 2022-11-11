using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Network;
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
        [CommandHandler("splash", AccessLevel.Player, CommandHandlerFlag.RequiresWorld, -1)]
        public static void HandleSplash(Session session, params string[] parameters)
        {
            var player = session.Player;

            
            if (player.CurrentAppraisalTarget is null)
                return;

            var targetGuid = new ObjectGuid(player.CurrentAppraisalTarget.Value);
            var selection = session.Player.CurrentLandblock?.GetObject(targetGuid);

            //var targets = player.GetSplashTargets(5, 20f);
            var targets = player.GetSplashTargets(selection, Settings.SplashCount, Settings.SplashRange);

            var sb = new StringBuilder($"\nSplash Targets:");

            foreach (var target in targets)
            {
                sb.Append($"\n  {target?.Name} - {target?.GetDistance(selection)}");
            }

            player.SendMessage(sb.ToString());

        }
        #endregion


        //Players last splash / split
        private static readonly Dictionary<Player, DateTime> _lastSplash = new();
        private static readonly Dictionary<Player, DateTime> _lastSplit = new();
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(WorldObject), nameof(WorldObject.TryCastSpell_WithRedirects), new Type[] { 
        //    typeof(Spell), typeof(WorldObject),typeof(WorldObject),typeof(WorldObject), typeof(bool), typeof(bool), typeof(bool)})]
        //public static void TryCast_WithRedirects(Spell spell, WorldObject target, WorldObject itemCaster = null, WorldObject weapon = null, bool isWeaponSpell = false, bool fromProc = false, bool tryResist = true)
        //{
        //    Debugger.Break();
        //}


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

            //Gate by cooldown
            if (!_lastSplit.TryGetValue(player, out var time)) {
                time = DateTime.MinValue;
                _lastSplit.Add(player,time);
            }

            var delta = DateTime.Now - time;
            if (delta < Settings.SplitCooldown)
                return;

            //Only projectiles?
            if (!spell.IsProjectile)
                return;

            var targets = player.GetSplashTargets(target, Settings.SplitCount, Settings.SplitRange);

            if (targets.Count < 1)
                return;

            //Splitting is going to occur, so set the cooldown
            _lastSplit[player] = DateTime.Now;

            //Bit of debug
            var sb = new StringBuilder($"\nSplash Targets:");
            foreach (var t in targets)
                sb.Append($"\n  {t?.Name} - {t?.GetDistance(target)}");
            player.SendMessage(sb.ToString());

            //Debugger.Break();
            var splitTo = Math.Min(Settings.SplitCount, targets.Count);          
            for (var i = 0; i < splitTo; i++)
            {
                __instance.TryCastSpell_WithRedirects(spell, targets[i], itemCaster, weapon, isWeaponSpell, fromProc);
                //HandleCastSpell(spell, targets[i], itemCaster, weapon, isWeaponSpell, fromProc, equip);
            }
        }
        


        #region Patches
        //Player public bool CreatePlayerSpell(WorldObject target, TargetCategory targetCategory, uint spellId, WorldObject casterItem)
        //[HarmonyPrefix]
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

        //DoCastSpell_Inner(Spell spell, WorldObject casterItem, uint manaUsed, WorldObject target, CastingPreCheckStatus castingPreCheckStatus, bool finishCast = true)
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

        //Player DamageTarget(Creature target, WorldObject damageSource)
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