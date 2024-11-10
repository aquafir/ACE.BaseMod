using ACE.Database;
using ACE.DatLoader.FileTypes;
using CustomSpells;
using Ganss.Excel;

namespace CustomSpells;

[HarmonyPatch]
public class PatchClass(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnWorldOpen() {
        Settings = SettingsContainer.Settings;
        SetupSpells(); 
    }

    private static void SetupSpells()
    {
        if (Settings.AutoloadSpreadsheet)
            LoadSpellSpreadsheet();

        CustomizeSpells(Settings.CustomSpells);

        OverrideSpellSets();
    }

    private static void CustomizeSpells(List<SpellCustomization> customSpells)
    {
        foreach (var customSpell in customSpells)
        {
            //Get keys from SpellId, defaulting to overriding the template
            uint template = (uint)customSpell.Template;
            uint key = (uint)(customSpell.Id ?? customSpell.Template);

            try
            {
                //Get and verify spell copies
                var dbSpell = DatabaseManager.World.GetCachedSpell(template).Clone();
                var spellBase = new Spell(template)._spellBase.Clone();

                if (dbSpell is null || spellBase is null)
                    continue;

                customSpell.Apply(spellBase, dbSpell);

                //Set SpellBase - DatManager.PortalDat.SpellTable.Spells
                //if (!DatManager.PortalDat.SpellTable.Spells.ContainsKey(key))
                //    DatManager.PortalDat.SpellTable.Spells.Add(key, spellBase);
                //else
                DatManager.PortalDat.SpellTable.Spells[key] = spellBase;

                //Set DB Spell - DatabaseManager.World.spellCache / GetCachedSpell
                DatabaseManager.World.spellCache[key] = dbSpell;

                //ModManager.Log($"Customized Spell: {customSpell.Template} -> {customSpell.Id}");
            }
            catch (Exception)
            {
                ModManager.Log($"Failed to customized {customSpell.Template} -> {customSpell.Id}");
            }
        }
    }

    private static void LoadSpellSpreadsheet(Player player = null)
    {
        var sw = Stopwatch.StartNew();
        var spells = SpellCustomization.ParseCustomizations();

        CustomizeSpells(spells);
        sw.Stop();

        var msg = $"Loaded {spells.Count} custom spells in {sw.ElapsedMilliseconds}ms from:\n{Settings.Spreadsheet}";
        ModManager.Log(msg);
        player?.SendMessage(msg);
    }

    private static void OverrideSpellSets()
    {
        try
        {
            foreach (var setOverride in Settings.Sets)
            {
                var key = (uint)setOverride.Key;

                //Add missing sets
                if (!DatManager.PortalDat.SpellTable.SpellSet.TryGetValue(key, out var datSet))
                {
                    datSet = new();
                    DatManager.PortalDat.SpellTable.SpellSet.Add(key, datSet);
                }

                //Order overriding sets by tier
                var tiers = setOverride.Value.OrderBy(x => x.NumEquipped);
                var start = tiers.FirstOrDefault().NumEquipped;
                var end = tiers.LastOrDefault().NumEquipped;

                //Clear sets
                datSet.SpellSetTiers.Clear();
                datSet.SpellSetTiersNoGaps.Clear();
                datSet.HighestTier = end;

                foreach (var tier in tiers)
                {
                    //Make and add SpellSetTier
                    SpellSetTiers sst = new() { Spells = tier.Spells.Select(x => (uint)x).ToList() };
                    datSet.SpellSetTiers.TryAdd(tier.NumEquipped, sst);
                }

                //Create the no-gaps list using ACE logic.  Could be improved
                SpellSetTiers lastSpellSetTier = null;
                for (uint i = 0; i <= datSet.HighestTier; i++)
                {
                    if (datSet.SpellSetTiers.TryGetValue(i, out var spellSetTiers))
                        lastSpellSetTier = spellSetTiers;

                    if (lastSpellSetTier != null)
                        datSet.SpellSetTiersNoGaps.TryAdd(i, lastSpellSetTier);
                }
            }
        }
        catch (Exception ex)
        {
            ModManager.Log(ex.Message, ModManager.LogLevel.Error);
            return;
        }

        ModManager.Log($"Replaced {Settings.Sets.Count} EquipmentSets with a combined {Settings.Sets.Sum(x => x.Value.Count())} tiers and {Settings.Sets.Sum(x => x.Value.Sum(s => s.Spells.Count))} set spells");
    }
 

    [CommandHandler("loadspells", AccessLevel.Developer, CommandHandlerFlag.None)]
    public static void HandleLoadSpellSpreadsheet(Session session, params string[] parameters)
    {
        LoadSpellSpreadsheet(session?.Player);
    }

    [CommandHandler("spelldump", AccessLevel.Developer, CommandHandlerFlag.ConsoleInvoke)]
    public static void HandleSpellDump(Session session, params string[] parameters)
    {
        List<SpellCustomization> sc = new();
        foreach (var spellId in Enum.GetValues<SpellId>())
        {
            var spell = new Spell(spellId);
            if (spell is null || spell._spellBase is null || spell._spell is null)
                continue;

            var custom = new SpellCustomization(spell);
            if (custom is null)
                continue;

            sc.Add(custom);
        }

        var path = Path.Combine(CustomSpells.Mod.Instance.ModPath, "Dump.xlsx");

        ExcelMapper excel = new() { CreateMissingHeaders = true, IgnoreNestedTypes = true };
        excel.SetupCustomSpellMappings();
        excel.Save(path, sc, "Spells");

        var msg = $"Exported {sc.Count} spells to be customized to:\n{path}";
        ModManager.Log(msg);
    }

    [CommandHandler("listset", AccessLevel.Player, CommandHandlerFlag.None)]
    public static void HandleSS(Session session, params string[] parameters)
    {
        var p = session.Player;
        var s = GetLastAppraisedObject(session);

        if (s is null || s.EquipmentSetId is null)
            return;

        if (!DatManager.PortalDat.SpellTable.SpellSet.TryGetValue((uint)s.EquipmentSetId, out var set))
            return;

        //ACE.Server.Entity.SpellSet loads from DatManager.PortalDat.SpellTable.SpellSet
        var sb = new StringBuilder($"\nEquipmentSet {s.EquipmentSetId}:");
        foreach (var tier in set.SpellSetTiersNoGaps)
        {
            sb.Append($"\nWearing >= {tier.Key}");
            foreach (var spellId in tier.Value.Spells)
            {
                Spell spell = new Spell(spellId);
                sb.Append($"\n  ({spellId}) {spell.Name}");
            }
        }
        p.SendMessage($"{sb}");
    }

    //[CommandHandler("setup-spells", AccessLevel.Developer, CommandHandlerFlag.None)]
    //public static void HandleSetupSpells(Session session, params string[] parameters)
    //{
    //    SetupSpells();
    //}

    //Lazy helper
    public static WorldObject GetLastAppraisedObject(Session session)
    {
        var targetID = session.Player.RequestedAppraisalTarget;
        if (targetID == null)
        {
            return null;
        }

        var target = session.Player.FindObject(targetID.Value, Player.SearchLocations.Everywhere, out _, out _, out _);
        if (target == null)
        {
            return null;
        }
        return target;
    }



    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Spell), nameof(Spell.GetMagicSkill))]
    //public static bool PreGetMagicSkill(ref Spell __instance, ref Skill __result)
    //{
    //    Debugger.Break();
    //    __result = __instance.School switch
    //    {
    //        MagicSchool.CreatureEnchantment => Skill.CreatureEnchantment,
    //        MagicSchool.WarMagic => Skill.WarMagic,
    //        MagicSchool.ItemEnchantment => Skill.ItemEnchantment,
    //        MagicSchool.LifeMagic => Skill.LifeMagic,
    //        MagicSchool.VoidMagic => Skill.VoidMagic,
    //        MagicSchool x when x <= (MagicSchool)Skill.Summoning => (Skill)x,
    //        _ => default,
    //    };

    //    return false;
    //}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.GetCreatureSkill), new Type[] { typeof(MagicSchool) })]
    public static bool PreGetCreatureSkill(MagicSchool skill, ref Creature __instance, ref CreatureSkill __result)
    {
        //if (__instance is Player player) 
        //    player.SendMessage($"Casting {player?.CurrentSpell?.Name} - {player.LastSuccessCast_School}");
        //Debugger.Break();

        __result = skill switch
        {
            MagicSchool.CreatureEnchantment => __instance.GetCreatureSkill(Skill.CreatureEnchantment),
            MagicSchool.WarMagic => __instance.GetCreatureSkill(Skill.WarMagic),
            MagicSchool.ItemEnchantment => __instance.GetCreatureSkill(Skill.ItemEnchantment),
            MagicSchool.LifeMagic => __instance.GetCreatureSkill(Skill.LifeMagic),
            MagicSchool.VoidMagic => __instance.GetCreatureSkill(Skill.VoidMagic),
            MagicSchool x when x <= (MagicSchool)Skill.Summoning => __instance.GetCreatureSkill((Skill)x),
            _ => null,
        };

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.HasFoci), new Type[] { typeof(MagicSchool) })]
    public static bool PreHasFoci(MagicSchool school, ref Player __instance, ref bool __result)
    {
        __result = school switch
        {
            MagicSchool.WarMagic => __instance.AugmentationInfusedWarMagic > 0,
            MagicSchool.LifeMagic => __instance.AugmentationInfusedLifeMagic > 0,
            MagicSchool.ItemEnchantment => __instance.AugmentationInfusedItemMagic > 0,
            MagicSchool.CreatureEnchantment => __instance.AugmentationInfusedCreatureMagic > 0,
            MagicSchool.VoidMagic => __instance.AugmentationInfusedVoidMagic > 0,
            _ => false
        };

        if (Player.FociWCIDs.TryGetValue(school, out var wcid))
            __result = __instance.Inventory.Values.FirstOrDefault(i => i.WeenieClassId == wcid) != null;
        else __result = true;
        
        return false;
    }

}

