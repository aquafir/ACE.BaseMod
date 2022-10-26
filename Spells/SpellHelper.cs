using ACE.DatLoader;
using ACE.DatLoader.FileTypes;
using ACE.Server.Entity;

namespace Spells;

public static class SpellHelper
{
    private const char CD = ',';    //Column delimeter
    private const char SD = '\t';   //Spell ID delimeter

    //Comparable spells are the matches in everything but level.  e.g., Cold Bolt 1-8 are a group
    static Dictionary<uint, uint> _comparableSpells { get; set; } = new();

    //Related spells are loose fits.  e.g., All offensive War
    static Dictionary<uint, uint> _relatedSpells { get; set; } = new();

    //Groups use an index that points to their set of spell IDs
    static Dictionary<uint, List<uint>> _groups { get; set; } = new();

    //static string _comparablePath = Path.Combine(Mod.ModPath, "Comparable.csv");
    //static string _relatedPath = Path.Combine(Mod.ModPath, "Related.csv");
    static string _groupPath = Path.Combine(Mod.ModPath, "Groups.csv");

    public static bool TryInitializeSpellGroups()
    {
        if (!File.Exists(PatchClass.Settings.PortalDatPath))
        {
            ModManager.Log($"Unable to create spell groups.  Missing portal dat.");
            return false;
        }

        //Todo: clean this up and verify
        try
        {
            //Load comparable map
            if (File.Exists(_groupPath))
            {
                //Skip header, tab-separated list
                foreach (var line in File.ReadAllLines(_groupPath).Skip(1).Select(x => x.Split(CD)))
                {
                    //Groups have 2 columns, the others 
                    switch (line[2])
                    {
                        case "G":
                            _groups.Add(uint.Parse(line[0]), new List<uint>(line[1].Split(SD).Select(x => uint.Parse(x))));
                            break;
                        case "C":
                            _comparableSpells.Add(uint.Parse(line[0]), uint.Parse(line[1]));
                            break;
                        case "R":
                            _relatedSpells.Add(uint.Parse(line[0]), uint.Parse(line[1]));
                            break;
                    }
                }
                ModManager.Log($"Loaded comparable spell groups.");
            }
            else
            {
                CreateComparableSpellGroups();
                ModManager.Log($"Created comparable spell groups.");

                CreateRelatedSpellGroups();
                ModManager.Log($"Created related spell groups.");

                var output = new StringBuilder("ID,Group,Type\r\n");
                output.AppendLine(string.Join(System.Environment.NewLine, _comparableSpells.OrderBy(x => x.Key).Select(kvp => $"{kvp.Key}{CD}{kvp.Value}{CD}C")));
                output.AppendLine(string.Join(System.Environment.NewLine, _relatedSpells.OrderBy(x => x.Key).Select(kvp => $"{kvp.Key}{CD}{kvp.Value}{CD}R")));
                output.AppendLine(string.Join(System.Environment.NewLine, _groups.OrderBy(x => x.Key).Select(kvp => $"{kvp.Key}{CD}{string.Join(SD, kvp.Value)}{CD}G")));

                File.WriteAllText(_groupPath, output.ToString());
            }
        }
        catch (Exception ex)
        {
            ModManager.Log($"Failed to load or create groups: {ex.Message}");
            return false;
        }

        return true;
    }



    private static void CreateComparableSpellGroups()
    {
        var portalDat = new PortalDatDatabase(PatchClass.Settings.PortalDatPath, false);
            
        var spells = !PatchClass.Settings.OnlyPlayerSpells ?
            portalDat.SpellTable.Spells :
            //Restrict SpellTable to player spells.  PlayerSpellTable is sorted for binary search
            portalDat.SpellTable.Spells.Where(c => Array.BinarySearch(Player.PlayerSpellTable, c.Key) >= 0);

        var groups = spells.OrderBy(s => new Spell(s.Key).Level).GroupBy(x => new
        //Create similar categories.  Might be a bit off
        {
            x.Value.Category,                           //Group by category
            //MaskedFlags = x.Value.GetComparedFlags(),
            MaskedFlags = x.Value.Bitfield & COMPARABLE_MASK,
            x.Value.MetaSpellType,                      //Healing Mastery vs Heal Self
            x.Value.NonComponentTargetType,             //Blood Drinker vs Spirit Drinker
            x.Value.School,                             //Nether Blast same category as Flame Blast
                                                        //Unique group if Portal<Sending|Recall|Summon|Link> and FellowPortalSending
            IsPortal = (x.Value.MetaSpellType.ToString().Contains("Portal") ? x.Key : 0)
        });

        uint groupIndex = (uint)_groups.Count;
        foreach (var g in groups)
        {
            //Point spell to its group
            foreach (var s in g)
                _comparableSpells.Add(s.Key, groupIndex);

            //Create the group
            _groups.Add(groupIndex++, g.Select(x => x.Key).ToList());
        }

        portalDat = null;

        #region Group Dump
        if (!PatchClass.Settings.DumpSpellGroups)
            return;

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        var sb = new StringBuilder();
        int gNum = 1;
        foreach (var g in groups)
        {
            sb.AppendLine($"Group {gNum++} ({g.Count()}):");
            foreach (var s in g)
            {
                sb.AppendLine($"  {s.Key}\t{s.Value.Name}\t{(s.Value.Bitfield & COMPARABLE_MASK):X8}\t");
            }
        }
        watch.Stop();
        sb.Insert(0, $"{watch.ElapsedMilliseconds} ms\r\n\r\n");
        File.WriteAllText(Path.Combine(Mod.ModPath, "Comparable Spell Dump.txt"), sb.ToString());
        #endregion Group Dump
    }

