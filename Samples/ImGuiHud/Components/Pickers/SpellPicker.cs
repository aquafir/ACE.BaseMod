//using ACE.DatLoader.FileTypes;


//public class SpellPicker : TexturedPicker<SpellInfo>
//{
//    static readonly SpellTable table = UBService.PortalDat.SpellTable;
//    static readonly SpellBook spellbook = G.Game.Character.SpellBook;

//    //Unfiltered set of choices
//    static readonly SpellInfo[] choices = table.Spells.Select(x => new SpellInfo(x.Key, x.Value)).ToArray();

//    public SpellFilter Filter = new() { Active = true, Label = "Filter Spells?" };

//    public void SetSpellFromId(uint id)
//    {
//        if (table.Spells.TryGetValue(id, out var spell))
//        {
//            Selection = new(id, spell);

//            //Update page?
//            var index = Choices.ToList().IndexOf(Selection);
//            if (index < 0)
//                return;

//            CurrentPage = index / PerPage;
//        }
//    }

//    public override void DrawBody()
//    {
//        if (Filter.Check())
//            Choices = Filter.Active ? Filter.GetFiltered(choices).ToArray() : choices;

//        base.DrawBody();
//    }

//    public override void DrawItem(SpellInfo item, int index)
//    {
//        base.DrawItem(item, index);

//        //Add in a tooltip
//        if (ImGui.IsItemHovered())
//        {
//            ImGui.BeginTooltip();
//            var id = item.Id;
//            var spell = item.Spell;
//            if (spellbook.TryGet(id, out var sd))
//            {
//                var skill = G.Game.Character.GetMagicSkill(spell.School);
//                ImGui.Text($"{spell.Name} ({id})\nChance: {SkillCheck.GetMagicSkillChance(skill, (int)spell.Power):P2}%\n{spell.School}\n{spell.Power}\n{spell.Desc}");
//            }

//            ImGui.EndTooltip();
//        }
//    }

//    public SpellPicker() : base(x => TextureManager.GetOrCreateTexture(x.Spell.Icon), choices) { }
//}
