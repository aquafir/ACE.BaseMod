
public abstract class IOptionalFilter<T>(Func<T, bool> filterPredicate = null) : IFilter<T>(filterPredicate)
{
    /// <summary>
    /// Controls whether the body of the optional filter is drawn when inactive 
    /// </summary>
    public bool ShowInactive = false;

    /// <summary>
    /// Determines if the filter is applied
    /// </summary>
    public bool Active;

    public virtual void DrawToggle()
    {
        if (ImGui.Checkbox($"{Label}##{_id}", ref Active))
            Changed = true;

        if (Active || ShowInactive)
            ImGui.SameLine();
    }

    /// <summary>
    /// Render the OptionalFilter
    /// </summary>
    public override bool Check()
    {
        try
        {
            Changed = false;

            DrawToggle();

            if (Active || ShowInactive)
                DrawBody();
        }
        catch (Exception ex) { ModManager.Log($"{ex}"); }

        return Changed;
    }

}