    private static void CreateRelatedSpellGroups()
    {
        var portalDat = new PortalDatDatabase(PatchClass.Settings.PortalDatPath, false);

        var spells = !PatchClass.Settings.OnlyPlayerSpells ?
            portalDat.SpellTable.Spells :
            //Restrict SpellTable to player spells.  PlayerSpellTable is sorted for binary search
            portalDat.SpellTable.Spells.Where(c => Array.BinarySearch(Player.PlayerSpellTable, c.Key) >= 0);

        var groups = spells.OrderBy(s => new Spell(s.Key).Level).GroupBy(x => new
        //Create similar categories.  Might be a bit off
        {
            MaskedFlags = x.Value.Bitfield & RELATED_MASK,
            //x.Value.NonComponentTargetType,             //Blood Drinker vs Spirit Drinker
            x.Value.School,                             //Nether Blast same category as Flame Blast
                                                        //Unique group if Portal<Sending|Recall|Summon|Link> and FellowPortalSending
            IsPortal = (x.Value.MetaSpellType.ToString().Contains("Portal") ? x.Key : 0)
        });

        uint groupIndex = (uint)_groups.Count;
        foreach (var g in groups)
        {
            //Point spell to its group
            foreach (var s in g)
                _relatedSpells.Add(s.Key, groupIndex);

            //Create the group
            _groups.Add(groupIndex++, g.Select(x => x.Key).ToList());
        }

        portalDat = null;

        #region Group Dump
        if (!PatchClass.Settings.DumpSpellGroups)
            return;

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        var sb = new StringBuilder();
        int gNum = 1;
        foreach (var g in groups)
        {
            sb.AppendLine($"Group {gNum++} ({g.Count()}):");
            foreach (var s in g)
            {
                sb.AppendLine($"  {s.Key}\t{s.Value.Name}\t{(s.Value.Bitfield & RELATED_MASK):X8}\t");
            }
        }
        watch.Stop();
        sb.Insert(0, $"{watch.ElapsedMilliseconds} ms\r\n\r\n");
        File.WriteAllText(Path.Combine(Mod.ModPath, "Related Spell Dump.txt"), sb.ToString());
        #endregion Group Dump
    }

    //Reference: LearnSpellsInBulk
    public static List<uint> GetComparableSpells(this Spell spell) => GetComparableSpells(spell.Id);
    public static List<uint> GetComparableSpells(uint id)
    {
        //Look for ID-->Index and Index-->Group
        if (_comparableSpells.TryGetValue(id, out var groupId) &&
            _groups.TryGetValue(groupId, out var group))
            return group;

        //Default is list of itself
        return new List<uint> { id };
    }

    public static List<uint> GetRelatedSpells(this Spell spell) => GetRelatedSpells(spell.Id);
    public static List<uint> GetRelatedSpells(uint id)
    {
        //Look for ID-->Index and Index-->Group
        if (_relatedSpells.TryGetValue(id, out var groupId) &&
            _groups.TryGetValue(groupId, out var group))
            return group;

        //Default is list of itself
        return new List<uint> { id };
    }

    #region Enums and Constants
    //ACE enum as uint for use with bitmask
    public enum SpellFlags : uint
    {
        Resistable = 0x1, PKSensitive = 0x2, Beneficial = 0x4, SelfTargeted = 0x8,
        Reversed = 0x10, NotIndoor = 0x20, NotOutdoor = 0x40, NotResearchable = 0x80,
        Projectile = 0x100, CreatureSpell = 0x200, ExcludedFromItemDescriptions = 0x400, IgnoresManaConversion = 0x800,
        NonTrackingProjectile = 0x1000, FellowshipSpell = 0x2000, FastCast = 0x4000, IndoorLongRange = 0x8000,
        DamageOverTime = 0x10000, UNKNOWN = 0x20000
    }

    //Masks ignore certain spell flags
    const uint COMPARABLE_MASK = (uint)~(
        SpellFlags.NotResearchable |    //Ignore fastcast/researchable for 7-8s
        SpellFlags.FastCast |
        SpellFlags.PKSensitive);        //Strength Self 1-6 diff PKSensitive than 7-8

    const uint RELATED_MASK = (uint)~(
        SpellFlags.Resistable |
        SpellFlags.PKSensitive |        //Strength Self 1-6 diff PKSensitive than 7-8
        SpellFlags.NotResearchable |    //Ignore fastcast/researchable for 7-8s
        SpellFlags.ExcludedFromItemDescriptions |
        SpellFlags.IgnoresManaConversion |
        SpellFlags.FellowshipSpell |
        SpellFlags.FastCast
        );
    #endregion
}
