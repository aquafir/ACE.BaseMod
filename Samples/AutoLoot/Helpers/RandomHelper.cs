using AutoLoot.Loot;
using Action = AutoLoot.Loot.Action;

namespace AutoLoot.Helpers;

public static class RandomHelper
{
    static PropertyString[] stringEnum = new[]
    {
        PropertyString.Name,
        //PropertyString.LongDesc
    };  //Enum.GetValues<PropertyString>();

    /// <summary>
    /// Create a random profile with descriptive names
    /// </summary>
    public static Profile RandomProfile(int rules, int requirements = 1, bool valReqs = true, bool stringReqs = true)
    {
        Profile profile = new();

        var valEnum = Enum.GetValues<ValueProp>();
        var compareEnum = Enum.GetValues<CompareType>();
        var actionEnum = Enum.GetValues<Action>();

        for (var i = 0; i < rules; i++)
        {
            //Create requirements
            List<StringRequirement> sReqs = new();
            List<ValueRequirement> vReqs = new();
            for (var j = 0; j < requirements; j++)
            {
                if (valReqs)
                    vReqs.Add(new ValueRequirement()
                    {
                        PropKey = ThreadSafeRandom.Next(0, 200),
                        TargetValue = ThreadSafeRandom.Next(0, 200),
                        PropType = valEnum.Random(),
                        Type = compareEnum.Random(),
                    });

                if (stringReqs)
                {
                    StringRequirement sReq = new()
                    {
                        Prop = stringEnum.Random(),
                        Value = randomWords.Random(),
                    };
                    //sReq.Initialize(); 
                    sReqs.Add(sReq);
                }
            }

            Rule rule = new()
            {
                ValueReqs = vReqs,
                StringReqs = sReqs,
                Action = actionEnum.Random(),
            };

            if (valReqs && vReqs.Count > 0)
            {
                var r = vReqs.FirstOrDefault();
                rule.Name = $"VRule {r.PropType} {r.Type.Friendly()} {r.TargetValue} --> {rule.Action}";
            }
            else if (stringReqs && sReqs.Count > 0)
            {
                var r = sReqs.FirstOrDefault();
                rule.Name = $"SRule {r.Prop} {r.Value} --> {rule.Action}";
            }

            profile.Rules.Add(rule);
        }

        return profile;
    }

    public static string Friendly(this CompareType type) => type switch
    {
        CompareType.GreaterThan => ">",
        CompareType.LessThanEqual => "<=",
        CompareType.LessThan => "<",
        CompareType.GreaterThanEqual => ">=",
        CompareType.NotEqual => "!=",
        CompareType.NotEqualNotExist => "!=??",
        CompareType.Equal => "==",
        CompareType.NotExist => "??",
        CompareType.Exist => "?",
        CompareType.NotHasBits => "!B",
        CompareType.HasBits => "B",
        _ => "",
    };

    //Load words
    static string[] _words;
    static string[] randomWords
    {
        get
        {
            if (_words is null) _words = File.ReadAllLines(Path.Combine(Mod.Instance.ModPath, "words.txt"));
            return _words;
        }
    }
}