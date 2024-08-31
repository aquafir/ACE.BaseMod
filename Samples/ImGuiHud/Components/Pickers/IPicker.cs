
public abstract class IPicker<T> : IComp
{
    /// <summary>
    /// The result of a selection
    /// </summary>
    public T Selection;

    /// <summary>
    /// Label used for the component
    /// </summary>
    public string Name => $"{Label}###{_id}";
}