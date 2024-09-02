

public abstract class IPagedPicker<T> : ICollectionPicker<T>  //where T
{
    //public uint Index;
    public int CurrentPage;
    public int PerPage = 20;

    //Todo: think about this?  Automatically cast if it isn't an array?
    public T[] ChoiceArray => Choices is T[] ca ? ca : Choices.ToArray(); //Choices as T[];

    public int Pages => Choices is null ? 0 : (int)(ChoiceArray.Length / PerPage);
    protected int offset => CurrentPage * PerPage;

    public virtual void DrawPageControls()
    {
        ImGui.SliderInt($"##{_id}PC", ref CurrentPage, 0, Pages, $"{CurrentPage}/{Pages}");
        ImGui.SameLine();
        if (ImGui.ArrowButton($"{_id}U", ImGuiDir.Left))
            CyclePage(-1);
        ImGui.SameLine();
        if (ImGui.ArrowButton($"{_id}D", ImGuiDir.Right))
            CyclePage(1);
    }

    public override void DrawBody()
    {
        DrawPageControls();

        //Don't think arrays are LINQ optimized so not using those methods
        //https://stackoverflow.com/questions/26685234/are-linqs-skip-and-take-optimized-for-arrays-4-0-edition#26685395
        for (var i = 0; i < PerPage; i++)
        {
            var current = i + offset;
            if (current >= ChoiceArray.Length)
                break;

            var choice = ChoiceArray[current];

            DrawItem(choice, i);
        }
    }

    /// <summary>
    /// Try to get the elements for a given page
    /// </summary>
    public IEnumerable<T> GetPage(int page) => Choices.Skip(offset).Take(PerPage);

    public void CycleSelection(int offset, bool defaultWithoutSelection = true)
    {
        //Check for valid page
        var page = GetPage(CurrentPage).ToList();
        if (page is null || page.Count == 0)
            return;

        //Find selection / optionally use a default
        var index = -1;
        if (Selection is null && defaultWithoutSelection)
            index = offset < 0 ? page.Count : page.Count - 1;
        else
            index = page.IndexOf(Selection);

        ModManager.Log($"Index: {Selection} {index} / {page.Count}");
        //Check for a valid index
        if (index == -1)
            return;

        //Grab the next offset
        var p = page.Count;
        var next = ((index + offset) % p + p) % p;

        SelectOnly(page[next]); //Selection = page[next];
    }

    /// <summary>
    /// Cycle through the pages by the offset
    /// </summary>
    public void CyclePage(int offset)
    {
        Selection = default(T);
        Selected.Clear();

        var p = Pages + 1;
        CurrentPage = ((CurrentPage + offset) % p + p) % p;
    }
}