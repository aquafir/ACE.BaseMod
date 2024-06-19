using ACE.Server.Network.GameEvent;
using ACE.Server.Network.GameMessages;

namespace ACE.Shared.Helpers;
public static class GameMessageExtensions
{
    /// <summary>
    /// Equivalent of base in a constructor for a GameEventMessage
    /// </summary>
    public static void Base(this GameEventMessage eventMessage, Session session, GameEventType eventType, GameMessageGroup group, GameMessageOpcode opCode = GameMessageOpcode.GameEvent)
    {
        eventMessage.Base(opCode, group);

        eventMessage.EventType = eventType;
        eventMessage.Session = session;

        var guid = session.Player != null ? session.Player.Guid : new ObjectGuid(0);

        eventMessage.Writer.WriteGuid(guid);
        eventMessage.Writer.Write(session.GameEventSequence++);
        eventMessage.Writer.Write((uint)eventMessage.EventType);
    }

    /// <summary>
    /// Equivalent of base in a constructor for a GameMessage
    /// </summary>
    public static void Base(this GameMessage gameMessage, GameMessageOpcode opCode, GameMessageGroup group)
    {
        gameMessage.Opcode = opCode;
        gameMessage.Group = group;
        gameMessage.Data = new System.IO.MemoryStream();
        gameMessage.Writer = new BinaryWriter(gameMessage.Data);

        if (gameMessage.Opcode != GameMessageOpcode.None)
            gameMessage.Writer.Write((uint)gameMessage.Opcode);
    }
}
