using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages.Messages;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace Discord;
internal class Discord
{
    private DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _services;
    private IMessageChannel _channel;


    private readonly DiscordSocketConfig _socketConfig = new()
    {
        GatewayIntents = GatewayIntents.All,
        //GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent,
        AlwaysDownloadUsers = true,
        LogGatewayIntentWarnings = false,
        LogLevel = LogSeverity.Critical,    //Spamming something otherwise?
    };

    private ConcurrentQueue<string> _outgoingMessages;
    private System.Timers.Timer _batchTimer;

    private Dictionary<string, IUser> _users = new();
    private DateTime _lastUserCheck = DateTime.MinValue;
    private TimeSpan _userCheckInterval = TimeSpan.FromSeconds(5);

    public Discord()
    {
        _configuration = new ConfigurationBuilder()
            //.AddEnvironmentVariables(prefix: "DC_")
            //.AddJsonFile("appsettings.json", optional: true)
            .Build();

        _services = new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton(_socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .BuildServiceProvider();
    }

    public async Task RunAsync()
    {
        //Set up outgoing message queue
        _outgoingMessages = new ConcurrentQueue<string>();
        _batchTimer = new System.Timers.Timer
        {
            AutoReset = true,
            Enabled = false,
            Interval = PatchClass.Settings.MESSAGE_INTERVAL,
        };
        _batchTimer.Elapsed += SendQueuedMessages;


        _client = _services.GetRequiredService<DiscordSocketClient>();

        _client.Log += LogAsync;

        // Here we can initialize the service that will register and execute our commands
        await _services.GetRequiredService<InteractionHandler>()
            .InitializeAsync();

        // Bot token can be provided from the Configuration object we set up earlier
        await _client.LoginAsync(TokenType.Bot, "MTAzODc0MTk1MTYzNTI3MTcxMA.GiMqNe.0zKrTWfXP2X12DG-SMtSkmscCG5zKD_NuGZksg");
        await _client.StartAsync();

        await OnReady();

        // Never quit the program until manually forced to.
        await Task.Delay(Timeout.Infinite);
    }

    private async Task OnReady()
    {
        ModManager.Log("Logged in to Discord...");

        //Grab the channel to be used for relaying messages        
        _channel = await _client.GetChannelAsync(PatchClass.Settings.RELAY_CHANNEL_ID) as IMessageChannel;
        if (_channel is null)
        {
            ModManager.Log($"Error getting Discord channel: {PatchClass.Settings.RELAY_CHANNEL_ID}", ModManager.LogLevel.Error);
            return;
        }

        //Set up relay
        _client.MessageReceived += OnDiscordChat;
        _client.Disconnected += Discord_Disconnected;

        //Start ACE-->Discord timer
        _batchTimer.Enabled = true;

        //Say hi
        //await _channel.SendMessageAsync("ACE Chat Relay is online.");
    }

    private async Task Discord_Disconnected(Exception arg)
    {
        //ModManager.Log($"TODO: Handle Discord disconnect", ModManager.LogLevel.Warn);
    }

    internal async Task Shutdown()
    {
        try
        {
            ModManager.Log($"Shutting down Discord...");
            await _client.LogoutAsync();
            _batchTimer.Elapsed -= SendQueuedMessages;
            _batchTimer.Enabled = false;

            _client.Ready -= OnReady;
            _client.MessageReceived -= OnDiscordChat;

            _client.Disconnected -= Discord_Disconnected;

            await _client.StopAsync();
        }
        catch (Exception ex)
        {
            ModManager.Log($"Error with Discord: {ex.Message}");
        }
    }



    #region Discord --> ACE (Chat and DM)
    /// <summary>
    /// Receives and routes messages from Discord
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public async Task OnDiscordChat(SocketMessage arg)
    {
        var content = arg.Content;
        //Ignore system messages
        var msg = arg as SocketUserMessage;
        if (msg is null)
            return;

        //Check for chat in approved channel
        if (msg.Channel.Id == PatchClass.Settings.RELAY_CHANNEL_ID && !msg.Author.IsBot)
        {
            RelayDiscordGeneralChat(msg);
            return;
        }

        //Check for a private chat
        var channelType = msg.Channel.GetChannelType();
        if (channelType is not null && channelType.Value == ChannelType.DM && !msg.Author.IsBot)
        {
            RelayDiscordPrivateChat(msg);
            return;
        }
    }
    /// <summary>
    /// Send a message to an in-game player when the bot gets a private messages
    /// </summary>
    public async Task RelayDiscordPrivateChat(SocketUserMessage msg)
    {
        var match = Regex.Match(msg.Content, @"/t (?<name>[+\w]+),(?<message>.*)");

        if (!match.Success)
        {
            //Error response
            return;
        }

        //Trim the prefix to the name if needed
        var name = DiscordName(match.Groups["name"].Value);
        var message = match.Groups["message"].Value.Trim();

        var targetPlayer = PlayerManager.GetOnlinePlayer(name);
        if (targetPlayer is null)
        {
            //Response
            return;
        }

        var fakeSender = GetDiscordFakePlayer(msg);

        //Message with a name/clickable response
        var tellEvent = new GameEventTell(targetPlayer.Session, message, fakeSender.Name, fakeSender.Guid.Full, targetPlayer.Guid.Full, ChatMessageType.Tell);
        targetPlayer.Session.Network.EnqueueSend(tellEvent);

        //Message with no prefix
        //player.SendMessage(message, ChatMessageType.Tell);
    }
    /// <summary>
    /// Broadcast general chat
    /// </summary>
    public async Task RelayDiscordGeneralChat(SocketUserMessage msg)
    {
        //Skip command messages
        if (msg.Content.StartsWith("!"))
            return;

        //Check if the server has disabled general chat
        //if (PropertyManager.GetBool("chat_disable_general").Item)
        //    return;

        var fakeSender = GetDiscordFakePlayer(msg);

        //Construct message
        //ModManager.Log($"Trying to send message from Discord:\n\t{msg.Author.Username}: {msg.Content}");
        var chatMessage = new GameMessageTurbineChat(
            ChatNetworkBlobType.NETBLOB_EVENT_BINARY,
            ChatNetworkBlobDispatchType.ASYNCMETHOD_SENDTOROOMBYNAME,
            TurbineChatChannel.General,
            fakeSender.Name, //Use prefix to filter out messages the relay is sending
                             //"~Discord",
            msg.Content,
            fakeSender.Guid.Full,
            ChatType.General);

        //Send a message to any player who is listening to general chat
        foreach (var recipient in PlayerManager.GetAllOnline())
        {
            // handle filters
            if (!recipient.GetCharacterOption(CharacterOption.ListenToGeneralChat))
                return;

            //Todo: think about how to handle squelches?
            //if (recipient.SquelchManager.Squelches.Contains(session.Player, ChatMessageType.AllChannels))
            //    continue;

            //ModManager.Log($"Sending to recipient {recipient.Name}");
            recipient.Session.Network.EnqueueSend(chatMessage);
        }
    }
    #endregion

    #region ACE --> Discord
    /// <summary>
    /// Relays ingame GameActionTalkDirect to a Discord user
    /// </summary>
    public async void RelayIngameDirectMessage(string userName, string message, Session session)
    {
        //Get a name convention shared by ACE/Discord
        userName = DiscordName(userName);

        //Check if the user is known
        if (!_users.TryGetValue(userName, out var user))
        {
            //Only check periodically update Discord users
            var lapsed = DateTime.Now - _lastUserCheck;
            if (lapsed < _userCheckInterval)
            {
                ModManager.Log($"Skipping user list synchronization for missing user: {lapsed.TotalSeconds} / {_userCheckInterval.TotalSeconds}s");
                return;
            }

            //If they aren't, fetch users for the server?
            await foreach (var userAsync in _channel.GetUsersAsync())
            {
                foreach (var u in userAsync)
                {
                    //Todo: decide how to map usernames to lookup names
                    if (_users.ContainsKey(u.Username))
                        continue;

                    //May only want to keep the ID?
                    _users.Add(u.Username, u);
                    ModManager.Log($"Found user {u.Username} - {u.Id} - {u.Status}");
                }
            }

            //If the user still isn't found a message can't be sent
            if (!_users.TryGetValue(userName, out user))
            {
                ModManager.Log($"User {userName} not found.  Message not sent.");
                var statusMessage = new GameEventWeenieError(session, WeenieError.CharacterNotAvailable);
                session.Network.EnqueueSend(statusMessage);
                return;
            }
        }

        //Send message to a found user
        user.SendMessageAsync(message);


        //And to the sender
        //EnqueueBroadcast(new GameMessageHearSpeech(message, GetNameWithSuffix(), Guid.Full, ChatMessageType.Speech), LocalBroadcastRange, ChatMessageType.Speech);
        if (session.Player.Name != userName)
            session.Network.EnqueueSend(new GameMessageSystemChat($"You tell {userName}, \"{message}\"", ChatMessageType.OutgoingTell));

        //Not handling afk / squelches

        //var tell = new GameEventTell(targetPlayer.Session, message, session.Player.GetNameWithSuffix(), session.Player.Guid.Full, targetPlayer.Guid.Full, ChatMessageType.Tell);
        //targetPlayer.Session.Network.EnqueueSend(tell
    }
    /// <summary>
    /// Called when a GameMessageTurbineChat is created and queues for Discord if eligible
    /// </summary>
    public void RelayIngameChat(string message, string senderName, ChatType chatType, uint channel, uint senderID, ChatNetworkBlobType chatNetworkBlobType, ChatNetworkBlobDispatchType chatNetworkBlobDispatchType)
    {
        //ModManager.Log($"Ingame message from {senderName} of type {chatType} has message:\n\t{message}");
        if (message is null || senderName is null)
            return;

        //Skip messages coming from Discord
        if (senderName.StartsWith(PatchClass.Settings.PREFIX))
            return;

        //Only relay LFG or General
        if (chatType == ChatType.General || chatType == ChatType.LFG)
            QueueMessageForDiscord($"[{chatType}] {senderName}: {message}");
    }

    public void QueueMessageForDiscord(string message)
    {
        //ModManager.Log($"Message queued for Discord:\n\t{message}");
        _outgoingMessages.Enqueue(message);
    }
    /// <summary>
    /// Batches queued messages going to Discord to help with rate limits
    /// </summary>
    private void SendQueuedMessages(object sender, ElapsedEventArgs e)
    {
        if (_channel is null)
            return;

        var batchedMessage = new StringBuilder();

        while (batchedMessage.Length < PatchClass.Settings.MAX_MESSAGE_LENGTH &&
            _outgoingMessages.TryDequeue(out string message))
        {
            batchedMessage.AppendLine(message);
        }

        Task.Run(async () =>
        {
            await _channel.SendMessageAsync(batchedMessage.ToString());
        });
    }
    #endregion

    #region Utility
    /// <summary>
    /// Converts ACE name to Discord format
    /// </summary>
    private string DiscordName(string aceName)
    {
        if (aceName.StartsWith(PatchClass.Settings.PREFIX))
            aceName = aceName.Substring(PatchClass.Settings.PREFIX.Length);

        return aceName;
    }
    /// <summary>
    /// Converts Discord name to ACE format
    /// </summary>
    private string ACEName(string discordName)
    {
        if (!discordName.StartsWith(PatchClass.Settings.PREFIX))
            discordName = PatchClass.Settings.PREFIX + discordName;

        return discordName;
    }

    /// <summary>
    /// Todo: Decide if GUID should just be generated every time, skipping the FakePlayer
    /// Discord user's fake in-game identity.  
    /// </summary>
    private Dictionary<ulong, FakePlayer> _fakePlayers = new();
    private FakePlayer GetDiscordFakePlayer(SocketUserMessage msg)
    {
        //Creature a fake player to use with chat clicks / r?
        if (!_fakePlayers.TryGetValue(msg.Author.Id, out var fakeSender))
        {
            fakeSender = new FakePlayer(ACEName(msg.Author.Username));
            _fakePlayers.Add(msg.Author.Id, fakeSender);
        }

        return fakeSender;
    }
    #endregion


    private async Task LogAsync(LogMessage message)
    => Console.WriteLine(message.ToString());
}
