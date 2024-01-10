using ACE.Server.Network.Managers;

namespace ACE.Shared.Helpers;

public static class NetworkHelpers
{
    public static int ActiveConnections(this Player player) => NetworkManager.GetSessionEndpointTotalByAddressCount(player.Session.EndPointC2S.Address);
}
