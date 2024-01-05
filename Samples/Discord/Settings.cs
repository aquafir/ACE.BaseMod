namespace Discord;

public class Settings
{
    public ulong RELAY_CHANNEL_ID { get; set; } = 800000000000000000;           //Channel to listen to
    public ulong GUILD_ID { get; set; } = 800000000000000000;                   //Guild to register commands to, since global takes time
    public string BOT_TOKEN { get; set; } = "";                                 //Bot credentials
    public int MAX_MESSAGE_LENGTH { get; set; } = 10000;
    public double MESSAGE_INTERVAL { get; set; } = 10000;
    public string PREFIX { get; set; } = "~";

    public List<ulong> DevIds { get; set; } = new();
    public const string RequiredRole = "SomeRole";

    public string LootProfilePath { get; } = Path.Combine(ModManager.ModPath, "LootProfiles");//Path.Combine(Mod.ModPath, "LootProfiles");
    public bool LootProfileUseUsername { get; set; } = true;
}