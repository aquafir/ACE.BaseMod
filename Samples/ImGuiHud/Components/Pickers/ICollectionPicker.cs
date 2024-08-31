


public abstract class ICollectionPicker<T> : IPicker<T>
{
    //Debated about where to put this.  Some choices till be different, like string[] for Combos and mapped to the generic type
    /// <summary>
    /// Items being displayed and selected from
    /// </summary>
    public IEnumerable<T> Choices;
    //public T[] Choices;

    /// <summary>
    /// Currently selected set of items
    /// </summary>
    public System.Collections.Generic.HashSet<T> Selected;

    /// <summary>
    /// Controls how selecting items works
    /// </summary>
    public SelectionStyle Mode = SelectionStyle.Single;

    public override void Init()
    {
        Selected = new();
        base.Init();
    }

    public override void DrawBody()
    {
        //Index used for line breaks?
        int index = 0;

        foreach (var choice in Choices)
            DrawItem(choice, index++);

        if (Mode.HasFlag(SelectionStyle.Multiple))
            DrawSelectionControls();
    }

    public virtual void DrawSelectionControls()
    {
        if (ImGui.Button($"Clear##{_id}"))
            ClearSelection();
        ImGui.SameLine();
        if (ImGui.Button($"Select All##{_id}"))
        {
            Selection = Choices.FirstOrDefault();
            Selected = Choices.ToHashSet();
        }
        ImGui.SameLine();
        if (ImGui.Button($"Accept##{_id}"))
            Changed = true;
    }

    //public abstract void DrawItem(T item, int index);
    public virtual void DrawItem(T item, int index) { }

    /// <summary>
    /// Called when an item is interacted with
    /// </summary>
    public virtual void SelectItem(T item, int index)
    {
        //TODO ADD MULTI SELECTION?
        switch (Mode)
        {
            case SelectionStyle.Single:
                SelectOnly(item); //Selection = item;
                Changed = true;
                ModManager.Log($"Select {index} - {Selected.Count} - {Selection}");
                break;

            case SelectionStyle.Multiple:
                if (ImGui.IsKeyDown(ImGuiKey.ModShift))
                {
                    SelectRange(item);
                    Changed = false;
                    ModManager.Log($"Range {index} - {Selected.Count} - {Selection}");
                }
                else if (ImGui.IsKeyDown(ImGuiKey.ModCtrl))
                {
                    Toggle(item);
                    Changed = false;
                    ModManager.Log($"Toggle {index} - {Selected.Count} - {Selection}");
                }
                else
                {
                    SelectOnly(item); //Selection = item;
                    Changed = true;
                    ModManager.Log($"Select {index} - {Selected.Count} - {Selection}");
                }
                break;
        }
    }


    /// <summary>
    /// Toggles the presence of a Choice in Selected, setting Selection to the item if it is being added.
    /// Returns true if the item was added
    /// </summary>
    /// <param name="item"></param>
    public virtual bool Toggle(T item)
    {
        //If the item could be added
        if (Selected.Add(item))
        {
            ModManager.Log($"Successfully added {item}");
            Selection = item;
            return true;
        }
        else
        {
            ModManager.Log($"Removed {item}");
            Selected.Remove(item);
            if (item.Equals(Selection))
                Selection = default(T);

            return false;
        }
    }
    /// <summary>
    /// Select an individual item, clearing any already 
    /// </summary>
    public virtual void SelectOnly(T item)
    {
        Selected.Clear();
        Selected.Add(item);
        Selection = item;
    }
    /// <summary>
    /// Selects all elements between the already selected one (or the initial item) and the target one.  If all items are already selected, deselect them
    /// </summary>
    /// <param name="item"></param>
    public virtual void SelectRange(T item)
    {
        //Get index of already selected
        var list = Choices.ToList();
        var selectionIndex = Selection.Equals(default(T)) ? -1 : list.IndexOf(Selection);

        //Missing goes from start?
        if (selectionIndex < 0)
            selectionIndex = 0;

        //Get index of item being selected
        var targetIndex = list.IndexOf(item);


        var start = Math.Min(selectionIndex, targetIndex);
        var end = Math.Max(selectionIndex, targetIndex);
        var num = end - start + 1;
        var range = list.Skip(start).Take(num);

        ModManager.Log($"{start} to {end}: {range.Count()}");

        bool allSelected = range.All(x => Selected.Contains(x));
        if (allSelected)
        {
            Selection = default(T);
            ModManager.Log($"Remove");
            foreach (var e in range)
                Selected.Remove(e); //Remove(e);
        }
        else
        {
            ModManager.Log($"Add");
            foreach (var e in range)
                Selected.Add(e); //Add(e);
        }
    }

    public void ClearSelection()
    {
        Selection = default(T);
        Selected?.Clear();
    }
}



/// <summary>
/// Todo: think about this
/// </summary>
[Flags]
public enum SelectionStyle
{
    /// <summary>
    /// Displays with no selection
    /// </summary>
    None = 0x0,
    /// <summary>
    /// Any selection sets the picker to interacted with
    /// </summary>
    Single = 0x1,
    /// <summary>
    /// Adds shift/ctrl modifiers for selection
    /// </summary>
    Multiple = 0x2,

    //    Immediate = 0x4,
}
