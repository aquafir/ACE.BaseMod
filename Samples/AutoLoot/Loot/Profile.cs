namespace AutoLoot.Loot;
public class Profile
{
    public List<Rule> Rules { get; set; } = new();

    //Test profile
    public static Profile SampleProfile = new()
    {
        Rules = new()
             {
                 new () {
                     Name = "Characters",
                     Action = Action.Salvage,
                     StringReqs = new()
                     {
                         new()
                         {
                              Prop = PropertyString.Name,
                              Value = ".{10}",
                         },
                     },
                 },
                 new () {
                     Name = "Axe",
                     Action = Action.Salvage,
                     StringReqs = new()
                     {
                         new()
                         {
                              Prop = PropertyString.Name,
                              Value = "Axe|Ono|Silifi|Tewhate|Hammer",
                         },
                     },
                 },
                 new () {
                     Name = "Name is Cloak/Ends with Necklace",
                     Action = Action.Keep,
                     StringReqs = new()
                     {
                         new()
                         {
                              Prop = PropertyString.Name,
                              Value = "Cloak|necklace$",
                         },
                     },
                 },
                new () {
                    Name = "Armor Over 140",
                    Action = Action.Keep,
                    ValueReqs = new()
                    {
                        new()
                        {
                            Type = CompareType.GreaterThanEqual,
                            PropType = ValueProp.PropertyInt,
                            PropKey = (int)PropertyInt.ArmorLevel,
                            TargetValue = 140,
                        }
                    },
                 }
             }
    };

    /// <summary>
    /// Returns the Action of the first Rule in a Profile whose requirements are satisfied by a WorldObject
    /// </summary>
    public Action Evaluate(WorldObject item, ref Rule match)
    {
        //var sb = new StringBuilder($"Loot debug for {item.Name}:");
        foreach (var rule in Rules)
        {
            //Debugger.Break();
            if (!rule.SatisfiesRequirements(item))
                continue;

            //Successful matches stop evaluation
            match = rule;
            return rule.Action;
        }

        //Default failure
        return Action.None;
    }

    /// <summary>
    /// Set up whatever might be needed with the rules
    /// </summary>
    public void Initialize()
    {
        foreach (var rule in Rules)
            rule.Initialize();
    }
}
