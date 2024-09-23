namespace Discord.Modules;
public class ChatCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; }
    private InteractionHandler _handler;
    public ChatCommandModule(InteractionHandler handler) => _handler = handler;

    // you need to add `Autocomplete` attribute before parameter to add autocompletion to it
    [SlashCommand("t", "Message player")]
    public async Task TellCommand(
        [Summary("Player"), Autocomplete(typeof(PlayerAutocompleteHandler))] string player,
        string message
        )
    {
        var user = this.Context?.User?.Username;

        if (string.IsNullOrEmpty(user))
            await RespondAsync("Unable to find the user of this command.");
        else
        {
            PatchClass.DiscordRelay.SayToTarget(Context.User.Username, player ?? "", message);
            await RespondAsync();
        }
    }

    [SlashCommand("g", "Message gen chat?")]
    public async Task ChatCommand(
    string message
    )
    {
        Debugger.Break();
        var user = this.Context?.User?.Username;
        if (string.IsNullOrEmpty(user))
            await RespondAsync("Unable to find the user of this command.");
        else
        {
            PatchClass.DiscordRelay.SayToGeneralChat(Context.User.Username, message);
            await RespondAsync();
        }
    }

}
