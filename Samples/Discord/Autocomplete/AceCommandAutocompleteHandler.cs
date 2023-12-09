namespace Discord.Autocomplete;

public class AceCommandAutocompleteHandler : AutocompleteHandler
{
    static string[] commands = CommandManager.GetCommands().Select(x => x.Attribute.Command).ToArray();

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var o = autocompleteInteraction.Data.Options;
        var option = autocompleteInteraction.Data.Options.Where(x => x.Focused)?.FirstOrDefault();
        if (option is null)
            return AutocompletionResult.FromError(InteractionCommandError.ParseFailed, "No parameter found.  Contact Bot smith.");

        var typed = option.Value.ToString();

        //var message = parameter as SocketMessage;
        var results = commands
            .Where(x => x.Contains(typed, StringComparison.OrdinalIgnoreCase))
            .Take(25)
            .Select(x => new AutocompleteResult(x, x));

        // max - 25 suggestions at a time (API limit)
        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}