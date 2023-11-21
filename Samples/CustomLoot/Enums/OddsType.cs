namespace CustomLoot.Enums;

//Just used for convenience.  string->Odds dictionary used
public enum OddsType
{
    Common,
    Rare,
    Always,
}

public static class OddsHelper
{
    public static Odds OddsOf(this OddsType type) => type switch
    {
        OddsType.Common => new()
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
        OddsType.Rare => new()
        {
            TierChance = new()
            {
                [7] = .01f,
                [8] = .03f,
            }
        },
        OddsType.Always => new()
        {
            TierChance = new()
            {
                [1] = 1f,
                [2] = 1f,
                [3] = 1f,
                [4] = 1f,
                [5] = 1f,
                [6] = 1f,
                [7] = 1f,
                [8] = 1f,
            }
        },
        _ => new(),
    };
}

