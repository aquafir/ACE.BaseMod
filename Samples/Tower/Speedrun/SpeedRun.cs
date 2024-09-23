namespace Tower;

[CommandCategory(nameof(Feature.Speedrun))]
[HarmonyPatchCategory(nameof(Feature.Speedrun))]
public static class Speedrun
{
    static List<TowerFloor> Floors => PatchClass.Settings.Tower.Floors;
    static SpeedrunSettings Settings => PatchClass.Settings.Speedrun;

    public static bool TryGetTimeFromLastFloorStarted(this Player player, out TimeSpan time)
    {
        time = default;

        if (!player.TryGetChallengedFloor(out var floor, out var start))
            return false;

        time = player.TotalTimeInGame().ToTimeSpan() - start;
        return true;
    }

    #region Helpers
    //public static int FirstCompletionRangeStart { get; set; } = 56000;
    //public static int PersonalBestRangeStart { get; set; } = 57000;
    public static PropertyFloat FirstCompletionTimeProp(this TowerFloor floor) => (PropertyFloat)(floor.Index + Settings.FirstCompletionRangeStart);
    public static PropertyInt FirstCompletionLevelProp(this TowerFloor floor) => (PropertyInt)(floor.Index + Settings.FirstCompletionRangeStart);
    public static PropertyFloat PersonalBestTimeProp(this TowerFloor floor) => (PropertyFloat)(floor.Index + Settings.PersonalBestRangeStart);

    public static TimeSpan? GetPersonalBest(this Player player, TowerFloor floor)
        => player.GetProperty(floor.PersonalBestTimeProp())?.ToTimeSpan();
    public static void SetPersonalBest(this Player player, TowerFloor floor, double time)
        => player.SetProperty(floor.PersonalBestTimeProp(), time);

    public static TimeSpan? GetFirstCompletionTime(this Player player, TowerFloor floor)
        => player.GetProperty(floor.FirstCompletionTimeProp())?.ToTimeSpan();
    public static void SetFirstCompletionTime(this Player player, TowerFloor floor, double time)
        => player.SetProperty(floor.FirstCompletionTimeProp(), time);

    public static int? GetFirstCompletionLevel(this Player player, TowerFloor floor)
        => player.GetProperty(floor.FirstCompletionLevelProp());
    public static void SetFirstCompletionLevel(this Player player, TowerFloor floor, int level)
        => player.SetProperty(floor.FirstCompletionLevelProp(), level);
    #endregion

    public static PropertyInt CurrentFloor => (PropertyInt)55999;
    public static PropertyFloat CurrentFloorStartTimestamp => (PropertyFloat)55999;
    public static bool TryGetChallengedFloor(this Player player, out TowerFloor floor, out TimeSpan startTime)
    {
        floor = null;
        startTime = (player.GetProperty(CurrentFloorStartTimestamp) ?? -1).ToTimeSpan();

        var index = player.GetProperty(CurrentFloor);
        if (index is null)
            return false;

        return index.Value.TryGetFloorByIndex(out floor);
    }
    public static void SetChallengedFloor(this Player player, TowerFloor floor)
    {
        player.SetProperty(CurrentFloor, floor.Index);
        player.SetProperty(CurrentFloorStartTimestamp, player.TotalTimeInGame());

        var time = Time.GetUnixTime();
        //player.SendMessage($"Starting {floor.Name} ({floor.Index}) at {Time.GetDateTimeFromTimestamp(time)}");
        //player.SendMessage($"Starting {floor.Name} ({floor.Index}) after {player.TotalTimeInGame().ToFriendlyTime()} time in game.");
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
                if (!player.TryGetTimeFromLastFloorStarted(out var timeCurrent))
                {
                    player.SendMessage($"For some reason the start of your last floor is unknown. Report to blode");
                }
                else
                {
                    //Check for first completion
                    var timeFirst = player.GetFirstCompletionTime(challengedFloor);
                    if (timeFirst is null)
                    {
                        player.SendMessage($"Congratulations!  You first finished floor {challengedFloor.Name} at level {player.Level} after {player.TotalTimeInGame().ToTimeSpan().GetFriendlyString()}");
                        player.SetFirstCompletionTime(challengedFloor, player.TotalTimeInGame());
                        player.SetFirstCompletionLevel(challengedFloor, player.Level ?? 1);
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
    [HarmonyPatch(typeof(Player), nameof(Player.Teleport), new Type[] { typeof(Position), typeof(bool), typeof(bool) })]
    public static void PostTeleport(Position newPosition, bool teleportingFromInstance, bool fromPortal, ref Player __instance)
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

        foreach (var floor in Floors.OrderBy(x => x.Level))
        {
            player.RemoveProperty(floor.FirstCompletionLevelProp());
            player.RemoveProperty(floor.PersonalBestTimeProp());
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
        foreach (var floor in Floors.OrderBy(x => x.Level))
        {
            var first = player.GetFirstCompletionTime(floor);
            //if (first is null)
            //    break;

            var pb = player.GetPersonalBest(floor);

            sb.Append($"{floor.Name} @ {floor.Landblock:X4} Level {floor.Level}\n");
            sb.Append($"  First: {(first is null ? "n/a" : $"{first}")}  --  Best: {(pb is null ? "n/a" : $"{pb}")}\n");
        }

        player.SendMessage($"{sb}");
    }

    [CommandHandler("tig", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleTimeInGame(Session session, params string[] parameters)
    {
        var player = session.Player;

        var prev = player.PreviousTimeInGame().ToTimeSpan().GetFriendlyString();
        var tot = player.TotalTimeInGame().ToTimeSpan().GetFriendlyString();
        player.SendMessage($"Previous time: {prev}\nTotal time: {tot}");
    }
}
