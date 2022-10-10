using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord;

internal class DiscordSettings
{
    //Supply credentials
    public ulong RELAY_CHANNEL_ID { get; set; } = 800000000000000000;
    public string BOT_TOKEN { get; set; } = "";
    public int MAX_MESSAGE_LENGTH { get; set; } = 10000;
    public double MESSAGE_INTERVAL { get; set; } = 10000;
    public string PREFIX { get; set; } = "~";
}
