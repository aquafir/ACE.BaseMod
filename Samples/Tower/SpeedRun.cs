using ACE.Server.Realms;

namespace Tower;

[HarmonyPatch]
public static class SpeedRun
{
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
#if REALM
    [HarmonyPatch(typeof(Player), nameof(Player.Teleport), new Type[] { typeof(InstancedPosition), typeof(bool), typeof(bool) })]
    public static void PostTeleport(InstancedPosition newPosition, bool teleportingFromInstance, bool fromPortal, ref Player __instance)
#else
    [HarmonyPatch(typeof(Player), nameof(Player.Teleport), new Type[] { typeof(Position), typeof(bool) })]
#endif
    {
        //Ignore logging back in
        //if (!fromPortal)
        //    return;

        //Check for completion
        __instance.CheckCompleteFloor();
    }

    [CommandHandler("clearspeed", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
#if REALM
    public static void HandleClearSpeed(ISession session, params string[] parameters)
#else
public static void HandleClearSpeed(Session session, params string[] parameters)
#endif
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
#if REALM
    public static void HandleSpeed(ISession session, params string[] parameters)
#else
public static void HandleSpeed(Session session, params string[] parameters)
#endif
    {
        var player = session.Player;

        var prev = player.PreviousTimeInGame();
        var tot = player.TotalTimeInGame();

        var sb = new StringBuilder();
        foreach (var floor in PatchClass.Settings.Floors.OrderBy(x => x.Level))
        {
            var first = player.GetFirstCompletion(floor);
            //if (first is null)
            //    break;

            var pb = player.GetPersonalBest(floor);

            sb.Append($"{floor.Name} @ {floor.Landblock:X4} Level {floor.Level}\n");
            sb.Append($"  First: {(first is null ? "n/a" : $"{first}")}  --  Best: {(pb is null ? "n/a" : $"{pb}")}\n");
        }

        player.SendMessage($"{sb}");
    }

    [CommandHandler("tig", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
#if REALM
    public static void HandleTimeInGame(ISession session, params string[] parameters)
#else
public static void HandleTimeInGame(Session session, params string[] parameters)
#endif
    {
        var player = session.Player;

        var prev = player.PreviousTimeInGame().ToTimeSpan().GetFriendlyString();
        var tot = player.TotalTimeInGame().ToTimeSpan().GetFriendlyString();
        player.SendMessage($"Previous time: {prev}\nTotal time: {tot}");
    }

    [CommandHandler("t3", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
#if REALM
    public static void HandleT3(ISession session, params string[] parameters)
#else
public static void HandleT3(Session session, params string[] parameters)
#endif
    {
        var player = session.Player;
        player.AugmentationCriticalExpertise = 100;
        player.AugmentationCriticalDefense = 100;
    }
}
