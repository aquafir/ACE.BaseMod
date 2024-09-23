using ACE.Server.Network.Managers;

namespace ACE.Shared.Helpers;

public static class NetworkExtensions
{
    /// <summary>
    /// Sends all updated Attributes of a player to them
    /// </summary>
    public static void SendUpdatedAttributes(this Player player)
    {
        //Update the player
        foreach (var key in player.Attributes)
            player.SendUpdated(key.Value);
    }
    public static void SendUpdated(this Player player, PropertyAttribute key)
    {
        if (!player.Attributes.TryGetValue(key, out var skill))
            return;

        player.SendUpdated(skill);
    }
    public static void SendUpdated(this Player player, CreatureAttribute key)
    {
        GameMessagePrivateUpdateAttribute gameMessagePrivateUpdateAttribute = new GameMessagePrivateUpdateAttribute(player, key);
        player.Session.Network.EnqueueSend(gameMessagePrivateUpdateAttribute);
    }

    /// <summary>
    /// Sends all updated Vitals of a player to them
    /// </summary>
    public static void SendUpdatedVitals(this Player player)
    {
        //Update the player
        foreach (var key in player.Vitals)
            player.SendUpdated(key.Value);
    }
    public static void SendUpdated(this Player player, PropertyAttribute2nd key)
    {
        if (!player.Vitals.TryGetValue(key, out var skill))
            return;

        player.SendUpdated(skill);
    }
    public static void SendUpdated(this Player player, CreatureVital key)
    {
        GameMessagePrivateUpdateVital gameMessagePrivateUpdateVital = new GameMessagePrivateUpdateVital(player, key);
        player.Session.Network.EnqueueSend(gameMessagePrivateUpdateVital);
    }



    /// <summary>
    /// Sends all updated skills of a player to them
    /// </summary>
    public static void SendUpdatedSkills(this Player player)
    {
        //Update the player
        foreach (var key in player.Skills)
            player.SendUpdated(key.Value);

        GameMessagePrivateUpdatePropertyInt gameMessagePrivateUpdatePropertyInt = new GameMessagePrivateUpdatePropertyInt(player, PropertyInt.AvailableSkillCredits, player.AvailableSkillCredits.GetValueOrDefault());
        player.Session.Network.EnqueueSend(gameMessagePrivateUpdatePropertyInt);
    }
    public static void SendUpdated(this Player player, Skill key) 
    {
        if (!player.Skills.TryGetValue(key, out var skill))
            return;

        player.SendUpdated(skill);
    }
    public static void SendUpdated(this Player player, CreatureSkill key)
    {
        var sac = key.AdvancementClass;
        if (sac != SkillAdvancementClass.Trained && sac != SkillAdvancementClass.Specialized)
            return;

        GameMessagePrivateUpdateSkill gameMessagePrivateUpdateSkill = new GameMessagePrivateUpdateSkill(player, key);
        //GameMessageSystemChat gameMessageSystemChat = new GameMessageSystemChat($"{sType.ToSentence()} {(sac == SkillAdvancementClass.Trained ? "trained" : "specialized")}.", ChatMessageType.Advancement);
        player.Session.Network.EnqueueSend(gameMessagePrivateUpdateSkill);
    }


    /// <summary>
    /// Returns number of sessions sharing this players endpoint
    /// </summary>
#if REALM
    public static int ActiveConnections(this Player player) => NetworkManager.Instance.GetSessionEndpointTotalByAddressCount(player.Session.EndPointC2S.Address);
#else
    public static int ActiveConnections(this Player player) => NetworkManager.GetSessionEndpointTotalByAddressCount(player.Session.EndPointC2S.Address);
#endif
}
