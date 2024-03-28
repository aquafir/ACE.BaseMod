namespace AutoLoot.Loot;

public class StringRequirement
{
    //Todo: think about how to make it readonly?
    Regex _regex { get; set; }
    public PropertyString Prop { get; set; }
    public string Value { get; set; } = "";

    /// <summary>
    /// Regex matches the pattern of the requirement on the value of the WorldItem's specified property
    /// </summary>
    public bool VerifyRequirement(WorldObject item)
    {
        //if(_regex is null) return false;

        //Get the string value
        var itemValue = item.GetProperty(Prop);

        if (itemValue is null)
            return false;

        return _regex.IsMatch(itemValue);
    }

    /// <summary>
    /// Initialize Regex for a string matching
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Initialize()
    {
        _regex = new Regex(Value, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
