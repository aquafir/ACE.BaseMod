namespace Expansion.Enums;

//Just used for convenience.  string->Odds dictionary used
public enum OddsGroup
{
    Common,
    Rare,
    Always,
}

public static class OddsHelper
{
    public static string? DefaultOdds(this Mutation mutator) => mutator switch
    {
        Mutation.Slayer => nameof(OddsGroup.Rare),
        _ => null,
    };
    public static Odds OddsOf(this OddsGroup type) => type switch
    {
        OddsGroup.Common => new()
        {
            TierChance = new()
            {
                [1] = .04f,
                [2] = .07f,
                [3] = .1f,
                [4] = .13f,
                [5] = .16f,
                [6] = .19f,
                [7] = .22f,
                [8] = .25f,
            }
        },
        OddsGroup.Rare => new()
        {
            TierChance = new()
            {
                [7] = .01f,
                [8] = .03f,
            }
        },
        OddsGroup.Always => new()
        {
            TierChance = new()
            {
                [0] = 1f,
            }
        },
        _ => new(),
    };
}

