namespace AutoLoot.Loot;

public class Rule
{
    public string Name { get; set; } = "";
    public int Priority { get; set; } = 0;              //Order evaluated.  Maybe
    public bool Enabled { get; set; } = true;           //In case a UI wants to be able to toggle rules
    public Action Action { get; set; } = Action.Keep;

    public List<ValueRequirement> ValueReqs { get; set; } = new();
    public List<StringRequirement> StringReqs { get; set; } = new();


    // Rules are responsible for determining if an item satisfies their requirements.
    // Todo: logic for how to order the evaluation
    public bool SatisfiesRequirements(WorldObject item)
    {
        //Check ValReqs
        if (!SatisfiesValueRequirements(item))
            return false;

        //Check StringReqs
        if (!SatisfiesStringRequirements(item))
            return false;

        //Todo: Check others...

        return true;
    }


    public bool SatisfiesValueRequirements(WorldObject item)
    {
        foreach (var req in ValueReqs)
        {
            if (!req.VerifyRequirement(item))
                return false;
        }

        return true;
    }


    public bool SatisfiesStringRequirements(WorldObject item)
    {
        foreach (var req in StringReqs)
        {
            if (!req.VerifyRequirement(item))
                return false;
        }

        return true;
    }


    /// <summary>
    /// Initialize Rule requirements
    /// </summary>
    public void Initialize()
    {
        foreach (var req in StringReqs)
            req.Initialize();
    }
}
