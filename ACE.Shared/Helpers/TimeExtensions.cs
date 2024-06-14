namespace ACE.Shared.Helpers;

public static class TimeExtensions
{
    //Gets previous time in game using a more accurate method than the ~7 second update of Age
    //Todo: support missing FakeProp?
    public static double PreviousTimeInGame(this Player player) => player.GetProperty(FakeFloat.TimeInGame) ?? player.Age.GetValueOrDefault();
    public static double TimeInGameFromLogin(this Player player) => Time.GetUnixTime() - (player.GetProperty(PropertyFloat.LoginTimestamp) ?? 0);
    public static double TotalTimeInGame(this Player player) => player.PreviousTimeInGame() + player.TimeInGameFromLogin();

    public static TimeSpan ToTimeSpan(this double timestamp) => TimeSpan.FromSeconds(timestamp);
    public static string ToFriendlyTime(this double timestamp) => TimeSpan.FromSeconds(timestamp).GetFriendlyString();
}
