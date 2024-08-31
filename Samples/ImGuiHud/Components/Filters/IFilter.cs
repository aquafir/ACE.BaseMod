
public abstract class IFilter<T>(Func<T, bool> filterPredicate = null) : IComp
{
    /// <summary>
    /// Render the Filter, returning the finished state, the opposite of Open, to match the ImGui pattern?
    /// </summary>
    public override bool Check()
    {
        try
        {
            Changed = false;
            DrawBody();
        }
        catch (Exception ex) { ModManager.Log($"{ex}"); }

        return Changed;
    }

    public virtual bool IsFiltered(T item) => filterPredicate(item);

    public virtual IEnumerable<T> GetFiltered(IEnumerable<T> input)
    {
        if (input is null)
            return null;

        return input.Where(item => !IsFiltered(item));
    }
}
