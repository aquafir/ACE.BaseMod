using ACE.Database.Entity;
using ACE.Database.Models.Shard;
using Biota = ACE.Database.Models.Shard.Biota;

namespace Tower;

[HarmonyPatch]
public static class OfflineProgress
{
    static double MinTime = TimeSpan.FromHours(1).TotalSeconds;
    static double MaxTime = TimeSpan.FromHours(24 * 3).TotalSeconds;

    public static double TimeSinceOfflineProgressClaimed(this Player player) => Time.GetUnixTime() - (player.GetProperty(FakeFloat.TimestampOfflineRewards) ?? Time.GetUnixTime());
    //public static PropertyFloat TimestampLastOfflineProgressClaimed => (PropertyFloat) (this Player playerfloor) => (PropertyFloat)(floor.Index + FirstCompletionRangeStart);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.DoPlayerEnterWorld), new Type[] { typeof(Session), typeof(Character), typeof(Biota), typeof(PossessedBiotas) })]
    public static void PostDoPlayerEnterWorld(Session session, Character character, Biota playerBiota, PossessedBiotas possessedBiotas)
    {
        
    }


    [CommandHandler("offline", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
    public static void HandleOfflineRewards(Session session, params string[] parameters)
    {
        var player = session.Player;

        var lapsed = Math.Clamp(player.TimeSinceOfflineProgressClaimed(), MinTime, MaxTime);


        var prev = player.PreviousTimeInGame().ToTimeSpan().GetFriendlyString();
        var tot = player.TotalTimeInGame().ToTimeSpan().GetFriendlyString();
        player.SendMessage($"Previous time: {prev}\nTotal time: {tot}");
    }
}