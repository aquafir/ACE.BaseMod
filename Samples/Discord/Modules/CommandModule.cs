using Discord;

namespace Discord.Modules;

// Interaction modules must be public and inherit from an IInteractionModuleBase
public class CommandModule : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
    public InteractionService Commands { get; set; }

    private InteractionHandler _handler;

    // Constructor injection is also a valid way to access the dependencies
    public CommandModule(InteractionHandler handler)
    {
        _handler = handler;
    }

    //https://discordnet.dev/guides/text_commands/intro.html
    //[SlashCommand("runas", "Run command as player")]
    //public async Task RunAsPlayer(
    //    [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
    //    [Summary("Command"), Autocomplete(typeof(AceCommandAutocompleteHandler))] string command,
    //    [Summary("Params")] string args = ""
    //    )
    //{
    //    if (PatchClass.Settings.DevIds.Contains(Context.User.Id))
    //    {
    //        if (!string.IsNullOrEmpty(args))
    //            command += " " + args;

    //        if (!await CommandHelpers.TryIssueACECommand(command, player))
    //            await Context.Channel.SendMessageAsync($"Failed to run command.");
    //        else
    //            await Context.Channel.SendMessageAsync($"As {player} issued {command}");
    //    }
    //    else
    //        await Context.Channel.SendMessageAsync($"Not authorized to run commands.");
    //}

    //[SlashCommand("run", "Run command as admin")]
    //public async Task RunAsDev(
    //    [Summary("Command"), Autocomplete(typeof(AceCommandAutocompleteHandler))] string command,
    //    [Summary("Params")] string args = ""
    //    )
    //{
    //    if (PatchClass.Settings.DevIds.Contains(Context.User.Id))
    //    {
    //        if (!string.IsNullOrEmpty(args))
    //            command += " " + args;

    //        if (!await CommandHelpers.TryIssueACECommand(command))
    //            await Context.Channel.SendMessageAsync($"Failed to run command.");
    //        else
    //            await Context.Channel.SendMessageAsync($"Issued {command}");
    //    }
    //    else
    //        await Context.Channel.SendMessageAsync($"Not authorized to run commands.");
    //}

    //[SlashCommand("kick", "Kick a player")]
    //public async Task KickPlayer(
    //[Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player)
    //{
    //    var p = PlayerManager.FindByName(player ?? "") as Player;
    //    if (p is null)
    //    {
    //        await RespondAsync($"Could not find {player}");
    //    }
    //    else
    //    {
    //        //p.Die();
    //        //p.Session.Terminate(ACE.Server.Network.Enum.SessionTerminationReason.AccountBooted);
    //        await RespondAsync($"Kicked {player}");
    //    }
    //}

    #region Unused Command References
    // You can use a number of parameter types in you Slash Command handlers (string, int, double, bool, IUser, IChannel, IMentionable, IRole, Enums) by default. Optionally,
    // you can implement your own TypeConverters to support a wider range of parameter types. For more information, refer to the library documentation.
    // Optional method parameters(parameters with a default value) also will be displayed as optional on Discord.

    // [Summary] lets you customize the name and the description of a parameter
    //[SlashCommand("echo", "Repeat the input")]
    //public async Task Echo(string echo, [Summary(description: "mention the user")] bool mention = false)
    //    => await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));

    //[SlashCommand("ping", "Pings the bot and returns its latency.")]
    //public async Task GreetUserAsync()
    //    => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);

    //[SlashCommand("bitrate", "Gets the bitrate of a specific voice channel.")]
    //public async Task GetBitrateAsync([ChannelTypes(ChannelType.Voice, ChannelType.Stage)] IChannel channel)
    //    => await RespondAsync(text: $"This voice channel has a bitrate of {(channel as IVoiceChannel).Bitrate}");

    // Use [ComponentInteraction] to handle message component interactions. Message component interaction with the matching customId will be executed.
    // Alternatively, you can create a wild card pattern using the '*' character. Interaction Service will perform a lazy regex search and capture the matching strings.
    // You can then access these capture groups from the method parameters, in the order they were captured. Using the wild card pattern, you can cherry pick component interactions.
    //[ComponentInteraction("musicSelect:*,*")]
    //public async Task ButtonPress(string id, string name)
    //{
    //    // ...
    //    await RespondAsync($"Playing song: {name}/{id}");
    //}

    // Select Menu interactions, contain ids of the menu options that were selected by the user. You can access the option ids from the method parameters.
    // You can also use the wild card pattern with Select Menus, in that case, the wild card captures will be passed on to the method first, followed by the option ids.
    //[ComponentInteraction("roleSelect")]
    //public async Task RoleSelect(string[] selections)
    //{
    //    throw new NotImplementedException();
    //}

    // With the Attribute DoUserCheck you can make sure that only the user this button targets can click it. This is defined by the first wildcard: *.
    // See Attributes/DoUserCheckAttribute.cs for elaboration.
    //[DoUserCheck]
    //[ComponentInteraction("myButton:*")]
    //public async Task ClickButtonAsync(string userId)
    //    => await RespondAsync(text: ":thumbsup: Clicked!");

    // Pins a message in the channel it is in.
    //[MessageCommand("pin")]
    //public async Task PinMessageAsync(IMessage message)
    //{
    //    // make a safety cast to check if the message is ISystem- or IUserMessage
    //    if (message is not IUserMessage userMessage)
    //        await RespondAsync(text: ":x: You cant pin system messages!");

    //    // if the pins in this channel are equal to or above 50, no more messages can be pinned.
    //    else if ((await Context.Channel.GetPinnedMessagesAsync()).Count >= 50)
    //        await RespondAsync(text: ":x: You cant pin any more messages, the max has already been reached in this channel!");

    //    else
    //    {
    //        await userMessage.PinAsync();
    //        await RespondAsync(":white_check_mark: Successfully pinned message!");
    //    }
    //}

    // This command will greet target user in the channel this was executed in.
    //[UserCommand("greet")]
    //public async Task GreetUserAsync(IUser user)
    //    => await RespondAsync(text: $":wave: {Context.User} said hi to you, <@{user.Id}>!");

    #endregion
}
