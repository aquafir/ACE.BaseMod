namespace Tower;
public class PVPSettings
{
    public uint EarId { get; set; } = 12253;
    public int MaxLevelDifference { get; set; } = 15;

    //Placeholder, a DID to store the killed player's ID
    public PropertyDataId EarSourcePlayerProp { get; set; } = (PropertyDataId)44998;

    public PropertyFloat LastEarDropProp { get; set; } = (PropertyFloat)44997;
    public double SecondsBetweenDrops { get; set; } = TimeSpan.FromMinutes(15).TotalSeconds;

    public double SecondsBetweenPk { get; set; } = TimeSpan.FromMinutes(5).TotalSeconds;
    //public PropertyInt64 LastPkTimestamp { get; set; } = (PropertyInt64)44996;
}

