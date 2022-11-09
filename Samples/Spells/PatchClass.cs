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
            //Need to decide on async use
            Mod.State = ModState.Loading;
            LoadSettings();

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