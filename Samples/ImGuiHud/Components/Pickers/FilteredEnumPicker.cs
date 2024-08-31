

public class FilteredEnumPicker<T> : EnumPicker<T> where T : struct, Enum
{
    protected string[] filteredChoices = { };

    protected RegexFilter<string> RegexFilter;

    public override void Init()
    {
        choices = Enum.GetNames(typeof(T));
        filteredChoices = choices.ToArray();

        RegexFilter = new(x => x);

        base.Init();
    }

    public override void DrawBody()
    {
        if (RegexFilter.Check())
        {
            filteredChoices = RegexFilter.GetFiltered(choices).ToArray();
            Changed = true;
        }

        ImGui.SetNextItemWidth(Width);
        if (ImGui.Combo(Name, ref index, filteredChoices, filteredChoices.Length, ItemsShown))
            Changed = true;

        //Parse enum if there's an update, ignore invalid choices?
        if (Changed && index > 0 && index < filteredChoices.Length)
            Changed = Enum.TryParse(filteredChoices[index], out Selection);
    }
}


public class FilteredEnumPicker : EnumPicker
{
    protected string[] filteredChoices = { };

    protected RegexFilter<string> RegexFilter;

    public FilteredEnumPicker(Type type) : base(type)
    {
        //Parent populates `choices`
        filteredChoices = choices.ToArray();
        RegexFilter = new(x => x);
    }

    public override void DrawBody()
    {
        if (RegexFilter.Check())
        {
            filteredChoices = RegexFilter.GetFiltered(choices).ToArray();
            Changed = true;
        }

        ImGui.SetNextItemWidth(Width);

        DrawParseCombo(filteredChoices);
    }
}
