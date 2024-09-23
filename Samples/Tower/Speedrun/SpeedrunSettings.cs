namespace Tower;
public class SpeedrunSettings
{
    //Tracks PropInt/Float for level first completed and fastest time
    public int FirstCompletionRangeStart { get; set; } = 56000;
    //Tracks best time
    public int PersonalBestRangeStart { get; set; } = 57000;

    //PropInt for started floor
    public int CurrentFloor => 55999;
    //PropFloat for time floor started
    public int CurrentFloorStartTimestamp => 55999;

}
