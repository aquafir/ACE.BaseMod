namespace Raise
{
    public class RaiseHistory
    {
        public Dictionary<string, Dictionary<RaiseTarget, int>> Raises { get; set; } = new();
    }
}
