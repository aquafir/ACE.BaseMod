using ACE.Server.WorldObjects;

namespace Expansion.Features;
[HarmonyPatchCategory(nameof(Feature.CreatureFollow))]
internal class CreatureFollow
{
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(WorldManager), nameof(WorldManager.ThreadSafeTeleport), new Type[] { typeof(Player), typeof(Position), typeof(IAction), typeof(bool) })]
    //public static void PreThreadSafeTeleport(Player player, Position newPosition, IAction actionToFollowUpWith, bool fromPortal)
    //{
    //    Follow(player, newPosition);
    //}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.Teleport), new Type[] { typeof(Position), typeof(bool) })]
    public static void PreTeleport(Position _newPosition, bool fromPortal, ref Player __instance)
    {
        Follow(__instance, _newPosition);
    }

    private static void Follow(Player player, Position newPosition)
    {
        if (player is null) return;

        var followers = player.GetSplashTargets(player, 20, 20).Where(x => x.MonsterState == Creature.State.Awake);
        player.SendMessage($"Followed by {followers.Count()} creatures!");

        foreach (var f in followers)
        {
            var newBiota = WorldObjectFactory.CreateNewWorldObject(f.Weenie);
            newBiota.Location = new(newPosition);
            f.Destroy();
            newBiota.EnterWorld();
            ModManager.Log($"{newBiota.Name} following {player.Name} to {newPosition}");
        }
    }

}
