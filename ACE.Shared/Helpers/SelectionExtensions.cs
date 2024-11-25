namespace ACE.Shared.Helpers;

public static class SelectionExtensions
{
    public static bool TryGetCurrentSelection(this Player player, out WorldObject wo, Player.SearchLocations locations = Player.SearchLocations.Everywhere)
    {
        wo = null;

        if (player is null)
            return false;

        //Try to find selected object ID
        var objectId = ObjectGuid.Invalid;
        if (player.HealthQueryTarget.HasValue)
            objectId = new ObjectGuid(player.HealthQueryTarget.Value);
        else if (player.ManaQueryTarget.HasValue)
            objectId = new ObjectGuid(player.ManaQueryTarget.Value);
        else if (player.CurrentAppraisalTarget.HasValue)
            objectId = new ObjectGuid(player.CurrentAppraisalTarget.Value);

        if (objectId == ObjectGuid.Invalid)
            return false;

        wo = player.FindObject(objectId.Full, locations);

        return wo is not null;
    }
}