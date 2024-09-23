namespace Discord.Autocomplete;

public class PlayerAutocompleteHandler : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var o = autocompleteInteraction.Data.Options;
        var option = autocompleteInteraction.Data.Options.Where(x => x.Name == "player")?.FirstOrDefault();
        if (option is null)
            return AutocompletionResult.FromError(InteractionCommandError.ParseFailed, "No parameter named player.  Contact Bot smith.");

        var typed = option.Value.ToString();

        //var message = parameter as SocketMessage;
        var results = PlayerManager.GetAllOnline()
            .Where(x => x.Name.StartsWith(typed, StringComparison.OrdinalIgnoreCase))
            .Select(x => new AutocompleteResult(x.Name, x.Name));//x.Guid.ToString()));

        // max - 25 suggestions at a time (API limit)
        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}