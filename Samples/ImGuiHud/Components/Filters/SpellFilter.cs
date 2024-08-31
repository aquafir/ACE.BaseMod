//using ACE.DatLoader.Entity;
//using ACE.DatLoader.FileTypes;
//using SpellBook = UtilityBelt.Scripting.Interop.SpellBook;




//public class SpellFilter : IOptionalFilter<SpellInfo>
//{
//    static readonly SpellTable table = UBService.PortalDat.SpellTable;
//    static readonly SpellBook spellbook = G.Game.Character.SpellBook;

//    FilterSet<SpellInfo> filters;

//    public bool UseKnown = false;

//    public bool UseCastable = false;
//    public float CastChance = 1f;

//    public override void Init()
//    {
//        filters = new(new()
//        {
//            new SpellLevelFilter() {Label = "Level"},
//            new SpellSchoolFilter() { Label = "School" },
//            new RegexFilter<SpellInfo>(x => x.Spell.Name) {Active = true, Label = "Name" },
//        });
//        filters.Label = "FilterSet";

//        base.Init();
//    }

//    public override void DrawBody()
//    {
//        ImGui.NewLine();

//        if (filters.Check())
//            Changed = true;

//        if (ImGui.Checkbox("Known", ref UseKnown))
//            Changed = true;

//        if (ImGui.Checkbox("Success", ref UseCastable))
//            Changed = true;

//        if (UseCastable)
//        {
//            if (ImGui.SliderFloat("Chance", ref CastChance, 0, 1, $"{CastChance:P2}%"))
//                Changed = true;
//        }
//    }

//    /// <summary>
//    /// Returns a filtered list of spells using UBs list
//    /// </summary>
//    public SpellInfo[] GetFilteredSpells() => GetFiltered(table.Spells.Select(x => new SpellInfo(x.Key, x.Value))).ToArray();

//    public override bool IsFiltered(SpellInfo spellInfo)
//    {
//        if (!Active)
//            return false;

//        var spell = spellInfo.Spell;
//        var id = spellInfo.Id;

//        if (UseKnown && !spellbook.IsKnown(id))
//            return true;

//        if (UseCastable)
//        {
//            int skill = G.Game.Character.GetMagicSkill(spell.School);
//            var chance = SkillCheck.GetMagicSkillChance(skill, (int)spell.Power) + .001f;

//            if (chance < CastChance)
//                return true;
//        }

//        return filters.IsFiltered(spellInfo);
//    }
//}

//public record struct SpellInfo(uint Id, SpellBase Spell);