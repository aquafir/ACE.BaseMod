using ACE.Entity;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Shared.Helpers;

public static class WorldObjectExtensions
{
    /// <summary>
    /// Play animation on a WorldObject
    /// </summary>
    public static void PlayAnimation(this WorldObject worldObject, PlayScript script, float speed = 1f) =>
        worldObject.EnqueueBroadcast(new GameMessageScript(worldObject.Guid, script, speed));

    //From AdminCommand TryCreateObject
    //public static void CreateInFront(this WorldObject obj, Player player, int? palette = null, float? shade = null, int? lifespan = null)
    //{
    //    obj.MoveInFrontOf(player);
    //    obj.EnterWorld();
    //}

    public static void MoveInFrontOf(this WorldObject obj, Player player)
    {
        if (obj.WeenieType == WeenieType.Creature)
            obj.Location = player.Location.InFrontOf(5f, true);
        else
        {
            var dist = Math.Max(2, obj.UseRadius ?? 2);

            obj.Location = player.Location.InFrontOf(dist);
        }

        obj.Location.LandblockId = new LandblockId(obj.Location.GetCell());
    }
}
