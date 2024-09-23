//using ACE.Adapter.GDLE.Models;
using ACE.DatLoader.Entity;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace Spells;

public static class SpellHelper
{
    private const char CD = ',';    //Column delimeter
    private const char SD = '\t';   //Spell ID delimeter

    //Comparable spells are the matches in everything but level.  e.g., Cold Bolt 1-8 are a group
    static Dictionary<uint, uint> _comparableSpells { get; set; } //= new();

    //Related spells are loose fits.  e.g., All offensive War
    static Dictionary<uint, uint> _relatedSpells { get; set; } //= new();

    //Groups use an index that points to their set of spell IDs
    static Dictionary<uint, List<uint>> _groups { get; set; } //= new();

    static string _groupPath = Path.Combine(Mod.ModPath, "Groups.csv");

    public static bool TryInitializeSpellGroups()
    {
        if (!File.Exists(PatchClass.Settings.PortalDatPath))
        {
            ModManager.Log($"Unable to create spell groups.  Missing portal dat.");
            return false;
        }

        //Initialize after reload
        _groups = new();
        _relatedSpells = new();
        _comparableSpells = new();

        //Todo: clean this up and verify
        try
        {
            //Load comparable map
            if (File.Exists(_groupPath) && PatchClass.Settings.LastGenerated == PatchClass.Settings.GroupType)
            {
                //Skip header, tab-separated list
                foreach (var line in File.ReadAllLines(_groupPath).Skip(1).Select(x => x.Split(CD)))
                {
                    //Groups have 2 columns, the others 
                    switch (line[2])
                    {
                        case "G":
                            if (!_groups.TryAdd(uint.Parse(line[0]), new List<uint>(line[1].Split(SD).Select(x => uint.Parse(x)))))
                                ModManager.Log($"Duplicate found: {line}");
                            break;
                        case "C":
                            if (!_comparableSpells.TryAdd(uint.Parse(line[0]), uint.Parse(line[1])))
                                ModManager.Log($"Duplicate found: {line}");
                            break;
                        case "R":
                            if (!_relatedSpells.TryAdd(uint.Parse(line[0]), uint.Parse(line[1])))
                                ModManager.Log($"Duplicate found: {line}");
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

                PatchClass.Settings.LastGenerated = PatchClass.Settings.GroupType;
                PatchClass.SaveSettings();
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

        //var spells = !PatchClass.Settings.OnlyPlayerSpells ?
        //    portalDat.SpellTable.Spells :
        //    //Restrict SpellTable to player spells.  PlayerSpellTable is sorted for binary search
        //    portalDat.SpellTable.Spells.Where(c => Array.BinarySearch(Player.PlayerSpellTable, c.Key) >= 0);

        //Just player spells
        var spells = new Dictionary<uint, SpellBase>();
        foreach (uint i in Player.PlayerSpellTable)
        {
            var spell = new Spell(i);
            var sBase = portalDat.SpellTable.Spells[i];
            if (spell.Flags.HasFlag(ACE.Entity.Enum.SpellFlags.Beneficial) && !spell.Flags.HasFlag(ACE.Entity.Enum.SpellFlags.SelfTargeted)
                //Require researchable for any < 6
                && (spell.Level > 6 || !spell.Flags.HasFlag(ACE.Entity.Enum.SpellFlags.NotResearchable))
                && (sBase.MetaSpellType == SpellType.Enchantment))
                spells.Add(i, sBase);
        }

        //var groups = spells.OrderBy(s => new Spell(s.Key).Level).GroupBy(x => new
        ////Create similar categories.  Might be a bit off
        //{
        //    MaskedFlags = x.Value.Bitfield & RELATED_MASK,            
        //    //x.Value.NonComponentTargetType,             //Blood Drinker vs Spirit Drinker
        //    x.Value.School,                             //Nether Blast same category as Flame Blast
        //                                                //Unique group if Portal<Sending|Recall|Summon|Link> and FellowPortalSending
        //    IsPortal = (x.Value.MetaSpellType.ToString().Contains("Portal") ? x.Key : 0)
        //});

        //Yonneh request
        var groups = spells.OrderBy(s => new Spell(s.Key).Level).GroupBy(x => new
        //Create similar categories.  Might be a bit off
        {
            //Split groups by level
            new Spell(x.Key).Level,
            //Split by buff type
            //IsBuff = x.Value.MetaSpellType == SpellType.Boost || x.Value.MetaSpellType == SpellType.Enchantment,
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

        //Header
        //sb.AppendLine($"Group\tID\tName\tBits\tLevel\t");
        //foreach (var g in groups)
        //{
        //    //sb.AppendLine($"Group {gNum++} ({g.Count()}):");
        //    foreach (var s in g)
        //    {
        //        sb.AppendLine($"{gNum++}\t{s.Key}\t{s.Value.Name}\t{(s.Value.Bitfield & RELATED_MASK):X8}\t{new Spell(s.Key).Level}");
        //        //sb.AppendLine($"  {s.Key}\t{s.Value.Name}\t{(s.Value.Bitfield & RELATED_MASK):X8}\t");
        //    }
        //}
        sb.AppendLine($"Group,ID,Name,Bits,Level");

        foreach (var g in groups)
        {
            //sb.AppendLine($"Group {gNum++} ({g.Count()}):");
            foreach (var s in g)
            {
                sb.AppendLine($"{gNum},{s.Key},{s.Value.Name},{(s.Value.Bitfield & RELATED_MASK):X8},{new Spell(s.Key).Level}");
                //sb.AppendLine($"  {s.Key},{s.Value.Name},{(s.Value.Bitfield & RELATED_MASK):X8},");
            }
            gNum++;
        }
        watch.Stop();
        //sb.Insert(0, $"{watch.ElapsedMilliseconds} ms\r\n\r\n");
        //File.WriteAllText(Path.Combine(Mod.ModPath, "Related Spell Dump.txt"), sb.ToString());
        File.WriteAllText(Path.Combine(Mod.ModPath, "Related Spell Dump.csv"), sb.ToString());
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

    const uint YONNEH_MASK = (uint)~(
        SpellFlags.CreatureSpell | SpellFlags.DamageOverTime | SpellFlags.ExcludedFromItemDescriptions | SpellFlags.FastCast
        | SpellFlags.FellowshipSpell | SpellFlags.IgnoresManaConversion | SpellFlags.IndoorLongRange | SpellFlags.NonTrackingProjectile
        | SpellFlags.NotIndoor | SpellFlags.NotOutdoor | SpellFlags.NotResearchable | SpellFlags.PKSensitive | SpellFlags.Projectile | SpellFlags.Resistable
        | SpellFlags.Reversed
        );
    #endregion




    //public static T DeepClone<T>(this T input) where T : ISerializable
    //{
    //    using (var stream = new MemoryStream())
    //    {
    //        var formatter = new BinaryFormatter();
    //        formatter.Serialize(stream, input);
    //        stream.Position = 0;
    //        return (T)formatter.Deserialize(stream);
    //    }
    //}


    public static byte[] ObjectToByteArray(object objectData)
    {
        if (objectData == null)
            return null;
        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, objectData);

        return memoryStream.ToArray();
    }

    public static object ByteArrayToObject(byte[] arrayBytes)
    {
        if (arrayBytes == null) return Encoding.UTF8.GetBytes(string.Empty);
        var memoryStream = new MemoryStream();
        var binaryFormatter = new BinaryFormatter();
        memoryStream.Write(arrayBytes, 0, arrayBytes.Length);
        memoryStream.Seek(0, SeekOrigin.Begin);
        var obj = binaryFormatter.Deserialize(memoryStream);
        return obj;
    }

    public static SpellBase DeepClone(this SpellBase spellBase)
    {
        //return spellBase.CloneJson<SpellBase>();
        //SpellBase clone = ByteArrayToObject(ObjectToByteArray(spellBase)) as SpellBase;

        var clone = new SpellBase();

        MemoryStream ms = new();
        BinaryWriter writer = new(ms);
        BinaryReader reader = new(ms);

        writer.WriteObfuscatedString(spellBase.Name);
        writer.WriteObfuscatedString(spellBase.Desc);

        writer.Write((uint)spellBase.School);
        writer.Write((uint)spellBase.Icon);
        writer.Write((uint)spellBase.Category);
        writer.Write((uint)spellBase.Bitfield);
        writer.Write((uint)spellBase.BaseMana);
        writer.Write((float)spellBase.BaseRangeConstant);
        writer.Write((float)spellBase.BaseRangeMod);
        writer.Write((uint)spellBase.Power);
        writer.Write((float)spellBase.SpellEconomyMod);
        writer.Write((uint)spellBase.FormulaVersion);
        writer.Write((float)spellBase.ComponentLoss);
        writer.Write((uint)spellBase.MetaSpellType);
        writer.Write((uint)spellBase.MetaSpellId);

        switch (spellBase.MetaSpellType)
        {
            case SpellType.Enchantment:
            case SpellType.FellowEnchantment:
                writer.Write((double)spellBase.Duration);
                writer.Write((float)spellBase.DegradeModifier);
                writer.Write((float)spellBase.DegradeLimit);
                break;
            case SpellType.PortalSummon:
                writer.Write((double)spellBase.PortalLifetime);
                break;
        }

        // TODO: Fix components
        for (int j = 0; j < 8; j++)
        {
            //if (j < spellBase.Formula.Count)
            //    writer.Write(spellBase.Formula[j]);
            //else
            writer.Write(0u);
        }

        writer.Write((uint)spellBase.CasterEffect);

        writer.Write((float)spellBase.CasterEffect);
        writer.Write((float)spellBase.TargetEffect);
        writer.Write((float)spellBase.FizzleEffect);
        writer.Write((double)spellBase.RecoveryInterval);
        writer.Write((float)spellBase.RecoveryAmount);
        writer.Write((uint)spellBase.DisplayOrder);
        writer.Write((uint)spellBase.NonComponentTargetType);
        writer.Write((uint)spellBase.ManaMod);

        ms.Position = 0;

        clone.Unpack(reader);

        return clone;
    }


    /// <summary>
    /// Writes a string the way SpellBase reads it
    /// </summary>
    private static void WriteObfuscatedString(this BinaryWriter writer, string spellBase)
    {
        writer.Write((UInt16)spellBase.Length);
        //System.Text.Encoding.GetEncoding(1252).GetString(spellBase.Name);
        for (var i = 0; i < spellBase.Length; i++)
        {
            // flip the bytes in the string to redo the obfuscation: i.e. 0xAB => 0xBA
            byte obfsByte = (byte)((spellBase[i] >> 4) | (spellBase[i] << 4));
            writer.Write(obfsByte);
        }

        // Aligns to the next DWORD boundary.
        writer.Align();
        //long alignDelta = writer.BaseStream.Position % 4;

        //for (var i = 0; i < writer.BaseStream.Position % 4; i++)
        //    writer.Write((byte)0);
    }

    /// <summary>
    /// Perform a deep Copy of the object, using Json as a serialization method. NOTE: Private members are not cloned using this method.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T CloneJson<T>(this T source)
    {
        // Don't serialize a null object, simply return the default for that object
        if (ReferenceEquals(source, null)) return default;

        // initialize inner objects individually
        // for example in default constructor some list property initialized with some values,
        // but in 'source' these items are cleaned -
        // without ObjectCreationHandling.Replace default constructor values will be added to result
        var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
    }

    public static T Clone<T>(this T source) //where T : ISerializable
    {
        var serialized = System.Text.Json.JsonSerializer.Serialize(source);
        return System.Text.Json.JsonSerializer.Deserialize<T>(serialized);
    }
}