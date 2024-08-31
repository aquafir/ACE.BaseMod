

public abstract class IComp: IDisposable
{
    protected static uint _nextId = 0;
    protected uint _id;

    public string Label = "";

    /// <summary>
    /// Tracks whether any changes were made by the modal
    /// </summary>
    public bool Changed;

    public IComp()
    {
        _id = _nextId++;
        Init();
    }

    /// <summary>
    /// Render the Filter, returning the finished state, the opposite of Open, to match the ImGui pattern?
    /// </summary>
    public virtual bool Check()
    {
        try
        {
            Changed = false;

            DrawBody();
        }
        catch (Exception ex) { ModManager.Log($"{ex}"); }

        return Changed;
    }

    /// <summary>
    /// Draw the main content of the Filter
    /// </summary>
    public abstract void DrawBody();

    public virtual void Init() { }

    public virtual void Dispose() { }

}
