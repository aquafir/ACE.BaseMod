using ACE.Common;
using ACE.Server.Network.GameAction.Actions;

namespace Tower;
[HarmonyPatch]

public static class SpeedRun
{
    public static double PreviousTimeInGame(this Player player) => player.TotalTimeInGame() - player.TimeInGameFromLogin();
    public static double TimeInGameFromLogin(this Player player) => Time.GetUnixTime() - (player.GetProperty(PropertyFloat.LoginTimestamp) ?? 0);
    public static double TotalTimeInGame(this Player player) => player.Age ?? 0;
    //public static TimeSpan TotalTimeInGameAsTimeSpan(this Player player) => TimeSpan.FromSeconds(player.TotalTimeInGame());

    public static bool TryGetTimeFromLastFloorStarted(this Player player, out TimeSpan time)
    {
        time = default(TimeSpan);

        if (!player.TryGetChallengedFloor(out var floor, out var start))
            return false;

        time = player.TotalTimeInGame().ToTimeSpan() - start;
        return true;
    }

    public static int FirstCompletionRangeStart { get; set; } = 56000;
    public static int PersonalBestRangeStart { get; set; } = 57000;
    public static PropertyFloat FirstCompletionProp(this TowerFloor floor) => (PropertyFloat)(floor.Index + FirstCompletionRangeStart);
    public static PropertyFloat PersonalBestProp(this TowerFloor floor) => (PropertyFloat)(floor.Index + PersonalBestRangeStart);

    public static TimeSpan? GetPersonalBest(this Player player, TowerFloor floor)
        => player.GetProperty(floor.PersonalBestProp())?.ToTimeSpan();
    public static void SetPersonalBest(this Player player, TowerFloor floor, double time)
        => player.SetProperty(floor.PersonalBestProp(), time);

    public static TimeSpan? GetFirstCompletion(this Player player, TowerFloor floor)
        => player.GetProperty(floor.FirstCompletionProp())?.ToTimeSpan();
    public static void SetFirstCompletion(this Player player, TowerFloor floor, double time)
        => player.SetProperty(floor.FirstCompletionProp(), time);

    public static PropertyInt CurrentFloor => (PropertyInt)55999;
    public static PropertyFloat CurrentFloorStartTimestamp => (PropertyFloat)55999;
    public static bool TryGetChallengedFloor(this Player player, out TowerFloor floor, out TimeSpan startTime)
    {
        floor = null;
        startTime = (player.GetProperty(CurrentFloorStartTimestamp) ?? -1).ToTimeSpan();

        var index = player.GetProperty(CurrentFloor);
        if (index is null)
            return false;

        return FloorExtensions.TryGetFloorByIndex(index.Value, out floor);
    }
    public static void SetChallengedFloor(this Player player, TowerFloor floor)
    {
        player.SetProperty(CurrentFloor, floor.Index);
        player.SetProperty(CurrentFloorStartTimestamp, player.TotalTimeInGame());

        var time = Time.GetUnixTime();
        player.SendMessage($"Starting {floor.Name} ({floor.Index}) at {Time.GetDateTimeFromTimestamp(time)}");
    }


