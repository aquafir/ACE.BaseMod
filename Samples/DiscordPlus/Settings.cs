namespace DiscordPlus
{
    public class Settings
    {
        //Supply credentials
        public ulong RELAY_CHANNEL_ID { get; set; } = 800000000000000000;
        public string BOT_TOKEN { get; set; } = "";
        public int MAX_MESSAGE_LENGTH { get; set; } = 10000;
        public double MESSAGE_INTERVAL { get; set; } = 10000;
        public string PREFIX { get; set; } = "~";

        public List<ulong> DevIds { get; set; } = new ();
    }
}