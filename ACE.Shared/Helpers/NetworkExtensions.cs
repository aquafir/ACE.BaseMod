using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Network.Managers;

namespace ACE.Shared.Helpers;

public static class NetworkExtensions
{
    /// <summary>
    /// Sends all updated skills of a player to them
    /// </summary>
    public static void SendUpdatedSkills(this Player player)
    {
        //Update the player
        foreach (var skill in player.Skills)
        {
            var sType = skill.Key;
            var sac = skill.Value.AdvancementClass;
            if (sac != SkillAdvancementClass.Trained && sac != SkillAdvancementClass.Specialized)
                continue;

            GameMessagePrivateUpdateSkill gameMessagePrivateUpdateSkill = new GameMessagePrivateUpdateSkill(player, player.GetCreatureSkill(sType));
            //GameMessageSystemChat gameMessageSystemChat = new GameMessageSystemChat($"{sType.ToSentence()} {(sac == SkillAdvancementClass.Trained ? "trained" : "specialized")}.", ChatMessageType.Advancement);
            player.Session.Network.EnqueueSend(gameMessagePrivateUpdateSkill);
        }
        GameMessagePrivateUpdatePropertyInt gameMessagePrivateUpdatePropertyInt = new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.AvailableSkillCredits, player.AvailableSkillCredits.GetValueOrDefault());
        player.Session.Network.EnqueueSend(gameMessagePrivateUpdatePropertyInt);
    }

    /// <summary>
    /// Returns number of sessions sharing this players endpoint
    /// </summary>
    public static int ActiveConnections(this Player player) => NetworkManager.GetSessionEndpointTotalByAddressCount(player.Session.EndPointC2S.Address);
}
