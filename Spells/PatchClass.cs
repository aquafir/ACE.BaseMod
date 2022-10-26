using ACE.Server.Entity;
using static ACE.Server.WorldObjects.Player;

namespace Spells
{
    [HarmonyPatch]
    public class PatchClass
    {
        public static Settings Settings = new();
        private static string filePath = Path.Combine(Mod.ModPath, "Settings.json");
        private static Random gen = new();

        //protected bool HandleCastSpell(Spell spell, WorldObject target, WorldObject itemCaster = null, WorldObject weapon = null, bool isWeaponSpell = false, bool fromProc = false, bool equip = false)

        //Player DoCastSpell(Spell spell, WorldObject caster, uint magicSkill, uint manaUsed, WorldObject target, CastingPreCheckStatus castingPreCheckStatus, bool checkAngle = true)

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
            var comps = Settings.UseComparable ? spell.GetComparableSpells() : spell.GetRelatedSpells();

            //var sb = new StringBuilder($"Spells comparable to {spellId}:\r\n");
            //foreach (var s in comps.Select(x => new Spell(x)))
            //    sb.AppendLine($"\t{s.Id}\t{s.Name}");
            //ModManager.Log(sb.ToString());

            ModManager.Log($"LB {__instance.CurrentLandblock.Id.Raw:X8} is {(__instance.CurrentLandblock.HasDungeon ? "has dungeon" : "outdoors")}");

            //Checking if a landblock has a dungeon.  Todo: Replace with a proper way of seeing if position is in one
            if (__instance.CurrentLandblock.HasDungeon && Settings.DifferentInDungeon)
            {
                //Not the right way to do this 1:1.  Should probably use the index of the spell in its group
                var dungeonId = (int)((spell.Id + __instance.CurrentLandblock.Id.Raw) % comps.Count);
                ModManager.Log($"{spell.Name ?? "-"} becomes {new Spell(comps[dungeonId]).Name ?? "-"}");
                spellId = comps[dungeonId];
            }
            else if (Settings.RandomizeSpells)
            {
                var randomId = (int)comps[gen.Next(comps.Count)];
                ModManager.Log($"{spell.Name ?? "-"} randomly {new Spell(comps[randomId]).Name ?? "-"}");
                spellId = comps[randomId];
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

            //Get the current spell's group
            var comps = Settings.UseComparable ? spell.GetComparableSpells() : spell.GetRelatedSpells();

            //var sb = new StringBuilder($"Spells comparable to {spellId}:\r\n");
            //foreach (var s in comps.Select(x => new Spell(x)))
            //    sb.AppendLine($"\t{s.Id}\t{s.Name}");
            //ModManager.Log(sb.ToString());

            ModManager.Log($"LB {__instance.CurrentLandblock.Id.Raw:X8} is {(__instance.CurrentLandblock.HasDungeon ? "has dungeon" : "outdoors")}");

            //Checking if a landblock has a dungeon.  Todo: Replace with a proper way of seeing if position is in one
            if (__instance.CurrentLandblock.HasDungeon && Settings.DifferentInDungeon)
            {
                //Not the right way to do this 1:1.  Should probably use the index of the spell in its group
                var dungeonId = (int)((spell.Id + __instance.CurrentLandblock.Id.Raw) % comps.Count);
                ModManager.Log($"{spell.Name ?? "-"} becomes {new Spell(comps[dungeonId]).Name ?? "-"}");
                spell.Init(comps[dungeonId]);
            }
            else if (Settings.RandomizeSpells)
            {
                var randomId = (int)comps[gen.Next(comps.Count)];
                ModManager.Log($"{spell.Name ?? "-"} randomly {new Spell(comps[randomId]).Name ?? "-"}");
                spell.Init(comps[randomId]);
            }

            spell.Formula.CurrentFormula = new();   //Lazy way of preventing this from throwing a null

            //Maybe of interest?
            //__instance.MagicState.SetCastParams(spell, __instance, 500, manaUsed, target, castingPreCheckStatus);
            //__instance.MagicState.OnCastDone();

            return true;
        }

        //Player DamageTarget(Creature target, WorldObject damageSource)
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), nameof(Player.DamageTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
        public static void AfterDamage(Creature target, WorldObject damageSource, ref Player __instance, ref DamageEvent __result)
        {
            if (!Settings.FistMagic)
                return;

            if (__instance is not Player)
                return;

            //Todo: fix checking attack type instead of equipped?  de.AttackType = AttackType.Punches
            if (__instance.GetEquippedWeapon() is not null)
                return;

            var randomId = Settings.FistPool[gen.Next(Settings.FistPool.Length)];
            var spell = new Spell(randomId);
            __instance.TryCastSpell_WithRedirects(spell, target);
            
            //__instance?.AttackHeight
            //__instance?.AttackType == AttackType.Punches
        }

        public static void Start()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    ModManager.Log($"Loading from {filePath}");
                    var jsonString = File.ReadAllText(filePath);
                    Settings = JsonSerializer.Deserialize<Settings>(jsonString);

                    SpellHelper.TryInitializeSpellGroups();
                }
                catch (Exception ex)
                {
                    ModManager.Log($"Failed to deserialize from {filePath}");
                    Settings = new Settings();
                    return;
                }
            }
            else
            {
                ModManager.Log($"Creating {filePath}");
                string jsonString = JsonSerializer.Serialize(Settings);
                File.WriteAllText(filePath, jsonString);
            }
        }
        public static void Shutdown()
        {
            string jsonString = JsonSerializer.Serialize(Settings);
            File.WriteAllText(filePath, jsonString);
        }
    }
}