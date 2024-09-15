namespace SelectiveStartup;

public class Settings
{
    public HashSet<ServerTask> Skip { get; set; } = Enum.GetValues<ServerTask>().ToHashSet();
}


public enum ServerTask
{
    EventManager,
    WorldManager,
    NetworkManager,
    SocketManager,
    InboundMessageManager,
    HouseManager,
    PlayerManager,
    RealmManager,
    GuidManager,
    PropertyManager,
    DatabaseManager,
    DDDManager,
    ServerManager,
    //DatManager,

    ConfigManager,
    CheckBiotaPropertiesPaletteOrder,
}