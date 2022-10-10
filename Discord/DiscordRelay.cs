using Discord.WebSocket;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Managers;
using System.Timers;
using System.Collections.Concurrent;
using ACE.Server.Network.GameMessages;
using Newtonsoft.Json.Linq;

namespace Discord;

public class DiscordRelay
{
    //Get credentials from file
    private static DiscordSettings _settings = new();
    private static string filePath = Path.Combine(Mod.ModPath, "Settings.json");

    private static DiscordSocketClient discord;
    private static IMessageChannel channel;

    //Outgoing messages
    private static ConcurrentQueue<string> outgoingMessages;
    private static System.Timers.Timer messageTimer;

    public async static void Initialize()
    {
        LoadSettings();

        //Set up outgoing message queue
        outgoingMessages = new ConcurrentQueue<string>();
        messageTimer = new System.Timers.Timer
        {
            AutoReset = true,
            Enabled = false,
            Interval = _settings.MESSAGE_INTERVAL,
        };
        messageTimer.Elapsed += SendQueuedMessages;

        //https://discordnet.dev/faq/basics/client-basics.html
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.GuildMessages | GatewayIntents.DirectMessages,
             
        };
        discord = new DiscordSocketClient(config);

        await discord.LoginAsync(TokenType.Bot, _settings.BOT_TOKEN);
        await discord.StartAsync();
        discord.Ready += OnReady;
    }

    private static void LoadSettings()
    {
        if (!File.Exists(filePath))
        {
            ModManager.Log($"Created settings at {filePath}");
            string jsonString = JsonSerializer.Serialize(_settings);
            File.WriteAllText(filePath, jsonString);
        }
        else
        {
            try
            {
                ModManager.Log($"Loading settings from {filePath}");
                var json = File.ReadAllText(filePath);
                _settings = JsonSerializer.Deserialize<DiscordSettings>(json);
            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to deserialize settings from {filePath}");
                _settings = new DiscordSettings();
            }
        }
    }

    //Finish initializing when logged in to Discord
    private static async Task OnReady()
    {
        //Grab the channel to be used for relaying messages
        channel = discord.GetChannel(_settings.RELAY_CHANNEL_ID) as IMessageChannel;
        if (channel == null)
        {
            //Handle errors starting up
            return;
        }

        //Set up relay
        discord.MessageReceived += OnDiscordChat;

        //Start ACE-->Discord timer
        messageTimer.Enabled = true;

        //Say hi
        QueueMessageForDiscord("Discord bot is online.");
    }

    //Batch messages going to Discord to help with rate limits
    private static void SendQueuedMessages(object sender, ElapsedEventArgs e)
    {
        if (channel is null)
            return;

        var batchedMessage = new StringBuilder();

        while (batchedMessage.Length < _settings.MAX_MESSAGE_LENGTH &&
            outgoingMessages.TryDequeue(out string message))
        {
            batchedMessage.AppendLine(message);
        }

        Task.Run(async () =>
        {
            await channel.SendMessageAsync(batchedMessage.ToString());
        });
    }

    //Relay messages from Discord
    private static Task OnDiscordChat(SocketMessage msg)
    {
        //ModManager.Log("Received message from Discord");
        //Ignore bot chat and incorrect channels
        if (msg.Author.IsBot || msg.Channel.Id != _settings.RELAY_CHANNEL_ID)
            return Task.CompletedTask;

        //Check if the server has disabled general chat
        if (PropertyManager.GetBool("chat_disable_general").Item)
            return Task.CompletedTask;

        //Construct message
        //ModManager.Log($"Trying to send message from Discord:\n\t{msg.Author.Username}: {msg.Content}");
        var chatMessage = new GameMessageTurbineChat(
            ChatNetworkBlobType.NETBLOB_EVENT_BINARY,
            ChatNetworkBlobDispatchType.ASYNCMETHOD_SENDTOROOMBYNAME,
            TurbineChatChannel.General,
            _settings.PREFIX + msg.Author.Username, //Use prefix to filter out messages the relay is sending
                                                    //"~Discord",
            msg.Content,
            0,
            ChatType.General);
        //var gameMessageTurbineChat = new GameMessageTurbineChat(ChatNetworkBlobType.NETBLOB_EVENT_BINARY, ChatNetworkBlobDispatchType.ASYNCMETHOD_SENDTOROOMBYNAME, adjustedChannelID, session.Player.Name, message, senderID, adjustedchatType);


        //Send a message to any player who is listening to general chat
        foreach (var recipient in PlayerManager.GetAllOnline())
        {
            // handle filters
            if (!recipient.GetCharacterOption(CharacterOption.ListenToGeneralChat))
                return Task.CompletedTask;

            //Todo: think about how to handle squelches?
            //if (recipient.SquelchManager.Squelches.Contains(session.Player, ChatMessageType.AllChannels))
            //    continue;

            //ModManager.Log($"Sending to recipient {recipient.Name}");
            recipient.Session.Network.EnqueueSend(chatMessage);
        }

        return Task.CompletedTask;
    }

    //Called when a GameMessageTurbineChat is created to see if it should be sent to Discord
    public static void RelayIngameChat(string message, string senderName, ChatType chatType, uint channel, uint senderID, ChatNetworkBlobType chatNetworkBlobType, ChatNetworkBlobDispatchType chatNetworkBlobDispatchType)
    {
        //ModManager.Log($"Ingame message from {senderName} of type {chatType} has message:\n\t{message}");
        if (message is null || senderName is null)
            return;
        if (senderName.StartsWith(_settings.PREFIX))
            return;

        if (chatType == ChatType.General || chatType == ChatType.LFG)
            QueueMessageForDiscord($"[{chatType}] {senderName}: {message}");
    }
    public static void QueueMessageForDiscord(string message)
    {
        //ModManager.Log($"Message queued for Discord:\n\t{message}");
        outgoingMessages.Enqueue(message);
    }

    internal static async void Shutdown()
    {
        try
        {
            messageTimer.Elapsed -= SendQueuedMessages;
            discord.Ready -= OnReady;
            discord.MessageReceived -= OnDiscordChat;

            await discord.StopAsync();
        }
        catch (Exception ex)
        {
            ModManager.Log($"Error with Discord: {ex.Message}");
        }
    }
}