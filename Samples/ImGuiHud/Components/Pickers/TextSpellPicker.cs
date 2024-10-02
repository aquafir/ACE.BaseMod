public class TextSpellPicker : IPagedPicker<Spell>
{
    protected string[] filteredChoices = { };
    protected RegexFilter<string> RegexFilter;
    static Vector2 size = new(60, 20);
    int columns => 3;

    public TextSpellPicker() : base()
    {
        Choices = Enum.GetValues<SpellId>().Skip(1).Where(x => !x.ToString().StartsWith("UNK")).Select(x => new Spell(x)).ToArray();
    }

    //public override void DrawBody()
    //{
    //    //Todo: Generic combobox picker ?
    //    DrawPageControls();
    //    //base.DrawBody();
    //    //Don't think arrays are LINQ optimized so not using those methods
    //    //https://stackoverflow.com/questions/26685234/are-linqs-skip-and-take-optimized-for-arrays-4-0-edition#26685395

    //    for (var i = 0; i < PerPage; i++)
    //    {
    //        var current = i + offset;
    //        if (current >= ChoiceArray.Length)
    //            break;

    //        var choice = ChoiceArray[current];

    //        DrawItem(choice, i);
    //    }
    //}

    public override void DrawItem(Spell item, int index)
    {
        //ImGui.Combo("Spells", ref this.Selection)

        if (item is null)
            return;

        //if (index % columns != 0)
        //    ImGui.SameLine();

        if (ImGui.Button($"{item?.Name}##{index}", size))
            SelectItem(item, index);

        //var icon = textureMap(item);
        //Vector4 bg = new(0);
        //Vector4 tint = new(1);

        ////Style based on whether select / in group / neither
        //int borderSize = 2;

        //if (Selection is not null && Selection.Equals(item))
        //{
        //    bg = SELECTED_BACKGROUND;
        //    //tint = SELECTED_TINT;
        //}
        //else if (Selected.Contains(item))
        //{
        //    bg = SELECTED_GROUP_BACKGROUND;
        //    //tint = SELECTED_GROUP_TINT;
        //}

        ////if (ImGui.TextureButton($"{Name}{index}", icon, IconSize, borderSize, bg))//, tint))
        //if (ImGui.ImageButton($"{Name}{index}", icon.TexturePtr, IconSize))//, borderSize, bg))//, tint))
        //    SelectItem(item, index);
    }
}