    /// <summary>
    /// Handles checking if the current floor the player is on completes the last one they were on
    /// </summary>
    public static void CheckCompleteFloor(this Player player)
    {
        //Check if on a tower floor
        if (!player.TryGetFloor(out var currentFloor))
            return;

        //If the player has a known floor they were previously on...
        if (player.TryGetChallengedFloor(out var challengedFloor, out var challengedFloorStart))
        {
            //Skip if the last known floor is the one the player is currently on 
            if (challengedFloor.Landblock == currentFloor.Landblock)
                return;

            //Check if the current floor is the one that would follow / complete it
            if (challengedFloor.TryGetNextFloor(out var nextFloor) && nextFloor.Landblock == currentFloor.Landblock)
            {
                if(!player.TryGetTimeFromLastFloorStarted(out var timeCurrent))
                {
                    player.SendMessage($"For some reason the start of your last floor is unknown. Report to blode");
                }
                else
                {
                    //Check for first completion
                    var timeFirst = player.GetFirstCompletion(challengedFloor);
                    if (timeFirst is null)
                    {
                        player.SendMessage($"Congratulations!  You first finished floor {challengedFloor.Name} after {player.TotalTimeInGame().ToTimeSpan().GetFriendlyString()}");
                        player.SetFirstCompletion(challengedFloor, player.TotalTimeInGame());
                    }

                    //Check for speed from level start
                    //Debugger.Break();
                    var timeBest = player.GetPersonalBest(challengedFloor);
                    if (timeBest is null || timeBest > timeCurrent)
                    {
                        player.SendMessage($"Congratulations!  New personal best on {challengedFloor.Name} @ {timeCurrent.GetFriendlyString()}");
                        player.SetPersonalBest(challengedFloor, timeCurrent.TotalSeconds);
                    }
                }
            }
        }

        //Set current floor and start time
        player.SetChallengedFloor(currentFloor);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Player), nameof(Player.Teleport), new Type[] { typeof(Position), typeof(bool) })]
    public static void PostTeleport(Position _newPosition, bool fromPortal, ref Player __instance)
    {
        //Ignore logging back in
        //if (!fromPortal)
        //    return;

        //Check for completion
        __instance.CheckCompleteFloor();
    }


    [CommandHandler("clearspeed", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleClearSpeed(Session session, params string[] parameters)
    {
        var player = session.Player;

        foreach (var floor in PatchClass.Settings.Floors.OrderBy(x => x.Level))
        {
            player.RemoveProperty(floor.FirstCompletionProp());
            player.RemoveProperty(floor.PersonalBestProp());
        }

        player.Age = 0;
        player.SendMessage($"Cleared speeds and reset age.");
    }

    [CommandHandler("speed", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleSpeed(Session session, params string[] parameters)
    {
        var player = session.Player;

        var prev = player.PreviousTimeInGame();
        var tot = player.TotalTimeInGame();

        var sb = new StringBuilder();
        foreach (var floor in PatchClass.Settings.Floors.OrderBy(x => x.Level))
        {
            //sb.Append($"{floor.Index,-3}{floor.Name} @ {floor.Landblock:X4} Level {floor.Level}\n");
            sb.Append($"{floor.Name} @ {floor.Landblock:X4} Level {floor.Level}\n");
            var pb = player.GetPersonalBest(floor);
            var first = player.GetFirstCompletion(floor);
            sb.Append($"  First: {(first is null ? "n/a" : $"{first}")}  --  Best: {(pb is null ? "n/a" : $"{pb}")}\n");
        }

        player.SendMessage($"{sb}");
    }


    [CommandHandler("tig", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleTimeInGame(Session session, params string[] parameters)
    {
        var player = session.Player;

        var prev = player.PreviousTimeInGame();
        var tot = player.TotalTimeInGame();
        player.SendMessage($"Previous time: {prev}\nTotal time: {tot}");
    }
}

//ACE.Common.Extensions
public static class TimeSpanExtensions
{
    public static TimeSpan ToTimeSpan(this double timestamp) => TimeSpan.FromSeconds(timestamp);

    //public static string GetFriendlyString(this TimeSpan timeSpan)
    //{
    //    // use datetime here?
    //    // this probably won't work with numDays...
    //    //var numYears = timeSpan.GetYears();
    //    //var numMonths = timeSpan.GetMonths();

    //    var numDays = timeSpan.ToString("%d");
    //    var numHours = timeSpan.ToString("%h");
    //    var numMinutes = timeSpan.ToString("%m");
    //    var numSeconds = timeSpan.ToString("%s");

    //    var sb = new StringBuilder();

    //    // did retail display months/years?

    //    //if (numYears > 0) sb.Append(numYears + "y ");
    //    //if (numMonths > 0) sb.Append(numMonths + "mo ");

    //    if (numDays != "0") sb.Append(numDays + "d ");
    //    if (numHours != "0") sb.Append(numHours + "h ");
    //    if (numMinutes != "0") sb.Append(numMinutes + "m ");
    //    if (numSeconds != "0") sb.Append(numSeconds + "s ");

    //    return sb.ToString().Trim();
    //}

    //public static string GetFriendlyLongString(this TimeSpan timeSpan)
    //{
    //    var numDays = timeSpan.ToString("%d");
    //    var numHours = timeSpan.ToString("%h");
    //    var numMinutes = timeSpan.ToString("%m");
    //    var numSeconds = timeSpan.ToString("%s");

    //    var sb = new StringBuilder();

    //    if (numDays != "0") sb.Append(numDays + $" day{((timeSpan.Days > 1) ? "s" : "")} ");
    //    if (numHours != "0") sb.Append($"{((numDays != "0") ? ", " : "")}" + numHours + $" hour{((timeSpan.Hours > 1) ? "s" : "")} ");
    //    if (numMinutes != "0") sb.Append($"{((numDays != "0" || numHours != "0") ? ", " : "")}" + numMinutes + $" minute{((timeSpan.Minutes > 1) ? "s" : "")} ");
    //    if (numSeconds != "0") sb.Append($"{((numDays != "0" || numHours != "0" || numMinutes != "0") ? "and " : "")}" + numSeconds + $" second{((timeSpan.Seconds > 1) ? "s" : "")} ");

    //    return sb.ToString().Trim();
    //}

    //public static uint SecondsPerMonth = 60 * 60 * 24 * 30;      // 30-day estimate
    //public static uint SecondsPerYear = 60 * 60 * 24 * 365;      // non-leap year

    //public static uint GetMonths(this TimeSpan timeSpan)
    //{
    //    return (uint)timeSpan.TotalSeconds / SecondsPerMonth;
    //}

    //public static uint GetYears(this TimeSpan timeSpan)
    //{
    //    return (uint)timeSpan.TotalSeconds / SecondsPerYear;
    //}
}