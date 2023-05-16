namespace Balance.Patches;

public interface IPatchable
{
    /// <summary>
    /// Manually add patches
    /// </summary>
    public void Start();
    /// <summary>
    /// Clean up and remove patches
    /// </summary>
    public void Shutdown();
}
